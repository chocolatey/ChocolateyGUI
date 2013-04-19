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
        public event Delegates.StartedDelegate RunStarted;

        public void ListOfIntalledPackages()
        {
            this.Log().Info("Getting list of installed packages");
            OnRunStarted();
            Task.Factory.StartNew(() => _libDirHelper.ReloadFromDir())
                        .ContinueWith(task =>
                        {
                            if (!task.IsFaulted)
                            {
                                this.Log().Debug("Run finished");
                                OnRunFinshed(task.Result);
                            }
                            else if (task.IsFaulted && RunFailed != null)
                            {
                                this.Log().Debug("Run failed");
                                RunFailed(task.Exception);
                            }
                        });
        }

        public void ListOfDistinctHighestInstalledPackages()
        {
            this.Log().Info("Getting list of installed packages");
            OnRunStarted(); 
            Task.Factory.StartNew(() => _libDirHelper.ReloadFromDir())
                        .ContinueWith(task =>
                            {
                                if (!task.IsFaulted)
                                {
                                    this.Log().Debug("Run finished");
                                    var results = task.Result.OrderByDescending(x => x.InstalledVersion, new PackagesSorter()).Distinct().OrderBy(x => x.Name).ToList();
                                    OnRunFinshed(results);
                                }
                                else if (task.IsFaulted && RunFailed != null)
                                {
                                    this.Log().Debug("Run failed");
                                    RunFailed(task.Exception);
                                }
                            });
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            this.Log().Debug("Run finsished");
            var handler = RunFinshed;
            if (handler != null) handler(packages);
        }

        private void OnRunStarted()
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler("Getting list of installed packages.");
        }
    }
}