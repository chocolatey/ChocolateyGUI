using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackagesService;

namespace Chocolatey.Explorer.Commands
{
    public class ClearCacheAvailablePackagesCommand : BaseCommand
    {
        public IAvailablePackagesService AvailablePackagesService { get; set; }

        public override void Execute()
        {
            this.Log().Info("Clearing cache of available packages.");
            var service = AvailablePackagesService as ICacheable;
            if (service != null)
            {
                service.InvalidateCache();
            }
        }
    }
}