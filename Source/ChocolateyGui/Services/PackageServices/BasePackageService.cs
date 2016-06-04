// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ChocolateyGui.Enums;
    using ChocolateyGui.Models;
    using ChocolateyGui.Providers;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.ViewModels.Items;
    using NuGet;
    using MemoryCache = System.Runtime.Caching.MemoryCache;

    public abstract class BasePackageService
    {
        /// <summary>
        /// The key in the <see cref="Cache">Service's Memory Cache</see> for this service's packages./>
        /// </summary>
        public const string LocalPackagesCacheKeyName = "LocalChocolateyService.Packages";

        /// <summary>
        /// Synchronizes the GetPackages method.
        /// </summary>
        internal readonly AsyncLock GetInstalledLock;

        private static MemoryCache cache = MemoryCache.Default;
        private readonly Func<string, ILogService> _logFactory;
        private readonly ILogService _logService;
        private readonly IProgressService _progressService;
        private readonly IChocolateyConfigurationProvider chocolateyConfigurationProvider;

        protected BasePackageService(IProgressService progressService, Func<string, ILogService> logFactory, IChocolateyConfigurationProvider chocolateyConfigurationProvider)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException("logFactory");
            }

            this.GetInstalledLock = new AsyncLock();
            this._progressService = progressService;
            this._logFactory = logFactory;
            this._logService = logFactory(typeof(IChocolateyPackageService).Name);
            this.chocolateyConfigurationProvider = chocolateyConfigurationProvider;
        }

        public event PackagesChangedEventHandler PackagesUpdated;

        /// <summary>
        /// Gets or sets the Cache for this service where out installed packages list is stored.
        /// </summary>
        public static MemoryCache Cache
        {
            get
            {
                return cache;
            }

            set
            {
                cache = value;
            }
        }

        public static ICollection<IPackageViewModel> CachedPackages
        {
            get
            {
                return (ICollection<IPackageViewModel>)Cache.Get(LocalPackagesCacheKeyName);
            }

            protected set
            {
                Cache.Set(LocalPackagesCacheKeyName, value, DateTimeOffset.Now + TimeSpan.FromMinutes(5));
            }
        }

        public ILogService LogService
        {
            get
            {
                return this._logService;
            }
        }

        /// <summary>
        /// Gets the Progress Service used to report progress to listeners.
        /// </summary>
        protected IProgressService ProgressService
        {
            get
            {
                return this._progressService;
            }
        }

        protected IChocolateyConfigurationProvider ChocolateyConfigurationProvider
        {
            get
            {
                return this.chocolateyConfigurationProvider;
            }
        }

        protected Func<string, ILogService> LogFactory
        {
            get { return _logFactory; }
        }

        public static void ClearPackageCache()
        {
            Cache.Remove(LocalPackagesCacheKeyName);
        }

        public static void PopulatePackages(IPackageViewModel packageInfo, ICollection<IPackageViewModel> packages)
        {
            if (packages == null)
            {
                throw new ArgumentNullException("packages");
            }

            packages.Add(packageInfo);
        }

        public void NotifyPackagesChanged(PackagesChangedEventType command, string packageId = "", string packageVersion = "")
        {
            PackagesUpdated?.Invoke(
                this,
                new PackagesChangedEventArgs
                {
                    EventType = command,
                    PackageId = packageId,
                    PackageVersion = packageVersion
                });
        }

        public async Task InstalledPackage(string id, SemanticVersion version)
        {
            ClearPackageCache();
            this.NotifyPackagesChanged(PackagesChangedEventType.Installed, id, version == null ? string.Empty : version.ToString());
            await this.ProgressService.StopLoading();
        }

        public async Task UninstalledPackage(string id, SemanticVersion version)
        {
            ClearPackageCache();
            this.NotifyPackagesChanged(PackagesChangedEventType.Uninstalled, id, version == null ? string.Empty : version.ToString());
            await this.ProgressService.StopLoading();
        }

        public async Task UpdatedPackage(string id)
        {
            ClearPackageCache();
            this.NotifyPackagesChanged(PackagesChangedEventType.Updated, id);
            await this.ProgressService.StopLoading();
        }

        public async void StartProgressDialog(string commandString, string initialProgressText, string id = "")
        {
            await this.ProgressService.StartLoading(string.Format("{0} {1}...", commandString, id));
            this.ProgressService.WriteMessage(initialProgressText);
        }
    }
}