// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetImage.xaml.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Akavache;
using Autofac;
using Caliburn.Micro;
using CefSharp;
using CefSharp.Internals;
using CefSharp.OffScreen;
using ChocolateyGui.Providers.PlatformProvider;
using ChocolateyGui.Utilities.Extensions;
using Serilog;
using Splat;
using BitmapMixins = Splat.WinForms.BitmapMixins;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Controls
{
    /// <summary>
    ///     Interaction logic for InternetImage.xaml
    /// </summary>
    public partial class InternetImage
    {
        public static readonly DependencyProperty IconUrlProperty = DependencyProperty.Register(
            "IconUrl", typeof(string), typeof(InternetImage), new PropertyMetadata(default(string)));

        private static readonly ILogger Logger = Log.ForContext<InternetImage>();
        private static readonly ChromiumWebBrowser RenderBrowser;
        private static readonly Lazy<BitmapSource> ErrorIcon = new Lazy<BitmapSource>(GetErrorImage);

        private readonly IObjectBlobCache _cache = Bootstrapper.Container.ResolveKeyed<IObjectBlobCache>("Local");

        static InternetImage()
        {
            var browserSettings = new BrowserSettings
            {
                WindowlessFrameRate = 1,
            };
            
            RenderBrowser = new ChromiumWebBrowser(string.Empty, browserSettings)
            {
                Size = GetBitmapSize()
            };
        }

        public InternetImage()
        {
            InitializeComponent();

            if (!RenderBrowser.IsBrowserInitialized)
            {
                RenderBrowser.BrowserInitialized += async (sender, args) =>
                {
                    await Execute.OnUIThreadAsync(async () =>
                    {
                        await SetImage(IconUrl);
                        this.ToObservable(IconUrlProperty, () => IconUrl)
                            .Subscribe(async url => await SetImage(url));
                    });
                };
            }
            else
            {
#pragma warning disable 4014
                SetImage(IconUrl);
#pragma warning restore 4014
                this.ToObservable(IconUrlProperty, () => IconUrl)
                    .Subscribe(async url => await SetImage(url));
            }
        }

        public string IconUrl
        {
            get { return (string) GetValue(IconUrlProperty); }
            set { SetValue(IconUrlProperty, value); }
        }

        private async Task SetImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                PART_Image.Source = null;
                PART_Loading.IsActive = false;
                return;
            }

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                Logger.Warning("Got an invalid img url: \"{IconUrl}\".", url);
                PART_Image.Source = ErrorIcon.Value;
                PART_Loading.IsActive = false;
                return;
            }

            PART_Loading.IsActive = true;
            var imagePart = uri.Segments.Last();
            var fileTypeSeperator = imagePart.LastIndexOf(".", StringComparison.InvariantCulture);
            if (fileTypeSeperator > 0 && imagePart.Substring(fileTypeSeperator + 1).Equals("svg", StringComparison.InvariantCultureIgnoreCase))
            {
                var source = await SvgUrlToBitmapSource(uri.ToString());
                PART_Image.Source = source;
                PART_Loading.IsActive = false;
                return;
            }

            if (fileTypeSeperator < 0)
            {
                Logger.Debug("Got an extensionless img url: \"{IconUrl}\".\nPassing straight to Image.", url);
            }

            var size = GetBitmapSize();
            var image = await _cache.LoadImageFromUrl(url, false, size.Width, size.Height, DateTimeOffset.UtcNow + TimeSpan.FromDays(1));
            PART_Image.Source = image.ToNative();
            PART_Loading.IsActive = false;
        }

        private async Task<BitmapSource> SvgUrlToBitmapSource(string url)
        {
            var size = GetBitmapSize();
            var image = await _cache.LoadImage(url, size.Width, size.Height)
                .Catch<IBitmap, KeyNotFoundException>(error =>
                {
                    return Observable.FromAsync(async () =>
                    {
                        var html = $@"<style>
                            img {{ width: 100%; height: auto; }}
                            body {{ margin: 0 }}
                            </style>
                            <img src=""{url}"">";
                        html = MarkdownViewer.HtmlTemplate.Replace("{{content}}", html);
                        await LoadHtmlAsync(RenderBrowser, html, "http://rawhtml/svg");

                        using (var result = await RenderBrowser.ScreenshotAsync(true))
                        {
                            using (var stream = new MemoryStream())
                            {
                                result.Save(stream, ImageFormat.Png);
                                await _cache.Insert(url, stream.ToArray(), DateTimeOffset.UtcNow + TimeSpan.FromDays(1));
                                stream.Position = 0;
                                return await BitmapLoader.Current.Load(stream, null, null);
                            }
                        }
                    });
                });
            return image.ToNative();
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        private static BitmapSource LoadBitmap(Bitmap source)
        {
            var ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = Imaging.CreateBitmapSourceFromHBitmap(ip,
                    IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }

        private static Task LoadBitmap(BitmapSource img)
        {
            if (!img.IsDownloading)
            {
                return Task.FromResult(true);
            }

            // If using .Net 4.6 then use TaskCreationOptions.RunContinuationsAsynchronously
            // and switch to tcs.TrySetResult below - no need for the custom extension method
            var tcs = new TaskCompletionSource<bool>();

            EventHandler handler = null;
            handler = (sender, args) =>
            {
                img.DownloadCompleted -= handler;

                // This is required when using a standard TaskCompletionSource
                // Extension method found in the CefSharp.Internals namespace
                tcs.TrySetResultAsync(true);
            };

            img.DownloadCompleted += handler;

            return tcs.Task;
        }

        private static void DisplayBitmap(Bitmap bitmap)
        {
            // Make a file to save it to (e.g. C:\Users\jan\Desktop\CefSharp screenshot.png)
            var screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CefSharp screenshot" + DateTime.Now.Ticks + ".png");

            Console.WriteLine();
            Console.WriteLine("Screenshot ready. Saving to {0}", screenshotPath);

            // Save the Bitmap to the path.
            // The image type is auto-detected via the ".png" extension.
            bitmap.Save(screenshotPath);

            // We no longer need the Bitmap.
            // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
            bitmap.Dispose();

            Console.WriteLine("Screenshot saved.  Launching your default image viewer...");

            // Tell Windows to launch the saved image.
            Process.Start(screenshotPath);

            Console.WriteLine("Image viewer launched.  Press any key to exit.");
        }

        private static Task LoadHtmlAsync(IWebBrowser browser, string html, string address)
        {
            // If using .Net 4.6 then use TaskCreationOptions.RunContinuationsAsynchronously
            // and switch to tcs.TrySetResult below - no need for the custom extension method
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<FrameLoadEndEventArgs> handler = null;
            handler = (sender, args) =>
            {
                browser.FrameLoadEnd -= handler;

                // This is required when using a standard TaskCompletionSource
                // Extension method found in the CefSharp.Internals namespace
                tcs.TrySetResultAsync(true);
            };

            browser.FrameLoadEnd += handler;

            if (!string.IsNullOrEmpty(address))
            {
                browser.LoadHtml(html, address);
            }

            return tcs.Task;
        }

        private static BitmapSource GetErrorImage()
        {
            var size = GetBitmapSize();
            return Imaging.CreateBitmapSourceFromHIcon(
                SystemIcons.Error.Handle, Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
        }

        private static System.Drawing.Size GetBitmapSize()
        {
            var scale = (new Windows7PlatformProvider()).GetDpiScaleFactor();
            var x = (int) Math.Round(64 * scale.Item1);
            var y = (int)Math.Round(64 * scale.Item2);
            return new System.Drawing.Size(x, y);
        }
    }
}