// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BasePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using ChocolateyGui.Enums;
    using ChocolateyGui.Models;
    using ChocolateyGui.Providers;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.Utilities.Nuspec;
    using ChocolateyGui.ViewModels.Items;

    using Newtonsoft.Json;

    public abstract class BasePackageService
    {
        /// <summary>
        /// The key in the <see cref="Cache">Service's Memory Cache</see> for this service's packages./>
        /// </summary>
        public const string LocalPackagesCacheKeyName = "LocalChocolateyService.Packages";

        /// <summary>
        /// The key in the <see cref="Cache">Service's Memory Cache</see> for this service's packages JSON./>
        /// </summary>
        public const string LocalPackagesJsonCacheKeyName = "LocalChocolateyService.PackagesJson";

        /// <summary>
        /// Cache for this service where out installed packages list is stored.
        /// </summary>
        public static readonly MemoryCache Cache = MemoryCache.Default;

        public static readonly Regex PackageRegex = new Regex(@"^(?<Name>[\w\.]*) (?<VersionString>(\d+(\s*\.\s*\d+){0,3})(-[a-z][0-9a-z-]*)?)$");

        /// <summary>
        /// The path of the packages description JSON file.
        /// </summary>
        public readonly string PackagesJsonPath;

        /// <summary>
        /// Logs things.
        /// </summary>
        public readonly ILogService LogService;

        /// <summary>
        /// Allows the Chocolatey Service to report progress to listeners.
        /// </summary>
        public readonly IProgressService ProgressService;

        public readonly IChocolateyConfigurationProvider ChocolateyConfigurationProvider;

        /// <summary>
        /// Synchronizes the GetPackages method.
        /// </summary>
        internal readonly AsyncLock GetInstalledLock;

        protected BasePackageService(IProgressService progressService, Func<Type, ILogService> logServiceFunc, IChocolateyConfigurationProvider chocolateyConfigurationProvider)
        {
            if (logServiceFunc == null)
            {
                throw new ArgumentNullException("logServiceFunc");
            }

            this.GetInstalledLock = new AsyncLock();
            this.ProgressService = progressService;
            this.LogService = logServiceFunc(typeof(IChocolateyPackageService));
            this.ChocolateyConfigurationProvider = chocolateyConfigurationProvider;

            this.PackagesJsonPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ChocolateyGUI",
                "packages.json");
        }

        public event PackagesChangedEventHandler PackagesUpdated;

        /// <summary>
        /// Placeholder. Will eventually allow one to call Chocolatey and list all the packages from a specified file system source.
        /// </summary>
        /// <param name="requestedPackages">
        /// The requested Packages.
        /// </param>
        /// <param name="directoryPath">
        /// The file system directory.
        /// </param>
        /// <returns>
        /// List of packages in directory.
        /// </returns>
        public async Task<IEnumerable<IPackageViewModel>> GetPackagesFromLocalDirectory(Dictionary<string, string> requestedPackages, string directoryPath)
        {
            var packages = new List<IPackageViewModel>();

            foreach (var nupkgFile in Directory.EnumerateFiles(directoryPath, "*.nupkg", SearchOption.AllDirectories))
            {
                var packageInfo = await NupkgReader.GetPackageInformation(nupkgFile);

                if (
                    !requestedPackages.Any(
                        e =>
                        string.Equals(e.Key, packageInfo.Id, StringComparison.CurrentCultureIgnoreCase)
                        && new SemanticVersion(e.Value) == packageInfo.Version))
                {
                    continue;
                }

                this.PopulatePackages(packageInfo, packages);
            }

            return packages;
        }

        public bool IsPackageInstalled(string id, SemanticVersion version)
        {
            if (Cache.Contains(LocalPackagesCacheKeyName))
            {
                return ((List<IPackageViewModel>)Cache.Get(LocalPackagesCacheKeyName))
                    .Any(package => string.Compare(package.Id, id, StringComparison.OrdinalIgnoreCase) == 0 && package.Version == version);
            }

            return false;
        }

        #region Packages Json Methods
        public void AddPackageEntry(string id, SemanticVersion version, Uri source)
        {
            // Grab the current packages.
            var packages = this.PackageConfigEntries();

            // Check if we already exist
            if (packages.Any(p => p.Id == id && p.Version == version))
            {
                this.LogService.ErrorFormat(
                    "Package with id {0} and version {1} is already in the packages.json.\n From source {2}",
                    id,
                    version,
                    source);

                var oldPackage = packages.First(p => p.Id == id && p.Version == version);

                if (oldPackage.Source != source)
                {
                    packages.Remove(oldPackage);
                    packages.Add(new PackageConfigEntry(id, version, source));
                }
            }
            else
            {
                packages.Add(new PackageConfigEntry(id, version, source));
            }

            this.SerializeJsonCache(packages);
        }

        public List<PackageConfigEntry> PackageConfigEntries()
        {
            // Check to see if we already have a cached version of the packages.json.
            var configEntries = Cache.Get(LocalPackagesJsonCacheKeyName) as List<PackageConfigEntry>;
            if (configEntries != null)
            {
                return configEntries;
            }

            // If there is no packages.json, just pass back an empty array.
            if (!File.Exists(this.PackagesJsonPath))
            {
                return new List<PackageConfigEntry>();
            }

            // If there is, deserialize and cache it.
            var packageJson = File.ReadAllText(this.PackagesJsonPath);
            var packages = JsonConvert.DeserializeObject<List<PackageConfigEntry>>(packageJson);
            Cache.Set(LocalPackagesJsonCacheKeyName, packages, DateTime.Now.AddHours(1));

            return packages;
        }

        public void RemovePackageEntry(string id, SemanticVersion version)
        {
            // Grab the current packages.
            var packages = this.PackageConfigEntries();

            // Remove all matching entries.
            packages.RemoveAll(pce =>
                string.Compare(pce.Id, id, StringComparison.OrdinalIgnoreCase) == 0 && pce.Version == version);

            this.SerializeJsonCache(packages);
        }
        #endregion

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

        public void PopulatePackages(IPackageViewModel packageInfo, List<IPackageViewModel> packages)
        {
            if (packages == null)
            {
                throw new ArgumentNullException("packages");
            }

            var packageConfigEntry =
                this.PackageConfigEntries()
                    .SingleOrDefault(
                        entry =>
                        string.Compare(entry.Id, packageInfo.Id, StringComparison.OrdinalIgnoreCase) == 0
                        && entry.Version == packageInfo.Version);

            if (packageConfigEntry != null)
            {
                packageInfo.Source = packageConfigEntry.Source;
            }

            packages.Add(packageInfo);
        }

        public void SerializeJsonCache(List<PackageConfigEntry> packages)
        {
            // Serialize to the appropriate format.
            var packageJson = JsonConvert.SerializeObject(packages);

            // Make sure we have a ChocolateyGUI folder in the LocalApplicationData folder.
            var directory = new DirectoryInfo(Path.GetDirectoryName(this.PackagesJsonPath));
            if (!directory.Exists)
            {
                directory.Create();
            }

            // Write the new package file.
            File.WriteAllText(this.PackagesJsonPath, packageJson);

            // Invalidate the package cache.
            Cache.Remove(LocalPackagesJsonCacheKeyName);

            // Throw in this comment for fun.
        }
    }
}