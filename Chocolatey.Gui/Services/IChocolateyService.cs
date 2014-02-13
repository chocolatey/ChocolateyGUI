using System.Collections.Generic;
using System.Threading.Tasks;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Services
{
    public interface IChocolateyService
    {
        Task<IEnumerable<IPackageViewModel>> GetPackages(bool force = false);
        Task<IEnumerable<IPackageViewModel>> GetPackagesFromLocalDirectory(string directoryPath);
        void InstallPackage(string id, SemanticVersion version = null, string source = null);
        void UninstallPackage(string id, SemanticVersion version = null, bool force = false);
        void UpdatePackage(string id);
        bool IsPackageInstalled(string id, SemanticVersion version);

        event PackagesChangedEventHandler PackagesUpdated;
    }
}
