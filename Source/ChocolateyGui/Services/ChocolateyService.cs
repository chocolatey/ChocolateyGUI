// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Runtime.Caching;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using ChocolateyGui.Controls;
    using ChocolateyGui.Enums;
    using ChocolateyGui.Models;
    using ChocolateyGui.Properties;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.Utilities.Nuspec;
    using ChocolateyGui.ViewModels.Items;
    using Newtonsoft.Json;

    public class ChocolateyService : IChocolateyService
    {
        /// <summary>
        /// The key in the <see cref="Cache">Service's Memory Cache</see> for this service's packages./>
        /// </summary>
        private const string LocalPackagesCacheKeyName = "LocalChocolateyService.Packages";

        /// <summary>
        /// The key in the <see cref="Cache">Service's Memory Cache</see> for this service's packages JSON./>
        /// </summary>
        private const string LocalPackagesJsonCacheKeyName = "LocalChocolateyService.PackagesJson";

        /// <summary>
        /// Cache for this service where out installed packages list is stored.
        /// </summary>
        private static readonly MemoryCache Cache = MemoryCache.Default;

        private static readonly Regex PackageRegex = new Regex(@"^(?<Name>[\w\.]*) (?<VersionString>(\d+(\s*\.\s*\d+){0,3})(-[a-z][0-9a-z-]*)?)$");

        /// <summary>
        /// Synchronizes the GetPackages method.
        /// </summary>
        private readonly AsyncLock _getInstalledLock;

        /// <summary>
        /// Logs things.
        /// </summary>
        private readonly ILogService _logService;

        /// <summary>
        /// The path of the packages description JSON file.
        /// </summary>
        private readonly string _packagesJsonPath;

        /// <summary>
        /// Allows the Chocolatey Service to report progress to listeners.
        /// </summary>
        private readonly IProgressService _progressService;

        /// <summary>
        /// The PowerShell runspace for this service.
        /// </summary>
        private readonly Runspace _runspace;

        public ChocolateyService(IProgressService progressService, Func<Type, ILogService> logServiceFunc)
        {
            if (logServiceFunc == null)
            {
                throw new ArgumentNullException("logServiceFunc");
            }

            this._runspace = RunspaceFactory.CreateRunspace(new ChocolateyHost(progressService));
            this._runspace.Open();

            this._getInstalledLock = new AsyncLock();
            this._progressService = progressService;
            this._logService = logServiceFunc(typeof(ChocolateyService));

            this._packagesJsonPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ChocolateyGUI", 
                "packages.json");
        }

        public event PackagesChangedEventHandler PackagesUpdated;

        /// <summary>
        /// Retrieves the currently installed packages.
        /// If the package list is cached, retrieve it from there.
        /// Else, scan the file system for packages and pull the appropriate information from there.
        /// </summary>
        /// <param name="force">Forces a cache reset.</param>
        /// <returns>List of currently installed packages.</returns>
        public async Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false)
        {
            // Ensure that we only retrieve the packages one at a to refresh the Cache.
            using (await this._getInstalledLock.LockAsync())
            {
                List<IPackageViewModel> packages;
                if (!force)
                {
                    packages = (List<IPackageViewModel>)Cache.Get(LocalPackagesCacheKeyName);
                    if (packages != null)
                    {
                        return packages;
                    }
                }

                await this._progressService.StartLoading("Chocolatey Service");
                this._progressService.WriteMessage("Retrieving installed packages...");

                var chocoPath = Settings.Default.chocolateyInstall;
                if (string.IsNullOrWhiteSpace(chocoPath) || !Directory.Exists(chocoPath))
                {
                    throw new InvalidDataException(
                        "Invalid Chocolatey Path. Check that chocolateyInstall is correct in the app.config.");
                }

                var libPath = Path.Combine(chocoPath, "lib");

                var chocoPackageList = (await this.RunIndirectChocolateyCommand("list -lo", false))
                    .Where(p => PackageRegex.IsMatch(p.ToString()))
                    .Select(p => PackageRegex.Match(p.ToString()))
                    .ToDictionary(m => m.Groups["Name"].Value, m => new SemanticVersion(m.Groups["VersionString"].Value));

                packages = new List<IPackageViewModel>();
                foreach (var nupkgFile in Directory.EnumerateFiles(libPath, "*.nupkg", SearchOption.AllDirectories))
                {
                    var packageInfo = await NupkgReader.GetPackageInformation(nupkgFile);

                    if (
                        !chocoPackageList.Any(
                            e =>
                            string.Equals(e.Key, packageInfo.Id, StringComparison.CurrentCultureIgnoreCase)
                            && e.Value == packageInfo.Version))
                    {
                        continue;
                    }

                    this.PopulatePackages(packageInfo, packages);
                }

                Cache.Set(
                    LocalPackagesCacheKeyName,
                    packages,
                    new CacheItemPolicy
                        {
                            AbsoluteExpiration = DateTime.Now.AddHours(1)
                });

                await this._progressService.StopLoading();
                return packages;
            }
        }

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

        #region Package Commands
        public async Task InstallPackage(string id, SemanticVersion version = null, Uri source = null)
        {
            await this._progressService.StartLoading(string.Format("Installing {0}...", id));
            this._progressService.WriteMessage("Building chocolatey command...");
            var arguments = new Dictionary<string, object> { { "command", "install" }, { "packageNames", id } };

            if (version != null)
            {
                arguments.Add("version", version.ToString());
            }

            if (source != null)
            {
                arguments.Add("source", source.ToString());
            }

            await this.ExecutePackageCommand(arguments);

            var newPackage =
                (await this.GetInstalledPackages()).OrderByDescending(p => p.Version)
                    .FirstOrDefault(
                        p =>
                        string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0
                        && (version == null || version == p.Version));

            if (newPackage != null)
            {
                this.AddPackageEntry(newPackage.Id, newPackage.Version, source);
            }

            this.NotifyPackagesChanged(PackagesChangedEventType.Installed, id, version == null ? string.Empty : version.ToString());
            await this._progressService.StopLoading();
        }

        public async Task ReinstallPackage(string id, SemanticVersion version = null, Uri source = null)
        {
            await this._progressService.StartLoading(string.Format("Reinstalling {0}...", id));
            this._progressService.WriteMessage("Building chocolatey command...");
            var arguments = new Dictionary<string, object> { { "command", "install" }, { "packageNames", id } };

            if (version != null)
            {
                arguments.Add("version", version.ToString());
            }

            if (source != null)
            {
                arguments.Add("source", source.ToString());
            }

            arguments.Add("force", true);

            await this.ExecutePackageCommand(arguments);

            var newPackage =
                (await this.GetInstalledPackages()).OrderByDescending(p => p.Version)
                    .FirstOrDefault(
                        p =>
                        string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0
                        && (version == null || version == p.Version));

            if (newPackage != null)
            {
                this.AddPackageEntry(newPackage.Id, newPackage.Version, source);
            }

            this.NotifyPackagesChanged(PackagesChangedEventType.Installed, id, version == null ? string.Empty : version.ToString());
            await this._progressService.StopLoading();
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

        public async Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            await this._progressService.StartLoading(string.Format("Uninstalling {0}...", id));
            this._progressService.WriteMessage("Building chocolatey command...");

            var arguments = new Dictionary<string, object>
                                {
                                    { "command", "uninstall" },
                                    { "version", version.ToString() },
                                    { "packageNames", id }
                                };

            await this.ExecutePackageCommand(arguments);

            this.RemovePackageEntry(id, version);
            this.NotifyPackagesChanged(PackagesChangedEventType.Uninstalled, id, version.ToString());
            await this._progressService.StopLoading();
        }

        public async Task UpdatePackage(string id, Uri source = null)
        {
            await this._progressService.StartLoading(string.Format("Updating {0}...", id));
            this._progressService.WriteMessage("Building chocolatey command...");
            var currentPackages = this.PackageConfigEntries().Where(p => string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0).ToList();

            var arguments = new Dictionary<string, object> { { "command", "update" }, { "packageNames", id } };

            await this.ExecutePackageCommand(arguments);

            var newPackages = (await this.GetInstalledPackages()).Where(p => string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0).ToList();

            var results = newPackages
                .FullOuterJoin(currentPackages, p => p.Version, cp => cp.Version, (p, cp, version) => new { Id = p != null ? p.Id ?? cp.Id : cp.Id, Version = version });

            foreach (var result in results)
            {
                if (currentPackages.Any(p => string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0 && p.Version == result.Version) && !newPackages.Any(p => p.Id == result.Id && p.Version == result.Version))
                {
                    this.RemovePackageEntry(result.Id, result.Version);
                }
                else
                {
                    this.AddPackageEntry(result.Id, result.Version, source);
                }
            }

            this.NotifyPackagesChanged(PackagesChangedEventType.Updated, id);
            await this._progressService.StopLoading();
        }

        public void ClearPackageCache()
        {
            Cache.Remove(LocalPackagesCacheKeyName);
        }

        #endregion

        #region Chocolatey Interop Methods
        /// <summary>
        /// Executes a PowerShell command and returns whether or not there was a result. Optionally calls <see cref="GetInstalledPackages"/>.
        /// </summary>
        /// <param name="commandArgs">The chocolatey command arguments.</param>
        /// <param name="refreshPackages">Whether to force <see cref="GetInstalledPackages"/>.</param>
        /// <returns>Whether or not a result was returned from <see cref="RunDirectChocolateyCommand"/>.</returns>
        public async Task<bool> ExecutePackageCommand(Dictionary<string, object> commandArgs, bool refreshPackages = true)
        {
            try
            {
                await this.RunDirectChocolateyCommand(commandArgs, refreshPackages);
                return true;
            }
            catch (Exception ex)
            {
                this._logService.Error("ExecutePackageCommmand threw an exception.", ex);
                return false;
            }
        }

        /// <summary>
        /// Executes a PowerShell Command by directly calling chocolatey.ps1. 
        /// </summary>
        /// <param name="commandArgs">
        /// The Chocolatey command arguments.
        /// </param>
        /// <param name="refreshPackages">
        /// Whether to force <see cref="GetInstalledPackages"/>.
        /// </param>
        /// <param name="logOutput">
        /// Whether the output should be logged to the faux PowerShell console or returned as results.
        /// </param>
        /// <returns>
        /// A collection of the output of the PowerShell runspace. Will be empty if <paramref cref="logOutput"/> is true.
        /// </returns>
        public async Task RunDirectChocolateyCommand(Dictionary<string, object> commandArgs, bool refreshPackages = true, bool logOutput = true)
        {
            await this._progressService.StartLoading("Chocolatey");
            this._progressService.WriteMessage("Processing chocolatey command...");

            var pipeline = this._runspace.CreatePipeline();

            var chocoPath = Path.Combine(Settings.Default.chocolateyInstall, "chocolateyinstall", "chocolatey.ps1");

            var powerShellCommand = new Command(chocoPath);

            foreach (var commandArg in commandArgs)
            {
                powerShellCommand.Parameters.Add(commandArg.Key, commandArg.Value);
            }

            pipeline.Commands.Add(powerShellCommand);

            try
            {
                await Task.Run(() => pipeline.Invoke());
            }
            catch (Exception e)
            {
                this._progressService.WriteMessage(e.ToString(), PowerShellLineType.Error);
                this._progressService.StopLoading().ConfigureAwait(false);
                throw;
            }

            if (logOutput)
            {
                this._progressService.WriteMessage("Executed successfully.");
            }

            if (refreshPackages)
            {
                await this.GetInstalledPackages(force: true);
            }

            await this._progressService.StopLoading();
        }

        /// <summary>
        /// Executes a PowerShell Command by calling Chocolatey through the PowerShell command line. 
        /// </summary>
        /// <param name="command">
        /// The Chocolatey command arguments.
        /// </param>
        /// <param name="refreshPackages">
        /// Whether to force <see cref="GetInstalledPackages"/>.
        /// </param>
        /// <param name="logOutput">
        /// Whether the output should be logged to the faux PowerShell console or returned as results.
        /// </param>
        /// <returns>
        /// A collection of the output of the PowerShell runspace. Will be empty if <paramref cref="logOutput"/> is true.
        /// </returns>
        public async Task<Collection<PSObject>> RunIndirectChocolateyCommand(string command, bool refreshPackages = true, bool logOutput = true)
        {
            await this._progressService.StartLoading("Chocolatey");
            this._progressService.WriteMessage("Processing chocolatey command...");

            var pipeline = this._runspace.CreatePipeline();

            pipeline.Commands.AddScript("chocolatey " + command);
            Collection<PSObject> results;

            try
            {
                results = await Task.Run(() => pipeline.Invoke());
            }
            catch (Exception e)
            {
                this._progressService.WriteMessage(e.ToString(), PowerShellLineType.Error);
                this._progressService.StopLoading().ConfigureAwait(false);
                throw;
            }

            if (logOutput)
            {
                this._progressService.WriteMessage("Executed successfully.");
            }

            if (refreshPackages)
            {
                await this.GetInstalledPackages(force: true);
            }

            await this._progressService.StopLoading();
            return results;
        }
        #endregion

        #region Packages Json Methods
        private void AddPackageEntry(string id, SemanticVersion version, Uri source)
        {
            // Grab the current packages.
            var packages = this.PackageConfigEntries();

            // Check if we already exist
            if (packages.Any(p => p.Id == id && p.Version == version))
            {
                this._logService.ErrorFormat(
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

        private List<PackageConfigEntry> PackageConfigEntries()
        {
            // Check to see if we already have a cached version of the packages.json.
            var configEntries = Cache.Get(LocalPackagesJsonCacheKeyName) as List<PackageConfigEntry>;
            if (configEntries != null)
            {
                return configEntries;
            }

            // If there is no packages.json, just pass back an empty array.
            if (!File.Exists(this._packagesJsonPath))
            {
                return new List<PackageConfigEntry>();
            }

            // If there is, deserialize and cache it.
            var packageJson = File.ReadAllText(this._packagesJsonPath);
            var packages = JsonConvert.DeserializeObject<List<PackageConfigEntry>>(packageJson);
            Cache.Set(LocalPackagesJsonCacheKeyName, packages, DateTime.Now.AddHours(1));
            return packages;
        }

        private void RemovePackageEntry(string id, SemanticVersion version)
        {
            // Grab the current packages.
            var packages = this.PackageConfigEntries();

            // Remove all matching entries.
            packages.RemoveAll(pce =>
                string.Compare(pce.Id, id, StringComparison.OrdinalIgnoreCase) == 0 && pce.Version == version);

            this.SerializeJsonCache(packages);
        }
        #endregion

        private void NotifyPackagesChanged(PackagesChangedEventType command, string packageId = "", string packageVersion = "")
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

        private void PopulatePackages(IPackageViewModel packageInfo, List<IPackageViewModel> packages)
        {
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

        private void SerializeJsonCache(List<PackageConfigEntry> packages)
        {
            // Serialize to the appropriate format.
            var packageJson = JsonConvert.SerializeObject(packages);

            // Make sure we have a ChocolateyGUI folder in the LocalApplicationData folder.
            var directory = new DirectoryInfo(Path.GetDirectoryName(this._packagesJsonPath));
            if (!directory.Exists)
            {
                directory.Create();
            }

            // Write the new package file.
            File.WriteAllText(this._packagesJsonPath, packageJson);

            // Invalidate the package cache.
            Cache.Remove(LocalPackagesJsonCacheKeyName);

            // Throw in this comment for fun.
        }
    }
}