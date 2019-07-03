// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SplashScreenService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Windows.Services;

namespace ChocolateyGui.Services
{
    public class SplashScreenService : ISplashScreenService
    {
        private readonly IImageService _imageService;
        private SplashScreen _splashScreen;

        public SplashScreenService(IImageService imageService)
        {
            _imageService = imageService;
        }

        public void Show()
        {
            _splashScreen = new SplashScreen(_imageService.SplashScreenImageName);
            _splashScreen.Show(true, true);
        }

        public void Close(TimeSpan duration)
        {
            _splashScreen.Close(duration);
        }
    }
}