using System.Collections.Generic;
using System.Linq;
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
        public event Delegates.FinishedDelegate RunFinshed;
        public event Delegates.FailedDelegate RunFailed;
        
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

        public void ListOfDistinctHighestInstalledPackages()
        {
            this.Log().Info("Getting list of installed packages");
            Task.Factory.StartNew(() => _libDirHelper.ReloadFromDir())
                        .ContinueWith((task) =>
                            {
                                if (!task.IsFaulted)
                                {
                                    var results = task.Result.OrderByDescending(x => x.InstalledVersion,new PackagesSorter()).Distinct().OrderBy(x=> x.Name).ToList();
                                    OnRunFinshed(results);
                                }
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