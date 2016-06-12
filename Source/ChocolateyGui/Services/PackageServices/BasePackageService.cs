// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

namespace ChocolateyGui.Services
{
    public abstract class BasePackageService
    {
        /// <summary>
        ///     The key in the <see cref="Cache">Service's Memory Cache</see> for this service's packages./>
        /// </summary>
        public const string LocalPackagesCacheKeyName = "LocalChocolateyService.Packages";

        /// <summary>
        ///     Synchronizes the GetPackages method.
        /// </summary>
        internal readonly AsyncLock GetInstalledLock;

        protected BasePackageService(IProgressService progressService, Func<string, ILogService> logFactory,
            IChocolateyConfigurationProvider chocolateyConfigurationProvider)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException("logFactory");
            }

            GetInstalledLock = new AsyncLock();
            ProgressService = progressService;
            LogFactory = logFactory;
            LogService = logFactory(typeof(IChocolateyPackageService).Name);
            ChocolateyConfigurationProvider = chocolateyConfigurationProvider;
        }

        /// <summary>
        ///     Gets or sets the Cache for this service where out installed packages list is stored.
        /// </summary>
        public static MemoryCache Cache { get; set; } = MemoryCache.Default;

        public static ICollection<IPackageViewModel> CachedPackages
        {
            get { return (ICollection<IPackageViewModel>) Cache.Get(LocalPackagesCacheKeyName); }

            protected set { Cache.Set(LocalPackagesCacheKeyName, value, DateTimeOffset.Now + TimeSpan.FromMinutes(5)); }
        }

        public ILogService LogService { get; }

        /// <summary>
        ///     Gets the Progress Service used to report progress to listeners.
        /// </summary>
        protected IProgressService ProgressService { get; }

        protected IChocolateyConfigurationProvider ChocolateyConfigurationProvider { get; }

        protected Func<string, ILogService> LogFactory { get; }

        public event PackagesChangedEventHandler PackagesUpdated;

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

        public void NotifyPackagesChanged(PackagesChangedEventType command, string packageId = "",
            string packageVersion = "")
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
            NotifyPackagesChanged(PackagesChangedEventType.Installed, id,
                version == null ? string.Empty : version.ToString());
            await ProgressService.StopLoading();
        }

        public async Task UninstalledPackage(string id, SemanticVersion version)
        {
            ClearPackageCache();
            NotifyPackagesChanged(PackagesChangedEventType.Uninstalled, id,
                version == null ? string.Empty : version.ToString());
            await ProgressService.StopLoading();
        }

        public async Task UpdatedPackage(string id)
        {
            ClearPackageCache();
            NotifyPackagesChanged(PackagesChangedEventType.Updated, id);
            await ProgressService.StopLoading();
        }

        public async void StartProgressDialog(string commandString, string initialProgressText, string id = "")
        {
            await ProgressService.StartLoading(string.Format("{0} {1}...", commandString, id));
            ProgressService.WriteMessage(initialProgressText);
        }
    }
}