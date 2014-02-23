using System;
using System.Threading.Tasks;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Services
{
    public interface IPackageService
    {
        Task<PackageSearchResults> Search(string query, Uri source = null);
        Task<PackageSearchResults> Search(string query, PackageSearchOptions options, Uri source = null);
        IPackageViewModel GetLatest(string id, bool includePrerelease = false, Uri source = null);
        Task<IPackageViewModel> EnsureIsLoaded(IPackageViewModel vm, Uri source = null);

        Task<bool> TestSourceUrl(Uri source);
    }
}
