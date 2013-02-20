using System.Collections.Generic;
using Chocolatey.Explorer.Model;

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

        private readonly IAvailablePackagesService _availablePackagesService;
        private IList<Package> _availablePackageCache;

        public CachedAvailablePackagesService(ODataAvailablePackagesService availablePackagesService)
        {
            _availablePackagesService = availablePackagesService;
			_availablePackagesService.RunFailed += AvailablePackagesServiceRunFailed;
        }

        public void InvalidateCache()
        {
            InvalidateAvailablePackagesCache();
        }

        public void InvalidateAvailablePackagesCache()
        {
            _availablePackageCache = null;
        }

        public void ListOfAvalablePackages()
        {
            if (_availablePackageCache == null)
            {
                OnRunStarted();
                _availablePackagesService.RunFinshed += OnUncachedAvailableRunFinished;
                _availablePackagesService.ListOfAvalablePackages();
            }
            else
            {
                OnRunFinshed(_availablePackageCache);
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
        public void OnUncachedAvailableRunFinished(IList<Package> packages)
        {
            _availablePackageCache = packages;
            _availablePackagesService.RunFinshed -= OnUncachedAvailableRunFinished;
            OnRunFinshed(packages);
        }

        private void AvailablePackagesServiceRunFailed(System.Exception exc)
		{
			if (RunFailed != null)
				RunFailed(exc);
		}

        private void OnRunStarted()
        {
            var handler = RunStarted;
            if (handler != null) handler("Getting list of available packages.");
        }
    }
}
