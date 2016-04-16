// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IBasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using ChocolateyGui.Models;

    public interface IBasePackageService
    {
        event PackagesChangedEventHandler PackagesUpdated;
    }
}