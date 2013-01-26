using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocolatey.Explorer.Model;
using log4net;

namespace Chocolatey.Explorer.Services
{
    /// <summary>
    /// Adapter for PackageVersionService, but caching all already
    /// downloaded PackageVersions internally.
    /// </summary>
    class CachedPackageVersionService : IPackageVersionService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PackageVersionService));

        public delegate void VersionResult(PackageVersion version);
        public event PackageVersionService.VersionResult VersionChanged;

        private IPackageVersionService packageVersionService;
        private IDictionary<string, PackageVersion> cachedVersions;

        public CachedPackageVersionService()
        {
            packageVersionService = new PackageVersionService();
            packageVersionService.VersionChanged += OnUncachedVersionChanged;
            InvalidateCache();
        }

        public void InvalidateCache()
        {
            cachedVersions = new Dictionary<string, PackageVersion>();
        }

        public void PackageVersion(string packageName)
        {
            PackageVersion cachedPackage;
            cachedVersions.TryGetValue(packageName, out cachedPackage);

            if (cachedPackage == null)
            {
                packageVersionService.PackageVersion(packageName);
            }
            else
            {
                OnVersionChanged(cachedPackage);
            }
        }

        /// <summary>
        /// Called when the PackageVersionService that should be
        /// cached is returning a new PackageVersion.
        /// </summary>
        /// <param name="version"></param>
        private void OnUncachedVersionChanged(PackageVersion version)
        {
            cachedVersions.Add(version.Name, version);
            OnVersionChanged(version);
        }

        private void OnVersionChanged(PackageVersion version)
        {
            var handler = VersionChanged;
            if (handler != null) handler(version);
        }
    }
}
