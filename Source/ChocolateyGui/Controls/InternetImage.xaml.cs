// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetImage.xaml.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using CefSharp;
using CefSharp.Internals;
using CefSharp.OffScreen;
using ChocolateyGui.Providers.PlatformProvider;
using ChocolateyGui.Utilities.Extensions;
using LiteDB;
using Nito.AsyncEx;
using Serilog;
using Splat;
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
        private static readonly LiteDatabase Data = Caliburn.Micro.IoC.Get<LiteDatabase>();
        private static readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();

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
            get { return (string)GetValue(IconUrlProperty); }
            set { SetValue(IconUrlProperty, value); }
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
                SystemIcons.Error.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
        }

        private static System.Drawing.Size GetBitmapSize()
        {
            var scale = new Windows7PlatformProvider().GetDpiScaleFactor();
            var x = (int)Math.Round(64 * scale.Item1);
            var y = (int)Math.Round(64 * scale.Item2);
            return new System.Drawing.Size(x, y);
        }

        private static void UploadFileAndSetMetadata(DateTime absoluteExpiration, MemoryStream imageStream, LiteFileStorage fileStorage, string id)
        {
            imageStream.Position = 0;
            var fileInfo = fileStorage.Upload(id, imageStream);
            fileStorage.SetMetadata(
                fileInfo.Id,
                new BsonDocument(new Dictionary<string, BsonValue> { { "Expires", absoluteExpiration } }));

            imageStream.Position = 0;
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

            var size = GetBitmapSize();
            var expiration = DateTime.UtcNow + TimeSpan.FromDays(1);

            var imagePart = uri.Segments.Last();
            var fileTypeSeperator = imagePart.LastIndexOf(".", StringComparison.InvariantCulture);

            BitmapSource source;
            if (fileTypeSeperator > 0 &&
                imagePart.Substring(fileTypeSeperator + 1).Equals("svg", StringComparison.InvariantCultureIgnoreCase))
            {
                source = (await LoadSvg(url, size.Width, size.Height, expiration)).ToNative();
            }
            else
            {
                try
                {
                    source = (await LoadImage(url, size.Width, size.Height, expiration)).ToNative();
                }
                catch (HttpRequestException)
                {
                    source = ErrorIcon.Value;
                }
            }

            PART_Image.Source = source;
            PART_Loading.IsActive = false;
        }

        private async Task<IBitmap> LoadImage(string url, float? desiredWidth, float? desiredHeight, DateTime absoluteExpiration)
        {
            var imageStream = await DownloadUrl(url, absoluteExpiration);
            return await BitmapLoader.Current.Load(imageStream, desiredWidth, desiredHeight);
        }

        private async Task<IBitmap> LoadSvg(
            string url,
            float? desiredWidth,
            float? desiredHeight,
            DateTime absoluteExpiration)
        {
            using (var upgradeToken = await Lock.UpgradeableReaderLockAsync())
            {
                var id = $"imagecache/{url.GetHashCode()}";
                using (var imageStream = new MemoryStream())
                {
                    var fileStorage = Data.FileStorage;
                    if (fileStorage.Exists(id))
                    {
                        var info = fileStorage.FindById(id);
                        var expires = info.Metadata["Expires"].AsDateTime;
                        if (expires > DateTime.UtcNow)
                        {
                            info.CopyTo(imageStream);
                            return await BitmapLoader.Current.Load(imageStream, desiredWidth, desiredHeight);
                        }

                        fileStorage.Delete(id);
                    }

                    using (await upgradeToken.UpgradeAsync())
                    {
                        // If we couldn't find the image or it expired
                        var html = $@"<style>
                            img {{ width: 100%; height: auto; }}
                            body {{ margin: 0 }}
                            </style>
                            <img src=""{url}"">";
                        html = MarkdownViewer.HtmlTemplate.Replace("{{content}}", html);
                        await LoadHtmlAsync(RenderBrowser, html, "http://rawhtml/svg");

                        using (var result = await RenderBrowser.ScreenshotAsync(true))
                        {
                            result.Save(imageStream, ImageFormat.Png);
                            imageStream.Position = 0;
                        }

                        UploadFileAndSetMetadata(absoluteExpiration, imageStream, fileStorage, id);
                        return await BitmapLoader.Current.Load(imageStream, null, null);
                    }
                }
            }
        }

        private async Task<Stream> DownloadUrl(string url, DateTime absoluteExpiration)
        {
            using (var upgradeToken = await Lock.UpgradeableReaderLockAsync())
            {
                var id = $"imagecache/{url.GetHashCode()}";
                var imageStream = new MemoryStream();

                var fileStorage = Data.FileStorage;
                if (fileStorage.Exists(id))
                {
                    var info = fileStorage.FindById(id);
                    var expires = info.Metadata["Expires"].AsDateTime;
                    if (expires > DateTime.UtcNow)
                    {
                        info.CopyTo(imageStream);
                        return imageStream;
                    }

                    fileStorage.Delete(id);
                }

                using (await upgradeToken.UpgradeAsync())
                {
                    // If we couldn't find the image or it expired
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        await response.Content.CopyToAsync(imageStream);
                    }

                    UploadFileAndSetMetadata(absoluteExpiration, imageStream, fileStorage, id);
                    return imageStream;
                }
            }
        }
    }
}