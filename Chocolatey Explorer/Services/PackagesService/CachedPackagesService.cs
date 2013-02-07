using System.Collections.Generic;
using Chocolatey.Explorer.Model;
using log4net;

namespace Chocolatey.Explorer.Services
{
    /// <summary>
    /// Adapter for PackagesService, but caching the Package-Lists
    /// internally.
    /// </summary>
    class CachedPackagesService : IPackagesService, ICacheable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CachedPackagesService));

        public delegate void FinishedDelegate(IList<Package> packages);
        public event PackagesService.FinishedDelegate RunFinshed;

        private readonly IPackagesService packagesService;
        private IList<Package> availablePackageCache;
        private IList<Package> installedPackageCache;

        public CachedPackagesService()
        {
            packagesService = new ODataPackagesService();
        }

        public void InvalidateCache()
        {
            InvalidateAvailablePackagesCache();
            InvalidateAvailablePackagesCache();
        }

        public void InvalidateAvailablePackagesCache()
        {
            availablePackageCache = null;
        }

        public void InvalidateInstalledPackagesCache()
        {
            installedPackageCache = null;
        }

        public void ListOfPackages()
        {
            if (availablePackageCache == null)
            {
                packagesService.RunFinshed += OnUncachedAvailableRunFinished;
                packagesService.ListOfPackages();
            }
            else
            {
                OnRunFinshed(availablePackageCache);
            }
        }

        public void ListOfInstalledPackages()
        {
            if (installedPackageCache == null)
            {
                packagesService.RunFinshed += OnUncachedInstalledRunFinished;
                packagesService.ListOfInstalledPackages();
            }
            else
            {
                OnRunFinshed(installedPackageCache);
            }
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            PackagesService.FinishedDelegate handler = RunFinshed;
            if (handler != null) handler(packages);
        }

        /// <summary>
        /// Called when the package service that should be cached finishes
        /// its run for currently packages currently available on the server.
        /// </summary>
        /// <param name="packages"></param>
        public void OnUncachedAvailableRunFinished(IList<Package> packages)
        {
            availablePackageCache = packages;
            packagesService.RunFinshed -= OnUncachedAvailableRunFinished;
            OnRunFinshed(packages);
        }

        /// <summary>
        /// Called when the package service that should be cached finishes
        /// its run for currently installed packages.
        /// </summary>
        /// <param name="packages"></param>
        public void OnUncachedInstalledRunFinished(IList<Package> packages)
        {
            installedPackageCache = packages;
            packagesService.RunFinshed -= OnUncachedInstalledRunFinished;
            OnRunFinshed(packages);
        }
    }
}
