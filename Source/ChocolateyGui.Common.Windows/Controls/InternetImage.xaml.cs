// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetImage.xaml.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using ImageMagick;
using LiteDB;
using MahApps.Metro.IconPacks;
using Microsoft.VisualStudio.Threading;
using Serilog;
using Splat;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Common.Windows.Controls
{
    /// <summary>
    ///     Interaction logic for InternetImage.xaml
    /// </summary>
    public partial class InternetImage
    {
        public static readonly DependencyProperty IconUrlProperty = DependencyProperty.Register(
            "IconUrl", typeof(string), typeof(InternetImage), new PropertyMetadata(default(string)));

        private static readonly ILogger Logger = Log.ForContext<InternetImage>();
        private static readonly Lazy<ImageSource> ErrorIcon = new Lazy<ImageSource>(() => GetPackIconEntypoImage(PackIconEntypoKind.CircleWithCross, Brushes.OrangeRed));
        private static readonly Lazy<ImageSource> EmptyIcon = new Lazy<ImageSource>(GetEmptyImage);
        private static readonly LiteDatabase Data = IoC.Get<LiteDatabase>();
        private static readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();

        public InternetImage()
        {
            InitializeComponent();
#pragma warning disable 4014
            SetImage(IconUrl);
#pragma warning restore 4014
            this.ToObservable(IconUrlProperty, () => IconUrl)
                .Subscribe(async url => await SetImage(url));
        }

        public string IconUrl
        {
            get { return (string)GetValue(IconUrlProperty); }
            set { SetValue(IconUrlProperty, value); }
        }

        private static ImageSource GetPackIconEntypoImage(PackIconEntypoKind packIconKind, Brush brush)
        {
            var packIcon = new PackIconEntypo { Kind = packIconKind };

            var pen = new Pen();
            pen.Freeze();
            var geometry = Geometry.Parse(packIcon.Data);
            var geometryDrawing = new GeometryDrawing(brush, pen, geometry);
            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(geometryDrawing);
            drawingGroup.Transform = new ScaleTransform(3.5, 3.5);
            var drawingImage = new DrawingImage(drawingGroup);
            drawingImage.Freeze();
            return drawingImage;
        }

        private static ImageSource GetEmptyImage()
        {
            var image = new BitmapImage(new Uri("pack://application:,,,/ChocolateyGui;component/chocolatey@4.png", UriKind.RelativeOrAbsolute));
            image.Freeze();
            return image;
        }

        private static void UploadFileAndSetMetadata(DateTime absoluteExpiration, MemoryStream imageStream, LiteStorage fileStorage, string id)
        {
            imageStream.Position = 0;
            var fileInfo = fileStorage.Upload(id, null, imageStream);
            fileInfo.Metadata.Add(new KeyValuePair<string, BsonValue>("Expires", absoluteExpiration));
            imageStream.Position = 0;
        }

        private async Task<IBitmap> LoadImage(string url, float desiredWidth, DateTime absoluteExpiration)
        {
            var imageStream = await DownloadUrl(url, desiredWidth, absoluteExpiration).ConfigureAwait(false);

            // Don't specify width and height to keep the aspect ratio of the image.
            return await BitmapLoader.Current.Load(imageStream, null, null);
        }

        private async Task<Stream> DownloadUrl(string url, float desiredWidth, DateTime absoluteExpiration)
        {
            var id = $"imagecache/{url.GetHashCode()}";
            var imageStream = new MemoryStream();
            var fileStorage = Data.FileStorage;

            using (await Lock.UpgradeableReadLockAsync())
            {
                if (fileStorage.Exists(id))
                {
                    var info = fileStorage.FindById(id);
                    var expires = info.Metadata["Expires"].AsDateTime;
                    if (expires > DateTime.UtcNow)
                    {
                        info.CopyTo(imageStream);
                        return imageStream;
                    }
                }
            }

            // If we couldn't find the image or it expired
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var extension = GetExtension(url);

                using (var memoryStream = new MemoryStream())
                {
                    await response.Content.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    MagickReadSettings readSettings = null;
                    if (string.Equals(extension, "svg", StringComparison.OrdinalIgnoreCase))
                    {
                        readSettings = new MagickReadSettings { Format = MagickFormat.Svg };
                    }
                    else if (string.Equals(extension, "svgz", StringComparison.OrdinalIgnoreCase))
                    {
                        readSettings = new MagickReadSettings { Format = MagickFormat.Svgz };
                    }

                    using (var image = new MagickImage(memoryStream, readSettings))
                    {
                        // Resize the image to the desired width. When zero is specified for the height
                        // the height will be calculated with the aspect ratio.
                        image.Resize((int)desiredWidth, 0);

                        image.Write(imageStream, MagickFormat.Png);
                        imageStream.Flush();
                    }
                }
            }

            using (await Lock.WriteLockAsync())
            {
                // we don't need to delete the file, cause a upload does
                // Upload: Send file or stream to database. Can be used with file or Stream. If file already exists, file content is overwritten.
                UploadFileAndSetMetadata(absoluteExpiration, imageStream, fileStorage, id);
            }

            return imageStream;
        }

        private System.Drawing.Size GetCurrentSize()
        {
            var scale = NativeMethods.GetScaleFactor();
            var x = (int)Math.Round(ActualWidth * scale);
            var y = (int)Math.Round(ActualHeight * scale);
            return new System.Drawing.Size(x, y);
        }

        private async Task SetImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                PART_Image.Source = EmptyIcon.Value;
                PART_Loading.IsActive = false;
                return;
            }

            PART_Loading.IsActive = true;

            var size = GetCurrentSize();
            var expiration = DateTime.UtcNow + TimeSpan.FromDays(1);
            ImageSource source;
            try
            {
                source = (await LoadImage(url, size.Width, expiration)).ToNative();
            }
            catch (HttpRequestException)
            {
                source = ErrorIcon.Value;
            }
            catch (ArgumentException)
            {
                Logger.Warning("Got an invalid img url: \"{IconUrl}\".", url);
                source = ErrorIcon.Value;
            }
            catch (Exception exception)
            {
                Logger.Warning(exception, "Something went wrong with: \"{IconUrl}\".", url);
                source = ErrorIcon.Value;
            }

            PART_Image.Source = source;
            PART_Loading.IsActive = false;
        }

        private string GetExtension(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                throw new ArgumentException(nameof(url));
            }

            var imagePart = uri.Segments.Last();
            var fileTypeSeperator = imagePart.LastIndexOf(".", StringComparison.InvariantCulture);
            if (fileTypeSeperator <= 0)
            {
                return string.Empty;
            }

            return imagePart.Substring(fileTypeSeperator + 1);
        }
    }
}