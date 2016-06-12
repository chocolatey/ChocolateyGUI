// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IRemotePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using ChocolateyGui.Models;
using ChocolateyGui.ViewModels.Items;
using NuGet;

namespace ChocolateyGui.Services
{
    public interface IRemotePackageService
    {
        Task<PackageSearchResults> Search(string query);

        Task<PackageSearchResults> Search(string query, PackageSearchOptions options);

        Task<IPackageViewModel> GetLatest(string id, bool includePrerelease = false);

        Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version, bool isPrerelease);
    }
}