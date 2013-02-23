using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackageVersionService;

namespace Chocolatey.Explorer.Commands
{
    public class ClearCachePackageVersionCommand : BaseCommand
    {
        public IPackageVersionService PackageVersionService { get; set; }

        public override void Execute()
        {
            this.Log().Info("Clearing cache of all package versions.");
            var versionService = PackageVersionService as ICacheable;
            if (versionService != null)
            {
                versionService.InvalidateCache();
            }
        }
    }
}