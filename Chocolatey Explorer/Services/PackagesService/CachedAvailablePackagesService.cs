using System;
using System.Collections.Generic;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.SourceService;

namespace Chocolatey.Explorer.Services.PackagesService
{
    /// <summary>
    /// Adapter for _availablePackagesService, but caching the Package-Lists
    /// internally.
    /// </summary>
    class CachedAvailablePackagesService : IAvailablePackagesService, ICacheable
    {
        public event Delegates.FinishedDelegate RunFinshed;
		public event Delegates.FailedDelegate RunFailed;
        public event Delegates.StartedDelegate RunStarted;

        private readonly IODataAvailablePackagesService _availablePackagesService;
        private IList<Package> _availablePackageCache;
        private DateTime _invalidateCacheTime;
        private readonly ISourceService _sourceService;

        public CachedAvailablePackagesService(IODataAvailablePackagesService availablePackagesService, ISourceService sourceService)
        {
            _availablePackagesService = availablePackagesService;
            _sourceService = sourceService;
            _availablePackagesService.RunFailed += AvailablePackagesServiceRunFailed;
            _availablePackagesService.RunFinshed += OnUncachedAvailableRunFinished;
            _sourceService.CurrentSourceChanged += _sourceService_CurrentSourceChanged;
            InvalidateCache();
        }

        private void _sourceService_CurrentSourceChanged(Source source)
        {
            InvalidateCache();
            ListOfAvailablePackages();
        }

        public void InvalidateCache()
        {
            this.Log().Debug("Invalidate cache");
            _availablePackageCache = null;
            _invalidateCacheTime = DateTime.Now.AddMinutes(60);
        }

        public void ListOfAvailablePackages()
        {
            if (_availablePackageCache == null || DateTime.Now > _invalidateCacheTime)
            {
                OnRunStarted();
                this.Log().Debug("Get list of available packages from source");
                _availablePackagesService.ListOfAvailablePackages();
            }
            else
            {
                this.Log().Debug("Return list of packages from cache.");
                OnRunFinshed(_availablePackageCache);
            }
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            this.Log().Debug("Run Finished");
            var handler = RunFinshed;
            if (handler != null) handler(packages);
        }

        /// <summary>
        /// Called when the package service that should be cached finishes
        /// it's run for packages currently available on the server.
        /// </summary>
        /// <param name="packages"></param>
        public void OnUncachedAvailableRunFinished(IList<Package> packages)
        {
            this.Log().Debug("UnCached run finishes.");
            _availablePackageCache = packages;
            OnRunFinshed(packages);
        }

        private void AvailablePackagesServiceRunFailed(System.Exception exc)
		{
            this.Log().Debug("Run Failed");
            if (RunFailed != null)
				RunFailed(exc);
		}

        private void OnRunStarted()
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler("Getting list of available packages.");
        }
    }
}
