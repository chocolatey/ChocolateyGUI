// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetImage.xaml.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using LiteDB;
using MahApps.Metro.IconPacks;
using Microsoft.VisualStudio.Threading;
using Serilog;
using SkiaSharp;
using Splat;
using Svg.Skia;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Common.Windows.Controls
{
    /// <summary>
    ///     Interaction logic for InternetImage.xaml
    /// </summary>
    public partial class InternetImage
    {
        public static readonly DependencyProperty IconUrlProperty = DependencyProperty.Register(
            nameof(IconUrl), typeof(string), typeof(InternetImage), new PropertyMetadata(default(string)));

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

        private static bool IsSvg(string url)
        {
            var extension = Path.GetExtension(url)?.ToLower();
            return extension == ".svg" || extension == ".svgz";
        }

        private static void ExtractImageFromStream(string url, Size desiredSize, Stream inputStream, Stream imageStream)
        {
            if (IsSvg(url))
            {
                using (var svg = new SKSvg())
                {
                    try
                    {
                        svg.Load(inputStream);
                    }
                    catch (Exception exception)
                    {
                        Logger.Warning(exception, $"Something is wrong with: \"{url}\".");
                    }

                    var skPicture = svg.Picture;
                    var imageInfo = new SKImageInfo((int)desiredSize.Width, (int)desiredSize.Height);
                    using (var surface = SKSurface.Create(imageInfo))
                    {
                        using (var canvas = surface.Canvas)
                        {
                            // calculate the scaling need to fit to desired size
                            var scaleX = desiredSize.Width / skPicture.CullRect.Width;
                            var scaleY = desiredSize.Height / skPicture.CullRect.Height;
                            var matrix = SKMatrix.MakeScale((float)scaleX, (float)scaleY);

                            // draw the svg
                            canvas.Clear(SKColors.Transparent);
                            canvas.DrawPicture(skPicture, ref matrix);
                            canvas.Flush();

                            using (var data = surface.Snapshot())
                            {
                                using (var pngImage = data.Encode(SKEncodedImageFormat.Png, 100))
                                {
                                    pngImage.SaveTo(imageStream);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var bitmap = SKBitmap.Decode(inputStream);
                if (bitmap != null)
                {
                    var resizeInfo = GetResizeSkImageInfo(desiredSize, bitmap);
                    using (var resizedBitmap = bitmap.Resize(resizeInfo, SKFilterQuality.High))
                    {
                        bitmap.Dispose();

                        using (var image = SKImage.FromBitmap(resizedBitmap))
                        {
                            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                            {
                                data.SaveTo(imageStream);
                            }
                        }
                    }
                }
            }
        }

        private static SKImageInfo GetResizeSkImageInfo(Size desiredSize, SKBitmap bitmap)
        {
            var resizeInfo = new SKImageInfo((int)desiredSize.Width, (int)desiredSize.Height);

            // Test whether there is more room in width or height
            if (Math.Abs(bitmap.Width - desiredSize.Width) > Math.Abs(bitmap.Height - desiredSize.Height))
            {
                // More room in width, so leave image width set to canvas width
                // and increase/decrease height by same ratio
                var widthRatio = (double)desiredSize.Width / (double)bitmap.Width;
                var newHeight = (int)Math.Floor(bitmap.Height * widthRatio);

                resizeInfo.Height = newHeight;
            }
            else
            {
                // More room in height, so leave image height set to canvas height
                // and increase/decrease width by same ratio
                var heightRatio = (double)desiredSize.Height / (double)bitmap.Height;
                var newWidth = (int)Math.Floor(bitmap.Width * heightRatio);

                resizeInfo.Width = newWidth;
            }

            return resizeInfo;
        }

        private async Task<IBitmap> LoadImage(string url, Size desiredSize, DateTime absoluteExpiration)
        {
            var imageStream = await DownloadUrl(url, desiredSize, absoluteExpiration).ConfigureAwait(false);

            // Don't specify width and height to keep the aspect ratio of the image.
            return await BitmapLoader.Current.Load(imageStream, null, null);
        }

        private async Task<Stream> DownloadUrl(string url, Size desiredSize, DateTime absoluteExpiration)
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

                using (var memoryStream = new MemoryStream())
                {
                    await response.Content.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    ExtractImageFromStream(url, desiredSize, memoryStream, imageStream);
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
                source = (await LoadImage(url, size, expiration)).ToNative();
            }
            catch (HttpRequestException)
            {
                source = ErrorIcon.Value;
            }
            catch (ArgumentException exception)
            {
                Logger.Warning(exception, $"Got an invalid img url: \"{url}\".");
                source = ErrorIcon.Value;
            }
            catch (Exception exception)
            {
                Logger.Warning(exception, $"Something went wrong with: \"{url}\".");
                source = ErrorIcon.Value;
            }

            PART_Image.Source = source;
            PART_Loading.IsActive = false;
        }

        private Size GetCurrentSize()
        {
            var scale = NativeMethods.GetScaleFactor();
            var x = (int)Math.Round(ActualWidth * scale);
            var y = (int)Math.Round(ActualHeight * scale);
            return new Size(x, y);
        }
    }
}