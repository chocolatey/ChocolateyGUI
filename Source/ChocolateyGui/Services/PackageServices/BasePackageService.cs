// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Providers;
using ChocolateyGui.Utilities;
using ChocolateyGui.ViewModels.Items;
using NuGet;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace ChocolateyGui.Services.PackageServices
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

        private readonly IEventAggregator _eventAggregator;

        protected BasePackageService(IProgressService progressService,
            IChocolateyConfigurationProvider chocolateyConfigurationProvider,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            GetInstalledLock = new AsyncLock();
            ProgressService = progressService;
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

        /// <summary>
        ///     Gets the Progress Service used to report progress to listeners.
        /// </summary>
        protected IProgressService ProgressService { get; }

        protected IChocolateyConfigurationProvider ChocolateyConfigurationProvider { get; }

        public static void ClearPackageCache()
        {
            Cache.Remove(LocalPackagesCacheKeyName);
        }

        public void InstalledPackage(string id, SemanticVersion version)
        {
            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Installed, version));
        }

        public void UninstalledPackage(string id, SemanticVersion version)
        {
            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Uninstalled, version));
        }

        public void UpdatedPackage(string id)
        {
            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Updated));
        }

        public async Task<IDisposable> StartProgressDialog(string commandString, string initialProgressText, string id = "")
        {
            await ProgressService.StartLoading($"{commandString} {id}...");
            ProgressService.WriteMessage(initialProgressText);
            return new DisposableAction(() => ProgressService.StopLoading());
        }
    }
}