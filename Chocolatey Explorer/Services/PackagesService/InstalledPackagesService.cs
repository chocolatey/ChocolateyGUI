using System.Collections.Generic;
using System.Threading.Tasks;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services.PackagesService
{
    public class InstalledPackagesService : IInstalledPackagesService
    {
        private readonly IChocolateyLibDirHelper _libDirHelper;

        public InstalledPackagesService(IChocolateyLibDirHelper libDirHelper)
        {
            _libDirHelper = libDirHelper;
        }
        public event AvailablePackagesService.FinishedDelegate RunFinshed;
        public event AvailablePackagesService.FailedDelegate RunFailed;
        
        public void ListOfIntalledPackages()
        {
            this.Log().Info("Getting list of installed packages");
            Task.Factory.StartNew(() => _libDirHelper.ReloadFromDir())
                        .ContinueWith((task) =>
                        {
                            if (!task.IsFaulted)
                                OnRunFinshed(task.Result);
                            else if (task.IsFaulted && RunFailed != null)
                                RunFailed(task.Exception);
                        });
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            var handler = RunFinshed;
            if (handler != null) handler(packages);
        }
    }
}