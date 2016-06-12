// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IBasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Models;

namespace ChocolateyGui.Services
{
    public interface IBasePackageService
    {
        event PackagesChangedEventHandler PackagesUpdated;
    }
}