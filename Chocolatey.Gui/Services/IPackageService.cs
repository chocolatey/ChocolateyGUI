using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Services
{
    public interface IPackageService
    {
        void SetSource(string packageUrl);
        void SetSource(Uri packageUri);
        Task<PackageSearchResults> Search(string query);
        Task<PackageSearchResults> Search(string query, PackageSearchOptions options);
        IPackageViewModel GetLatest(string id, bool includePrerelease = false);
    }
}
