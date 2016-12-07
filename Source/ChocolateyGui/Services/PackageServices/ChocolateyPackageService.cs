// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using chocolatey;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.results;
using ChocolateyGui.Providers;
using ChocolateyGui.Services.PackageServices;
using ChocolateyGui.Utilities.Extensions;
using ChocolateyGui.ViewModels.Items;
using NuGet;

namespace ChocolateyGui.Services
{
    public class ChocolateyPackageService : BasePackageService, IChocolateyPackageService
    {
        private readonly IMapper _mapper;
        private readonly Func<IPackageViewModel> _packageFactory;

        public ChocolateyPackageService(
            IProgressService progressService,
            IChocolateyConfigurationProvider chocolateyConfigurationProvider,
            IMapper mapper,
            IEventAggregator eventAggregator,
            Func<IPackageViewModel> packageFactory)
            : base(progressService, chocolateyConfigurationProvider, eventAggregator)
        {
            _mapper = mapper;
            _packageFactory = packageFactory;
        }

        public async Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false)
        {
            // Ensure that we only retrieve the packages one at a time to refresh the Cache.
            using (await GetInstalledLock.LockAsync())
            {
                ICollection<IPackageViewModel> packages;
                if (!force)
                {
                    packages = CachedPackages;

                    if (packages != null)
                    {
                        return packages;
                    }
                }

                using (await StartProgressDialog("Chocolatey Service", "Retrieving installed packages..."))
                {
                    var choco = Lets.GetChocolatey().Init(ProgressService);
                    choco.Set(config =>
                    {
                        config.CommandName = CommandNameType.list.ToString();
                        config.ListCommand.LocalOnly = true;
                        config.AllowUnofficialBuild = true;
#if !DEBUG
                        config.Verbose = false;
#endif // DEBUG
                    });

                    var packageResults = await choco.ListAsync<PackageResult>();

                    packages = packageResults
                        .Select(
                            package =>
                            {
                                var packageInfo = ChocolateyExtensions.GetPackageInformationService().get_package_information(package.Package);
                                var pgck = _mapper.Map(package.Package, _packageFactory());
                                pgck.IsPinned = packageInfo.IsPinned;
                                return pgck;
                            })
                        .Select(package =>
                        {
                            package.IsInstalled = true;
                            return package;
                        }).ToList();

                    CachedPackages = packages;

                    return packages;
                }
            }
        }

        public async Task InstallPackage(string id, SemanticVersion version = null, Uri source = null,
            bool force = false)
        {
            var failed = false;
            var errors = new List<string>();
            using (await StartProgressDialog("Install Package", "Installing package", id))
            {
                var choco = Lets.GetChocolatey().Init(ProgressService, message => errors.Add(message));
                choco.Set(config =>
                {
                    config.CommandName = CommandNameType.install.ToString();
                    config.PackageNames = id;
                    config.AllowUnofficialBuild = true;
#if !DEBUG
                    config.Verbose = false;
#endif // DEBUG

                    if (version != null)
                    {
                        config.Version = version.ToString();
                    }

                    if (source != null)
                    {
                        config.Sources = source.ToString();
                    }

                    if (force)
                    {
                        config.Force = true;
                    }
                });

                await choco.RunAsync();

                if (Environment.ExitCode != 0)
                {
                    failed = true;
                    Environment.ExitCode = 0;
                }

                await GetInstalledPackages(true);

                InstalledPackage(id, version);
            }

            if (failed)
            {
                await ProgressService.ShowMessageAsync("Failed to intall package", string.Join("\n", errors));
            }
        }

        public async Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            var failed = false;
            var errors = new List<string>();
            using (await StartProgressDialog("Uninstalling", "Uninstalling package", id))
            {
                var choco = Lets.GetChocolatey().Init(ProgressService, message => errors.Add(message));
                choco.Set(config =>
                {
                    config.CommandName = CommandNameType.uninstall.ToString();
                    config.PackageNames = id;
                    config.AllowUnofficialBuild = true;
#if !DEBUG
                    config.Verbose = false;
#endif // DEBUG

                    if (version != null)
                    {
                        config.Version = version.ToString();
                    }
                });

                await choco.RunAsync();

                if (Environment.ExitCode != 0)
                {
                    failed = true;
                    Environment.ExitCode = 0;
                }

                await GetInstalledPackages(true);

                UninstalledPackage(id, version);
            }

            if (failed)
            {
                await ProgressService.ShowMessageAsync("Failed to intall package", string.Join("\n", errors));
            }
        }

        public async Task UpdatePackage(string id, Uri source = null)
        {
            var failed = false;
            var errors = new List<string>();
            using (await StartProgressDialog("Updating", "Updating package", id))
            {
                var choco = Lets.GetChocolatey().Init(ProgressService, message => errors.Add(message));
                choco.Set(config =>
                {
                    config.CommandName = CommandNameType.upgrade.ToString();
                    config.PackageNames = id;
                    config.AllowUnofficialBuild = true;
#if !DEBUG
                    config.Verbose = false;
#endif // DEBUG
                });

                await choco.RunAsync();

                if (Environment.ExitCode != 0)
                {
                    failed = true;
                    Environment.ExitCode = 0;
                }

                await GetInstalledPackages(true);

                UpdatedPackage(id);
            }

            if (failed)
            {
                await ProgressService.ShowMessageAsync("Failed to intall package", string.Join("\n", errors));
            }
        }
    }
}