// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IChocolateyPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChocolateyGui.Subprocess.Models;
using ChocolateyGui.ViewModels.Items;
using NuGet;
using PackageSearchResults = ChocolateyGui.Models.PackageSearchResults;

namespace ChocolateyGui.Services
{
    public interface IChocolateyPackageService
    {
        Task<PackageSearchResults> Search(string query, PackageSearchOptions options);

        Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version, bool isPrerelease);

        Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false);

        Task<IReadOnlyList<Tuple<string, SemanticVersion>>> GetOutdatedPackages(bool includePrerelease = false);

        Task InstallPackage(string id, SemanticVersion version = null, Uri source = null, bool force = false);

        Task UninstallPackage(string id, SemanticVersion version, bool force = false);

        Task UpdatePackage(string id, Uri source = null);

        Task PinPackage(string id, SemanticVersion version);

        Task UnpinPackage(string id, SemanticVersion version);

        Task<IReadOnlyList<ChocolateyFeature>> GetFeatures();

        Task SetFeature(ChocolateyFeature feature);

        ValueTask<bool> RequiresElevation();

        Task<IReadOnlyList<ChocolateySetting>> GetSettings();

        Task SetSetting(ChocolateySetting setting);
    }
}