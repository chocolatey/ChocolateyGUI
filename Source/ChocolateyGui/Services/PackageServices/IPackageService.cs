using System;
using System.Threading.Tasks;
using ChocolateyGui.Models;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Services
{
    public interface IPackageService
    {
        Task<PackageSearchResults> Search(string query, Uri source = null);
        Task<PackageSearchResults> Search(string query, PackageSearchOptions options, Uri source = null);
        Task<IPackageViewModel> GetLatest(string id, bool includePrerelease = false, Uri source = null);
        Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm, Uri source = null);

        Task<bool> TestSourceUrl(Uri source);
    }
}
