using System;
using System.Collections.Generic;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services.PackageVersionService
{
    /// <summary>
    /// Adapter for PackageVersionService, but caching all already
    /// downloaded PackageVersions internally.
    /// </summary>
    class CachedPackageVersionService : IPackageVersionService, ICacheable
    {

        public event Delegates.VersionResult VersionChanged;
        public event Delegates.StartedDelegate RunStarted;

        private readonly IODataPackageVersionService _packageVersionService;
        private IDictionary<string, VersionAndCacheTime> _cachedVersions;

        public CachedPackageVersionService(IODataPackageVersionService packageVersionService)
        {
            _packageVersionService = packageVersionService;
            _packageVersionService.VersionChanged += OnUncachedVersionChanged;
            InvalidateCache();
        }

        public void InvalidateCache()
        {
            this.Log().Debug("Invalidate cache");
            _cachedVersions = new Dictionary<string, VersionAndCacheTime>();
        }

        public void PackageVersion(string packageName)
        {
            this.Log().Debug("Get packageVersion for packagename {0}", packageName);
            VersionAndCacheTime cachedPackage;
            _cachedVersions.TryGetValue(packageName, out cachedPackage);
            OnStarted(packageName);

            if (cachedPackage == null || DateTime.Now > cachedPackage.InvalidateCacheTime)
            {
                this.Log().Debug("Get pacakge from service");
                _packageVersionService.PackageVersion(packageName);
            }
            else
            {
                this.Log().Debug("Get package from cache");
                OnVersionChanged(cachedPackage.Version);
            }
        }

        /// <summary>
        /// Called when the PackageVersionService that should be
        /// cached is returning a new PackageVersion.
        /// </summary>
        /// <param name="version"></param>
        private void OnUncachedVersionChanged(PackageVersion version)
        {
            this.Log().Debug("Run finished on uncached version");
            if (_cachedVersions.ContainsKey(version.Name))
            {
                _cachedVersions[version.Name] = new VersionAndCacheTime { Version = version, InvalidateCacheTime = DateTime.Now.AddMinutes(30) };
            }
            else
            {
                _cachedVersions.Add(version.Name, new VersionAndCacheTime { Version = version, InvalidateCacheTime = DateTime.Now.AddMinutes(30) });
            }
            OnVersionChanged(version);
        }

        private void OnVersionChanged(PackageVersion version)
        {
            this.Log().Debug("New packageversion {0}", version);
            var handler = VersionChanged;
            if (handler != null) handler(version);
        }

        private void OnStarted(string packageName)
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler("Getting package " + packageName);
        }

        private class VersionAndCacheTime
        {
            public PackageVersion Version { get; set; }
            public DateTime InvalidateCacheTime { get; set; }
        }
    }
}
