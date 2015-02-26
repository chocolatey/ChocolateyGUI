// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Threading.Tasks;
    using ChocolateyGui.Models;
    using ChocolateyGui.ViewModels.Items;

    public interface IPackageService
    {
        Task<PackageSearchResults> Search(string query, Uri source = null);

        Task<PackageSearchResults> Search(string query, PackageSearchOptions options, Uri source = null);

        Task<IPackageViewModel> GetLatest(string id, bool includePrerelease = false, Uri source = null);

        Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel viewModel, Uri source = null);

        Task<bool> TestSourceUrl(Uri source);
    }
}