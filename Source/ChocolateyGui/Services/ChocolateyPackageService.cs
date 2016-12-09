// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyPackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using chocolatey;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.results;
using ChocolateyGui.Models;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Services.PackageServices;
using ChocolateyGui.Utilities;
using ChocolateyGui.Utilities.Extensions;
using ChocolateyGui.ViewModels.Items;
using MahApps.Metro.Controls.Dialogs;
using NuGet;
using Serilog;
using ILogger = Serilog.ILogger;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace ChocolateyGui.Services
{
    public class ChocolateyPackageService : IChocolateyPackageService
    {
        /// <summary>
        ///     The key in the <see cref="Cache">Service's Memory Cache</see> for this service's packages./>
        /// </summary>
        private const string LocalPackagesCacheKeyName = "LocalChocolateyService.Packages";

        private readonly IProgressService _progressService;
        private readonly IMapper _mapper;
        private readonly Func<IPackageViewModel> _packageFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly AsyncLock _getInstalledLock = new AsyncLock();

        private readonly ILogger _logger = Log.ForContext<ChocolateyPackageService>();

        public ChocolateyPackageService(
            IProgressService progressService,
            IMapper mapper,
            IEventAggregator eventAggregator,
            Func<IPackageViewModel> packageFactory)
        {
            _progressService = progressService;
            _mapper = mapper;
            _eventAggregator = eventAggregator;
            _packageFactory = packageFactory;
        }

        public async Task<PackageSearchResults> Search(string query)
        {
            return await Search(query, new PackageSearchOptions());
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options)
        {
            await _progressService.StartLoading("Search");
            if (string.IsNullOrWhiteSpace(query))
            {
                _progressService.WriteMessage("Loading data...");
            }
            else
            {
                _progressService.WriteMessage(string.Format("Searching for {0}", query));
            }

            var results = await SearchImpl(query, options);
            await _progressService.StopLoading();
            return results;
        }

        public async Task<IPackageViewModel> GetLatest(string id, bool includePrerelease = false)
        {
            _progressService.WriteMessage(string.Format("Getting latest version of {0}...", id));
            var choco = Lets.GetChocolatey().Init(_progressService);
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = id;
                config.AllowUnofficialBuild = true;
                config.ListCommand.ByIdOnly = true;
                config.Prerelease = includePrerelease;
                config.ListCommand.Exact = true;
#if !DEBUG
                config.Verbose = false;
#endif // DEBUG
            });

            var packageResults = await choco.ListAsync<PackageResult>();
            var package = packageResults
                .FirstOrDefault(
                    result =>
                        includePrerelease ? result.Package.IsAbsoluteLatestVersion : result.Package.IsLatestVersion);

            return GetMappedPackage(package, _packageFactory);
        }

        public async Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version,
            bool isPrerelease)
        {
            var choco = Lets.GetChocolatey().Init(_progressService);
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = id;
                config.AllowUnofficialBuild = true;
                config.ListCommand.ByIdOnly = true;
                config.Prerelease = isPrerelease;
                config.ListCommand.Exact = true;
                config.AllVersions = true;
#if !DEBUG
                config.Verbose = false;
#endif // DEBUG
            });

            var packageResults = await choco.ListAsync<PackageResult>();
            var package = packageResults
                .Where(result => string.Equals(result.Package.Id, id, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault(
                    result => result.Package.Version == version);

            return GetMappedPackage(package, _packageFactory);
        }

        public async Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false)
        {
            // Ensure that we only retrieve the packages one at a time to refresh the Cache.
            using (await _getInstalledLock.LockAsync())
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
                    var choco = Lets.GetChocolatey().Init(_progressService);
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
            if (!Privileged.IsElevated)
            {
                await ShieldElevation("install package");
                return;
            }

            var failed = false;
            var errors = new List<string>();
            using (await StartProgressDialog("Install Package", "Installing package", id))
            {
                var choco = Lets.GetChocolatey().Init(_progressService, message => errors.Add(message));
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

                try
                {
                    await choco.RunAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to install package {Id}", id);
                    failed = true;
                    errors.Add(ex.Message);
                }

                if (Environment.ExitCode != 0)
                {
                    failed = true;
                    Environment.ExitCode = 0;
                }

                await GetInstalledPackages(true);

                _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Installed, version));
            }

            if (failed)
            {
                await _progressService.ShowMessageAsync("Failed to install package", string.Join("\n", errors));
            }
        }

        public async Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            if (!Privileged.IsElevated)
            {
                await ShieldElevation("uninstall package");
                return;
            }

            var failed = false;
            var errors = new List<string>();
            using (await StartProgressDialog("Uninstalling", "Uninstalling package", id))
            {
                var choco = Lets.GetChocolatey().Init(_progressService, message => errors.Add(message));
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

                _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Uninstalled, version));
            }

            if (failed)
            {
                await _progressService.ShowMessageAsync("Failed to uninstall package", string.Join("\n", errors));
            }
        }

        public async Task UpdatePackage(string id, Uri source = null)
        {
            if (!Privileged.IsElevated)
            {
                await ShieldElevation("update package");
                return;
            }

            var failed = false;
            var errors = new List<string>();
            using (await StartProgressDialog("Updating", "Updating package", id))
            {
                var choco = Lets.GetChocolatey().Init(_progressService, message => errors.Add(message));
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

                _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Updated));
            }

            if (failed)
            {
                await _progressService.ShowMessageAsync("Failed to update package", string.Join("\n", errors));
            }
        }

        public async Task PinPackage(string id, SemanticVersion version)
        {
            if (!Privileged.IsElevated)
            {
                await ShieldElevation("pin package");
                return;
            }

            using (await StartProgressDialog("Pinning", "Pinning package", id))
            {
                var choco = Lets.GetChocolatey().Init(_progressService);
                choco.Set(config =>
                {
                    config.CommandName = "pin";
                    config.PinCommand.Command = PinCommandType.add;
                    config.PinCommand.Name = id;
                    config.Version = version.ToNormalizedString();
                    config.AllowUnofficialBuild = true;
#if !DEBUG
                    config.Verbose = false;
#endif // DEBUG
                });

                await choco.RunAsync();

                await GetInstalledPackages(true);

                _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Pinned));
            }
        }

        public async Task UnpinPackage(string id, SemanticVersion version)
        {
            if (!Privileged.IsElevated)
            {
                await ShieldElevation("unpin package");
                return;
            }

            using (await StartProgressDialog("Pinning", "Pinning package", id))
            {
                var choco = Lets.GetChocolatey().Init(_progressService);
                choco.Set(config =>
                {
                    config.CommandName = "pin";
                    config.PinCommand.Command = PinCommandType.remove;
                    config.PinCommand.Name = id;
                    config.Version = version.ToNormalizedString();
                    config.AllowUnofficialBuild = true;
#if !DEBUG
                    config.Verbose = false;
#endif // DEBUG
                });

                await choco.RunAsync();

                await GetInstalledPackages(true);

                _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Unpinned));
            }
        }

        /// <summary>
        ///     Gets or sets the Cache for this service where out installed packages list is stored.
        /// </summary>
        private static MemoryCache Cache { get; set; } = MemoryCache.Default;

        public static ICollection<IPackageViewModel> CachedPackages
        {
            get { return (ICollection<IPackageViewModel>)Cache.Get(LocalPackagesCacheKeyName); }

            protected set { Cache.Set(LocalPackagesCacheKeyName, value, DateTimeOffset.Now + TimeSpan.FromMinutes(5)); }
        }

        private IPackageViewModel GetMappedPackage(PackageResult package, Func<IPackageViewModel> packageFactory)
        {
            var mappedPackage = package == null ? null : _mapper.Map(package.Package, packageFactory());
            if (mappedPackage != null)
            {
                var packageInfo = ChocolateyExtensions.GetPackageInformationService().get_package_information(package.Package);
                mappedPackage.IsPinned = packageInfo.IsPinned;
            }

            return mappedPackage;
        }

        private async Task<PackageSearchResults> SearchImpl(string query, PackageSearchOptions options)
        {
            var choco = Lets.GetChocolatey().Init(_progressService);
            choco.Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = query;
                config.AllowUnofficialBuild = true;
                config.AllVersions = options.IncludeAllVersions;
                config.Prerelease = options.IncludePrerelease;
                config.ListCommand.Page = options.CurrentPage;
                config.ListCommand.PageSize = options.PageSize;
                config.ListCommand.OrderByPopularity = string.IsNullOrWhiteSpace(options.SortColumn) ||
                                                       options.SortColumn == "DownloadCount";
                config.ListCommand.Exact = options.MatchQuery;
#if !DEBUG
                config.Verbose = false;
#endif // DEBUG
            });

            var packages = (await choco.ListAsync<PackageResult>()).Select(package =>
            {
                var mappedPackage = GetMappedPackage(package, _packageFactory);
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation);
                return mappedPackage;
            });

            return new PackageSearchResults
            {
                Packages = packages,
                TotalCount = await Task.Run(() => choco.ListCount())
            };
        }

        private async Task<IDisposable> StartProgressDialog(string commandString, string initialProgressText, string id = "")
        {
            await _progressService.StartLoading($"{commandString} {id}...");
            _progressService.WriteMessage(initialProgressText);
            return new DisposableAction(() => _progressService.StopLoading());
        }

        private async Task<bool> ShieldElevation(string actionName)
        {
            var result =
                await
                    _progressService.ShowMessageAsync("Elevation Required",
                        "You must be running this app as admin to perform this operation",
                        MessageDialogStyle.AffirmativeAndNegative);

            if (result != MessageDialogResult.Affirmative)
            {
                return false;
            }

            if (Privileged.Elevate())
            {
                Application.Current.Shutdown();
                return true;
            }
            else
            {
                await _progressService.ShowMessageAsync("Error", $"Unable to {actionName}. {Environment.NewLine}Elevation was canceled or there was an issue running the application as an administrator.");
                return false;
            }
        }
    }
}