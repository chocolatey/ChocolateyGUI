using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Utilities.Nuspec;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Services
{
    public class ChocolateyService : IChocolateyService
    {        
        /// <summary>
        /// The PowerShell runspace for this service.
        /// </summary>
        private readonly PowerShell _ps;

        /// <summary>
        /// Synchornizes the GetPackages method.
        /// </summary>
        private readonly SemaphoreSlim _ss;

        /// <summary>
        /// Cache for this servce where out installed packages list is stored.
        /// </summary>
        private readonly MemoryCache _cache = MemoryCache.Default;

        /// <summary>
        /// Allows the Chocolatey Service to report progress to listeners.
        /// </summary>
        private readonly IProgressService _progressService;

        /// <summary>
        /// The key in the <see cref="_cache">Service's Memory Cache</see> for this service's packages./>
        /// </summary>
        private const string LocalCacheKeyName = "LocalChocolateyService.Packages";
        public ChocolateyService(IProgressService progressService)
        {
            _ps = PowerShell.Create();
            _ss = new SemaphoreSlim(1);
            _progressService = progressService;
        }    
        public async Task<IEnumerable<IPackageViewModel>> GetPackages(bool force = false)
        {
            // Ensure that we only retrieve the packages one at a to refresh the Cache.
            _ss.Wait(200);

            List<IPackageViewModel> packages;
            if (!force)
            {
                packages = (List<IPackageViewModel>) _cache.Get(LocalCacheKeyName);
                if (packages != null)
                {
                    _ss.Release();
                    return packages;
                }
            }

            _progressService.StartLoading();

            var chocoPath = ChocoConfigurationSection.Current.ChocolateyInstall.Path;
            var libPath = Path.Combine(chocoPath, "lib");

            packages = new List<IPackageViewModel>();
            foreach (var nupkgFile in Directory.EnumerateFiles(libPath, "*.nupkg", SearchOption.AllDirectories))
            {
                 packages.Add(await NupkgReader.GetPackageInformation(nupkgFile));
            }

            _cache.Set(LocalCacheKeyName, packages, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });

            _progressService.StopLoading();
            _ss.Release();

            return packages;
        }

        public Task<IEnumerable<IPackageViewModel>> GetPackagesFromLocalDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        public async void InstallPackage(string id, SemanticVersion version = null, string source = null)
        {
            await ExecutePackageCommand("chocolatey install " + id + (version != null ? " -version " + version : ""));
            NotifyPackagesChanged(PackagesChangedEventType.Installed, id, version == null ? "" : version.ToString());
        }

        public async void UninstallPackage(string id, SemanticVersion version = null, bool force = false)
        {
            await ExecutePackageCommand("chocolatey uninstall " + id + " -version " + version);
            NotifyPackagesChanged(PackagesChangedEventType.Uninstalled, id, version == null ? "" : version.ToString());
        }

        public async void UpdatePackage(string id)
        {
            await ExecutePackageCommand("chocolatey update " + id);
            NotifyPackagesChanged(PackagesChangedEventType.Updated, id);
        }

        public bool IsPackageInstalled(string id, SemanticVersion version)
        {
            if (_cache.Contains(LocalCacheKeyName))
            {
                return ((List<IPackageViewModel>)_cache.Get(LocalCacheKeyName))
                    .Any(package => string.Compare(package.Id, id, StringComparison.InvariantCultureIgnoreCase) == 0 && package.Version == version);
            }
            return false;
        }

        /// <summary>
        /// Executes a PowerShell command and returns whether or not there was a result. Optionally calls <see cref="RefreshPackages"/>.
        /// </summary>
        /// <param name="commandString">The PowerShell command string.</param>
        /// <param name="refreshPackages">Whether to force <see cref="GetPackages"/>.</param>
        /// <returns>Whether or not a result was returned from <see cref="RunPackageCommand"/>.</returns>
        public async Task<bool> ExecutePackageCommand(string commandString, bool refreshPackages = true)
        {
            return await RunPackageCommand(commandString, refreshPackages) != null;
        }

        /// <summary>
        /// Executes a PowerShell Command. 
        /// </summary>
        /// <param name="commandString">The PowerShell command string.</param>
        /// <param name="refreshPackages">Whether to force <see cref="GetPackages"/>.</param>
        /// <param name="logOutput">Whether the output should be logged to the faux PowerShell console or returned as results.</param>
        /// <param name="clearBuffer">Whether the faux PowerShell console should be cleared.</param>
        /// <returns>A collection of the ouptut of the PowerShell runspace. Will be empty if <paramref cref="logOuput"/> is true.</returns>
        public async Task<PSDataCollection<PSObject>> RunPackageCommand(string commandString, bool refreshPackages = true,
            bool logOutput = true, bool clearBuffer = true)
        {
            _progressService.StartLoading();

            _ps.Commands.Clear();

            if(clearBuffer)
                _progressService.Output.Clear();

            var ps = _ps.AddScript(commandString);
            if (logOutput)
            {
                ps.Streams.Verbose.DataAdded += (obj, args) =>
                {
                    var output = ps.Streams.Verbose[args.Index];
                    WriteOutput(output.Message);
                };
                ps.Streams.Error.DataAdded += (obj, args) =>
                {
                    var output = ps.Streams.Verbose[args.Index];
                    WriteError(output.Message);
                };
            }

            PSDataCollection<PSObject> results = null;
            try
            {
                results =
                    await
                        Task.Factory.FromAsync((ac, s) => ps.BeginInvoke(new PSDataCollection<PSObject>(), new PSInvocationSettings(), ac, s),
                            ar => ps.EndInvoke(ar), null);
            }
            catch (Exception e)
            {
                WriteError(e.ToString());
                _progressService.StopLoading();
                return results;
            }

            if (logOutput)
            {
                WriteOutput("Executed successfully...");
            }

            if (refreshPackages)
                await GetPackages(true);

            _progressService.StopLoading();
            return results;
        }

        /// <summary>
        /// Helper function to write output messages to the faux PowerShell console.
        /// </summary>
        /// <param name="message">Message to be written.</param>
        private void WriteOutput(string message)
        {
            _progressService.Output.Add(new PowerShellOutputLine { Text = message, Type = PowerShellLineType.Output });
        }

        /// <summary>
        /// Helper function to write error messages to the faux PowerShell console.
        /// </summary>
        /// <param name="message">Message to be written.</param>
        private void WriteError(string message)
        {
            _progressService.Output.Add(new PowerShellOutputLine { Text = message, Type = PowerShellLineType.Error });
        }

        void NotifyPackagesChanged(PackagesChangedEventType command, string packageId = "", string packageVersion = "")
        {
            var packagesUpdated = PackagesUpdated;
            if (packagesUpdated != null)
                packagesUpdated(this, new PackagesChangedEventArgs { EventType = command, PackageId = packageId, PackageVersion = packageVersion});
        }

        public event PackagesChangedEventHandler PackagesUpdated;
    }
}
