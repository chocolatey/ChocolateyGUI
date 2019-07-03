// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ISplashScreenService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Services
{
    public interface ISplashScreenService
    {
        void Show();

        void Close(TimeSpan duration);
    }
}