using System;
using System.Collections.Generic;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services.PackagesService
{
    public class CachedInstalledPackagesService : IInstalledPackagesService, ICacheable
    {
        public event Delegates.StartedDelegate RunStarted;
        public event Delegates.FinishedDelegate RunFinshed;
        public event Delegates.FailedDelegate RunFailed;

        private readonly IInstalledPackagesService _installedPackagesService;
        private IList<Package> _installedPackageCache;
        private DateTime _invalidateCacheTime;

        public CachedInstalledPackagesService(InstalledPackagesService installedPackagesService)
        {
            _installedPackagesService = installedPackagesService;
			_installedPackagesService.RunFailed += InstalledPackagesServiceRunFailed;
            _installedPackagesService.RunFinshed += OnUncachedInstalledRunFinished;
            InvalidateCache();
        }

        public void InvalidateCache()
        {
            this.Log().Debug("Invalidate cache");
            _installedPackageCache = null;
            _invalidateCacheTime = DateTime.Now.AddMinutes(30);
        }

        public void ListOfIntalledPackages()
        {
            if (_installedPackageCache == null || DateTime.Now > _invalidateCacheTime)
            {
                OnRunStarted();
                this.Log().Debug("Get list of packages from source.");
                _installedPackagesService.ListOfIntalledPackages();
            }
            else
            {
                this.Log().Debug("Get list of packages from cache.");
                OnRunFinshed(_installedPackageCache);
            }
        }

        public void ListOfDistinctHighestInstalledPackages()
        {
            if (_installedPackageCache == null || DateTime.Now > _invalidateCacheTime)
            {
                OnRunStarted();
                this.Log().Debug("Get list of distinct packages from server.");
                _installedPackagesService.ListOfDistinctHighestInstalledPackages();
            }
            else
            {
                this.Log().Debug("Get list of distinct packages from cache.");
                OnRunFinshed(_installedPackageCache);
            }
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            this.Log().Debug("Run finished");
            var handler = RunFinshed;
            if (handler != null) handler(packages);
        }

        /// <summary>
        /// Called when the package service that should be cached finishes
        /// its run for currently packages currently available on the server.
        /// </summary>
        /// <param name="packages"></param>
        private void OnUncachedInstalledRunFinished(IList<Package> packages)
        {
            this.Log().Debug("Uncached run finished");
            _installedPackageCache = packages;
            OnRunFinshed(packages);
        }

        private void InstalledPackagesServiceRunFailed(Exception exc)
        {
            this.Log().Debug("Run failed");
            if (RunFailed != null)
                RunFailed(exc);
        }

        private void OnRunStarted()
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler("Getting list of installed packages.");
        }
        
    }
}