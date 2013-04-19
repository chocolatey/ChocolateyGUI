using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackagesService;

namespace Chocolatey.Explorer.Commands
{
    /// <summary>
    /// Gets the <see cref="IInstalledePackagesService"/> from the container and then chechs if it is of type <see cref="ICacheable"/>.
    /// If it is it will then call InvalidateCache.
    /// </summary>
    public class ClearCacheInstalledPackagesCommand : BaseCommand
    {
        public IInstalledPackagesService InstalledPackagesService { get; set; }

        public override void Execute()
        {
            this.Log().Info("clearing cache of all installed packages.");
            var cacheable = InstalledPackagesService as ICacheable;
            if (cacheable != null)
            {
                cacheable.InvalidateCache();
            }
        }
    }
}