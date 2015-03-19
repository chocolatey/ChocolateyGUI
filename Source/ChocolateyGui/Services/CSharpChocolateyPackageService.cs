// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="CSharpChocolateyPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using ChocolateyGui.Models;
    using ChocolateyGui.Providers;
    using ChocolateyGui.Utilities.Nuspec;
    using ChocolateyGui.ViewModels.Items;

    public class CSharpChocolateyPackageService : BasePackageService, IChocolateyPackageService
    {
        public CSharpChocolateyPackageService(IProgressService progressService, Func<Type, ILogService> logServiceFunc, IChocolateyConfigurationProvider chocolateyConfigurationProvider)
            : base(progressService, logServiceFunc, chocolateyConfigurationProvider)
        {
        }

        public async Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false)
        {
            // Ensure that we only retrieve the packages one at a to refresh the Cache.
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

                var chocoExePath = Path.Combine(this.ChocolateyConfigurationProvider.ChocolateyInstall, "choco.exe");

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = chocoExePath,
                        Arguments = "list -lo",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                var outputLines = new Collection<string>();
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    outputLines.Add(proc.StandardOutput.ReadLine());
                }

                var chocoPackageList = outputLines.Where(p => PackageRegex.IsMatch(p.ToString()))
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

        public Task<bool> ExecutePackageCommand(Dictionary<string, object> commandArgs, bool refreshPackages = true)
        {
            throw new NotImplementedException();
        }

        public Task InstallPackage(string id, SemanticVersion version = null, Uri source = null, bool force = false)
        {
            throw new NotImplementedException();
        }

        public Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePackage(string id, Uri source = null)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> SearchPackages(string queryString, bool includePreRelease, bool includeAllVersions, Uri source)
        {
            throw new NotImplementedException();
        }
    }
}