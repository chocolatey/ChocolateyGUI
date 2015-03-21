// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="CSharpChocolateyPackageService.cs">
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
    using System.Text;
    using System.Threading.Tasks;

    using ChocolateyGui.AsyncProcess;
    using ChocolateyGui.Enums;
    using ChocolateyGui.Models;
    using ChocolateyGui.Providers;
    using ChocolateyGui.Utilities.Extensions;
    using ChocolateyGui.Utilities.Nuspec;
    using ChocolateyGui.ViewModels.Items;
    
    public class CSharpChocolateyPackageService : BasePackageService, IChocolateyPackageService
    {
        private string chocoExePath = string.Empty;

        public CSharpChocolateyPackageService(IProgressService progressService, Func<Type, ILogService> logServiceFunc, IChocolateyConfigurationProvider chocolateyConfigurationProvider)
            : base(progressService, logServiceFunc, chocolateyConfigurationProvider)
        {
            this.chocoExePath = Path.Combine(this.ChocolateyConfigurationProvider.ChocolateyInstall, "choco.exe");
        }

        public async Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false)
        {
            // Ensure that we only retrieve the packages one at a time to refresh the Cache.
            using (await this.GetInstalledLock.LockAsync())
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

                await this.ProgressService.StartLoading("Chocolatey Service");
                this.ProgressService.WriteMessage("Retrieving installed packages...");

                var result = await ProcessEx.RunAsync(this.chocoExePath, "list -lo");

                var chocoPackageList = result.StandardOutput.Where(p => PackageRegex.IsMatch(p.ToString()))
                    .Select(p => PackageRegex.Match(p.ToString()))
                    .ToDictionary(m => m.Groups["Name"].Value, m => new SemanticVersion(m.Groups["VersionString"].Value));

                var libPath = Path.Combine(this.ChocolateyConfigurationProvider.ChocolateyInstall, "lib");
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
                    BasePackageService.LocalPackagesCacheKeyName,
                    packages,
                    new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(1)
                    });

                await this.ProgressService.StopLoading();
                return packages;
            }            
        }

        public async Task InstallPackage(string id, SemanticVersion version = null, Uri source = null, bool force = false)
        {
            await this.ProgressService.StartLoading(string.Format("Installing {0}...", id));
            this.ProgressService.WriteMessage("Building chocolatey command...");

            var arguments = new StringBuilder();
            arguments.AppendFormat("install {0} -y", id);

            if (version != null)
            {
                arguments.AppendFormat(" --version {0}", version);
            }

            if (source != null)
            {
                arguments.AppendFormat(" --source {0}", source);
            }

            if (force)
            {
                arguments.Append(" --force");
            }

            await ProcessEx.RunAsync(this.chocoExePath, arguments.ToString());

            var newPackage =
                (await this.GetInstalledPackages(true)).OrderByDescending(p => p.Version)
                    .FirstOrDefault(
                        p =>
                        string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0
                        && (version == null || version == p.Version));

            if (newPackage != null)
            {
                this.AddPackageEntry(newPackage.Id, newPackage.Version, source);
            }

            this.NotifyPackagesChanged(PackagesChangedEventType.Installed, id, version == null ? string.Empty : version.ToString());

            await this.ProgressService.StopLoading();
        }

        public async Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            await this.ProgressService.StartLoading(string.Format("Uninstalling {0}...", id));
            this.ProgressService.WriteMessage("Building chocolatey command...");

            var arguments = new StringBuilder();
            arguments.AppendFormat("uninstall {0} -y", id);

            if (version != null)
            {
                arguments.AppendFormat(" --version {0}", version);
            }

            await ProcessEx.RunAsync(this.chocoExePath, arguments.ToString());
            await this.GetInstalledPackages(force: true);

            this.RemovePackageEntry(id, version);
            this.NotifyPackagesChanged(PackagesChangedEventType.Uninstalled, id, version.ToString());

            await this.ProgressService.StopLoading();
        }

        public async Task UpdatePackage(string id, Uri source = null)
        {
            await this.ProgressService.StartLoading(string.Format("Updating {0}...", id));
            this.ProgressService.WriteMessage("Building chocolatey command...");
            var currentPackages = this.PackageConfigEntries().Where(p => string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0).ToList();

            var arguments = new StringBuilder();
            arguments.AppendFormat("upgrade {0} -y", id);

            await ProcessEx.RunAsync(this.chocoExePath, arguments.ToString());

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
            await this.ProgressService.StopLoading();
        }

        public async Task<Dictionary<string, string>> SearchPackages(string queryString, bool includePrerelease, bool includeAllVersions, Uri source)
        {
            throw new NotImplementedException();
        }
    }
}