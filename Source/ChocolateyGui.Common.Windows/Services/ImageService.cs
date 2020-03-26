// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ImageService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChocolateyGui.Common.Windows.Services
{
    public class ImageService : IImageService
    {
        public string SplashScreenImageName
        {
            get
            {
                var dpi = NativeMethods.GetScaleFactor();
                var img = "chocolatey.png";

                if (dpi >= 2f)
                {
                    img = "chocolatey@3.png";
                }
                else if (dpi > 1.00f)
                {
                    img = "chocolatey@2.png";
                }

                return img;
            }
        }

        public ImageSource PrimaryApplicationImage
        {
            get
            {
                var image = new BitmapImage(new Uri("pack://application:,,,/ChocolateyGui;component/chocolatey_logo.png", UriKind.RelativeOrAbsolute));
                image.Freeze();
                return image;
            }
        }

        public ImageSource SecondaryApplicationImage
        {
            get { return null; }
        }

        public Uri ToolbarIconUri
        {
            get { return new Uri("pack://application:,,,/chocolateyicon.ico"); }
        }
    }
}