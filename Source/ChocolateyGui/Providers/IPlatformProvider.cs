// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IPlatformProvider.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Providers
{
    public interface IPlatformProvider
    {
        Tuple<float, float> GetDpiScaleFactor();
    }
}