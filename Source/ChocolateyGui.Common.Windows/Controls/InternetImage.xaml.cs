// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetImage.xaml.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using Serilog;
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
        private static readonly IPackageIconService PackageIconService = IoC.Get<IPackageIconService>();

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

        private async Task SetImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                PART_Image.Source = PackageIconService.GetEmptyIconImage();
                PART_Loading.IsActive = false;
                return;
            }

            PART_Loading.IsActive = true;

            var size = GetCurrentSize();
            var expiration = DateTime.UtcNow + TimeSpan.FromDays(1);
            ImageSource source;
            try
            {
                source = await PackageIconService.GetImage(url, size, expiration);
            }
            catch (HttpRequestException)
            {
                source = PackageIconService.GetErrorIconImage();
            }
            catch (ArgumentException exception)
            {
                Logger.Warning(exception, $"Got an invalid img url: \"{url}\".");
                source = PackageIconService.GetErrorIconImage();
            }
            catch (Exception exception)
            {
                Logger.Warning(exception, $"Something went wrong with: \"{url}\".");
                source = PackageIconService.GetErrorIconImage();
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