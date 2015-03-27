// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="CSharpChocolateyPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ChocolateyGui.AsyncProcess;
    using ChocolateyGui.Enums;
    using ChocolateyGui.Models;
    using ChocolateyGui.Providers;
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
                ICollection<IPackageViewModel> packages;
                if (!force)
                {
                    packages = BasePackageService.CachedPackages;
                    
                    if (packages != null)
                    {
                        return packages;
                    }
                }

                this.StartProgressDialog("Chocolatey Service", "Retrieving installed packages...");

                var result = await ProcessEx.RunAsync(this.chocoExePath, "list -lo");

                var chocoPackageList = result.StandardOutput.Where(p => PackageRegex.IsMatch(p.ToString(CultureInfo.InvariantCulture)))
                    .Select(p => PackageRegex.Match(p.ToString(CultureInfo.InvariantCulture)))
                    .ToDictionary(m => m.Groups["Name"].Value, m => new SemanticVersion(m.Groups["VersionString"].Value));

                var libPath = Path.Combine(this.ChocolateyConfigurationProvider.ChocolateyInstall, "lib");

                packages = new List<IPackageViewModel>();

                await this.EnumerateLocalPackagesAndSetCache(packages, chocoPackageList, libPath);

                await this.ProgressService.StopLoading();
                return packages;
            }            
        }

        public async Task InstallPackage(string id, SemanticVersion version = null, Uri source = null, bool force = false)
        {
            this.StartProgressDialog("Installing", "Building chocolatey command...", id);

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

            this.UpdatePackageLists(id, source, newPackage, version);
        }

        public async Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            this.StartProgressDialog("Uninstalling", "Building chocolatey command...", id);

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
            this.StartProgressDialog("Updating", "Building chocolatey command...", id);

            var currentPackages = this.PackageConfigEntries().Where(p => string.Compare(p.Id, id, StringComparison.OrdinalIgnoreCase) == 0).ToList();

            var arguments = new StringBuilder();
            arguments.AppendFormat("upgrade {0} -y", id);

            await ProcessEx.RunAsync(this.chocoExePath, arguments.ToString());

            var newPackages = await this.GetInstalledPackages();

            this.UpdatePackageLists(id, source, currentPackages, newPackages);
        }
    }
}