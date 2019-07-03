// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IBrandingService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Media;

namespace ChocolateyGui.Common.Windows.Services
{
    public interface IImageService
    {
        string SplashScreenImageName { get; }

        ImageSource PrimaryApplicationImage { get; }

        ImageSource SecondaryApplicationImage { get; }

        Uri ToolbarIconUri { get; }
    }
}