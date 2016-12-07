// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IChocolateyPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChocolateyGui.ViewModels.Items;
using NuGet;

namespace ChocolateyGui.Services.PackageServices
{
    public interface IChocolateyPackageService
    {
        Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false);

        Task InstallPackage(string id, SemanticVersion version = null, Uri source = null, bool force = false);

        Task UninstallPackage(string id, SemanticVersion version, bool force = false);

        Task UpdatePackage(string id, Uri source = null);
    }
}