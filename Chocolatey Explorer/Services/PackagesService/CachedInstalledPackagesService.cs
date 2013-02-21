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
            _installedPackageCache = null;
            _invalidateCacheTime = DateTime.Now.AddMinutes(30);
        }

        public void ListOfIntalledPackages()
        {
            if (_installedPackageCache == null || DateTime.Now > _invalidateCacheTime)
            {
                OnRunStarted();
                _installedPackagesService.ListOfIntalledPackages();
            }
            else
            {
                OnRunFinshed(_installedPackageCache);
            }
        }

        public void ListOfDistinctHighestInstalledPackages()
        {
            if (_installedPackageCache == null || DateTime.Now > _invalidateCacheTime)
            {
                OnRunStarted();
                _installedPackagesService.ListOfDistinctHighestInstalledPackages();
            }
            else
            {
                OnRunFinshed(_installedPackageCache);
            }
        }

        private void OnRunFinshed(IList<Package> packages)
        {
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
            _installedPackageCache = packages;
            OnRunFinshed(packages);
        }

        private void InstalledPackagesServiceRunFailed(Exception exc)
        {
            if (RunFailed != null)
                RunFailed(exc);
        }

        private void OnRunStarted()
        {
            var handler = RunStarted;
            if (handler != null) handler("Getting list of installed packages.");
        }
        
    }
}