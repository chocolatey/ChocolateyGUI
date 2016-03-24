// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using ChocolateyGui.Enums;
    using ChocolateyGui.Models;
    using ChocolateyGui.Providers;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.ViewModels.Items;
    using Newtonsoft.Json;
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
        private ILogService logService;
        private IProgressService progressService;
        private IChocolateyConfigurationProvider chocolateyConfigurationProvider;

        protected BasePackageService(IProgressService progressService, Func<Type, ILogService> logServiceFunc, IChocolateyConfigurationProvider chocolateyConfigurationProvider)
        {
            if (logServiceFunc == null)
            {
                throw new ArgumentNullException("logServiceFunc");
            }

            this.GetInstalledLock = new AsyncLock();
            this.progressService = progressService;
            this.logService = logServiceFunc(typeof(IChocolateyPackageService));
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
                return this.logService;
            }
        }

        /// <summary>
        /// Gets the Progress Service used to report progress to listeners.
        /// </summary>
        public IProgressService ProgressService
        {
            get
            {
                return this.progressService;
            }
        }

        public IChocolateyConfigurationProvider ChocolateyConfigurationProvider
        {
            get
            {
                return this.chocolateyConfigurationProvider;
            }
        }

        public void ClearPackageCache()
        {
            Cache.Remove(LocalPackagesCacheKeyName);
        }

        public void NotifyPackagesChanged(PackagesChangedEventType command, string packageId = "", string packageVersion = "")
        {
            var packagesUpdated = this.PackagesUpdated;
            if (packagesUpdated != null)
            {
                packagesUpdated(
                    this,
                    new PackagesChangedEventArgs
                    {
                        EventType = command,
                        PackageId = packageId,
                        PackageVersion = packageVersion
                    });
            }
        }

        public void PopulatePackages(IPackageViewModel packageInfo, ICollection<IPackageViewModel> packages)
        {
            if (packages == null)
            {
                throw new ArgumentNullException("packages");
            }

            packages.Add(packageInfo);
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

        public async Task UpdatedPackage(string id, SemanticVersion oldVersion, SemanticVersion newVersion)
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