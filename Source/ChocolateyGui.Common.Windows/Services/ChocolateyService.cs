// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using chocolatey;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.app.nuget;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.results;
using chocolatey.infrastructure.services;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using Microsoft.VisualStudio.Threading;
using NuGet;
using ChocolateySource = ChocolateyGui.Common.Models.ChocolateySource;
using IFileSystem = chocolatey.infrastructure.filesystem.IFileSystem;

namespace ChocolateyGui.Common.Windows.Services
{
    using ChocolateySource = ChocolateySource;
    using ILogger = Serilog.ILogger;

    public class ChocolateyService : IChocolateyService
    {
        private static readonly ILogger Logger = Serilog.Log.ForContext<ChocolateyService>();
        private static readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();
        private readonly IMapper _mapper;
        private readonly IProgressService _progressService;
        private readonly IChocolateyConfigSettingsService _configSettingsService;
        private readonly IXmlService _xmlService;
        private readonly IFileSystem _fileSystem;
        private readonly IConfigService _configService;
        private GetChocolatey _choco;
        private string _localAppDataPath = string.Empty;
#pragma warning disable SA1401 // Fields must be private
#pragma warning restore SA1401 // Fields must be private

        public ChocolateyService(IMapper mapper, IProgressService progressService, IChocolateyConfigSettingsService configSettingsService, IXmlService xmlService, IFileSystem fileSystem, IConfigService configService)
        {
            _mapper = mapper;
            _progressService = progressService;
            _configSettingsService = configSettingsService;
            _xmlService = xmlService;
            _fileSystem = fileSystem;
            _configService = configService;
            _choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));

            _localAppDataPath = _fileSystem.combine_paths(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Chocolatey GUI");
        }

        public Task<bool> IsElevated()
        {
            return Task.FromResult(Elevation.Instance.IsElevated);
        }

        public async Task<IEnumerable<Package>> GetInstalledPackages()
        {
            _choco.Set(
                config =>
                    {
                        config.CommandName = CommandNameType.list.ToString();
                        config.ListCommand.LocalOnly = true;
                    });

            var chocoConfig = _choco.GetConfiguration();

            // Not entirely sure what is going on here.  When there are no sources defined, for example, when they
            // are all disabled, the ListAsync command isn't returning any packages installed locally.  When in this
            // situation, use the nugetService directly to get the list of installed packages.
            if (chocoConfig.Sources != null)
            {
                var packages = await _choco.ListAsync<PackageResult>();
                return packages
                    .Select(package => GetMappedPackage(_choco, package, _mapper, true))
                    .ToArray();
            }
            else
            {
                var nugetService = _choco.Container().GetInstance<INugetService>();
                var packages = await Task.Run(() => nugetService.list_run(chocoConfig));
                return packages
                    .Select(package => GetMappedPackage(_choco, package, _mapper, true))
                    .ToArray();
            }
        }

        public async Task<IReadOnlyList<OutdatedPackage>> GetOutdatedPackages(bool includePrerelease = false, string packageName = null, bool forceCheckForOutdatedPackages = false)
        {
            var preventAutomatedOutdatedPackagesCheck = _configService.GetEffectiveConfiguration().PreventAutomatedOutdatedPackagesCheck ?? false;

            if (preventAutomatedOutdatedPackagesCheck && !forceCheckForOutdatedPackages)
            {
                return new List<OutdatedPackage>();
            }

            var outdatedPackagesFile = _fileSystem.combine_paths(_localAppDataPath, "outdatedPackages.xml");

            var outdatedPackagesCacheDurationInMinutesSetting = _configService.GetEffectiveConfiguration().OutdatedPackagesCacheDurationInMinutes;
            int outdatedPackagesCacheDurationInMinutes = 0;
            if (!string.IsNullOrWhiteSpace(outdatedPackagesCacheDurationInMinutesSetting))
            {
                int.TryParse(outdatedPackagesCacheDurationInMinutesSetting, out outdatedPackagesCacheDurationInMinutes);
            }

            if (_fileSystem.file_exists(outdatedPackagesFile) && (DateTime.Now - _fileSystem.get_file_modified_date(outdatedPackagesFile)).TotalMinutes < outdatedPackagesCacheDurationInMinutes)
            {
                return _xmlService.deserialize<List<OutdatedPackage>>(outdatedPackagesFile);
            }
            else
            {
                var choco = Lets.GetChocolatey();
                choco.Set(
                    config =>
                    {
                        config.CommandName = "outdated";
                        config.PackageNames = packageName ?? chocolatey.infrastructure.app.ApplicationParameters.AllPackages;
                        config.UpgradeCommand.NotifyOnlyAvailableUpgrades = true;
                        config.RegularOutput = false;
                        config.QuietOutput = true;
                        config.Prerelease = false;
                    });
                var chocoConfig = choco.GetConfiguration();

                // If there are no Sources configured, for example, if they are all disabled, then figuring out
                // which packages are outdated can't be completed.
                if (chocoConfig.Sources != null)
                {
                    var nugetService = choco.Container().GetInstance<INugetService>();
                    var packages = await Task.Run(() => nugetService.upgrade_noop(chocoConfig, null));
                    var results = packages
                        .Where(p => !p.Value.Inconclusive)
                        .Select(p => new OutdatedPackage
                        { Id = p.Value.Package.Id, VersionString = p.Value.Package.Version.ToNormalizedString() })
                        .ToArray();

                    try
                    {
                        _xmlService.serialize(results, outdatedPackagesFile);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, Resources.Application_OutdatedPackagesError);
                    }

                    return results.ToList();
                }
                else
                {
                    return new List<OutdatedPackage>();
                }
            }
        }

        public async Task<PackageOperationResult> InstallPackage(
            string id,
            string version = null,
            Uri source = null,
            bool force = false)
        {
            using (await Lock.WriteLockAsync())
            {
                var logger = new SerilogLogger(Logger, _progressService);
                var choco = Lets.GetChocolatey().SetCustomLogging(logger);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.install.ToString();
                            config.PackageNames = id;
                            config.Features.UsePackageExitCodes = false;

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

                Action<LogMessage> grabErrors;
                var errors = GetErrors(out grabErrors);

                using (logger.Intercept(grabErrors))
                {
                    await choco.RunAsync();

                    if (Environment.ExitCode != 0)
                    {
                        Environment.ExitCode = 0;
                        return new PackageOperationResult { Successful = false, Messages = errors.ToArray() };
                    }

                    return PackageOperationResult.SuccessfulCached;
                }
            }
        }

        public async Task<PackageResults> Search(string query, PackageSearchOptions options)
        {
            _choco.Set(
                config =>
                    {
                        config.CommandName = CommandNameType.list.ToString();
                        config.Input = query;
                        config.AllVersions = options.IncludeAllVersions;
                        config.ListCommand.Page = options.CurrentPage;
                        config.ListCommand.PageSize = options.PageSize;
                        config.Prerelease = options.IncludePrerelease;
                        config.ListCommand.LocalOnly = false;
                        if (string.IsNullOrWhiteSpace(query) || !string.IsNullOrWhiteSpace(options.SortColumn))
                        {
                            config.ListCommand.OrderByPopularity = string.IsNullOrWhiteSpace(options.SortColumn)
                                                                   || options.SortColumn == "DownloadCount";
                        }
                        config.ListCommand.Exact = options.MatchQuery;
                        if (!string.IsNullOrWhiteSpace(options.Source))
                        {
                            config.Sources = options.Source;
                        }
#if !DEBUG
                        config.Verbose = false;
#endif // DEBUG
                    });

            var packages =
                (await _choco.ListAsync<PackageResult>()).Select(
                    pckge => GetMappedPackage(_choco, pckge, _mapper));

            return new PackageResults
            {
                Packages = packages.ToArray(),
                TotalCount = await Task.Run(() => _choco.ListCount())
            };
        }

        public async Task<Package> GetByVersionAndIdAsync(string id, string version, bool isPrerelease)
        {
            _choco.Set(
                config =>
                {
                    config.CommandName = "list";
                    config.Input = id;
                    config.ListCommand.Exact = true;
                    config.Version = version;
                    config.QuietOutput = true;
                    config.RegularOutput = false;
#if !DEBUG
                    config.Verbose = false;
#endif // DEBUG
                });
            var chocoConfig = _choco.GetConfiguration();

            var nugetLogger = _choco.Container().GetInstance<NuGet.ILogger>();
            var semvar = new SemanticVersion(version);
            var nugetPackage = await Task.Run(() => (NugetList.GetPackages(chocoConfig, nugetLogger) as IQueryable<IPackage>).FirstOrDefault(p => p.Version == semvar));
            if (nugetPackage == null)
            {
                throw new Exception("No Package Found");
            }

            return GetMappedPackage(_choco, new PackageResult(nugetPackage, null, chocoConfig.Sources), _mapper);
        }

        public async Task<PackageOperationResult> UninstallPackage(string id, string version, bool force = false)
        {
            using (await Lock.WriteLockAsync())
            {
                var logger = new SerilogLogger(Logger, _progressService);
                var choco = Lets.GetChocolatey().SetCustomLogging(logger);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.uninstall.ToString();
                            config.PackageNames = id;
                            config.Features.UsePackageExitCodes = false;

                            if (version != null)
                            {
                                config.Version = version.ToString();
                            }
                        });

                return await RunCommand(choco, logger);
            }
        }

        public async Task<PackageOperationResult> UpdatePackage(string id, Uri source = null)
        {
            using (await Lock.WriteLockAsync())
            {
                var logger = new SerilogLogger(Logger, _progressService);
                var choco = Lets.GetChocolatey().SetCustomLogging(logger);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.upgrade.ToString();
                            config.PackageNames = id;
                            config.Features.UsePackageExitCodes = false;
                        });

                return await RunCommand(choco, logger);
            }
        }

        public async Task<PackageOperationResult> PinPackage(string id, string version)
        {
            using (await Lock.WriteLockAsync())
            {
                _choco.Set(
                    config =>
                        {
                            config.CommandName = "pin";
                            config.PinCommand.Command = PinCommandType.add;
                            config.PinCommand.Name = id;
                            config.Version = version;
                        });

                try
                {
                    await _choco.RunAsync();
                }
                catch (Exception ex)
                {
                    return new PackageOperationResult { Successful = false, Exception = ex };
                }

                return PackageOperationResult.SuccessfulCached;
            }
        }

        public async Task<PackageOperationResult> UnpinPackage(string id, string version)
        {
            using (await Lock.WriteLockAsync())
            {
                _choco.Set(
                    config =>
                        {
                            config.CommandName = "pin";
                            config.PinCommand.Command = PinCommandType.remove;
                            config.PinCommand.Name = id;
                            config.Version = version;
                        });
                try
                {
                    await _choco.RunAsync();
                }
                catch (Exception ex)
                {
                    return new PackageOperationResult { Successful = false, Exception = ex };
                }

                return PackageOperationResult.SuccessfulCached;
            }
        }

        public async Task<ChocolateyFeature[]> GetFeatures()
        {
            var config = await GetConfigFile();
            var features = config.Features.Select(_mapper.Map<ChocolateyFeature>);
            return features.OrderBy(f => f.Name).ToArray();
        }

        public async Task SetFeature(ChocolateyFeature feature)
        {
            using (await Lock.WriteLockAsync())
            {
                _choco.Set(
                    config =>
                    {
                        config.CommandName = "feature";
                        config.FeatureCommand.Command = feature.Enabled ? chocolatey.infrastructure.app.domain.FeatureCommandType.enable : chocolatey.infrastructure.app.domain.FeatureCommandType.disable;
                        config.FeatureCommand.Name = feature.Name;
                    });

                await _choco.RunAsync();
            }
        }

        public async Task<ChocolateySetting[]> GetSettings()
        {
            var config = await GetConfigFile();
            var settings = config.ConfigSettings.Select(_mapper.Map<ChocolateySetting>);
            return settings.OrderBy(s => s.Key).ToArray();
        }

        public async Task SetSetting(ChocolateySetting setting)
        {
            using (await Lock.WriteLockAsync())
            {
                _choco.Set(
                    config =>
                        {
                            config.CommandName = "config";
                            config.ConfigCommand.Command = chocolatey.infrastructure.app.domain.ConfigCommandType.set;
                            config.ConfigCommand.Name = setting.Key;
                            config.ConfigCommand.ConfigValue = setting.Value;
                        });

                await _choco.RunAsync();
            }
        }

        public async Task<ChocolateySource[]> GetSources()
        {
            // We only want to provide the sources returned by calling choco.exe, which will exclude those
            // as required, based on configuration.  However, in order to be able to set all properties of the source
            // we need to read all information from the config file, i.e. the username and password
            var config = await GetConfigFile();
            var allSources = config.Sources.Select(_mapper.Map<ChocolateySource>).ToArray();

            var filteredSourceIds = _configSettingsService.source_list(_choco.GetConfiguration()).Select(s => s.Id).ToArray();

            var mappedSources = allSources.Where(s => filteredSourceIds.Contains(s.Id)).ToArray();
            return mappedSources;
        }

        public async Task AddSource(ChocolateySource source)
        {
            using (await Lock.WriteLockAsync())
            {
                _choco.Set(
                    config =>
                    {
                        config.CommandName = "source";
                        config.SourceCommand.Command = SourceCommandType.add;
                        config.SourceCommand.Name = source.Id;
                        config.Sources = source.Value;
                        config.SourceCommand.Username = source.UserName;
                        config.SourceCommand.Password = source.Password;
                        config.SourceCommand.Certificate = source.Certificate;
                        config.SourceCommand.CertificatePassword = source.CertificatePassword;
                        config.SourceCommand.Priority = source.Priority;
                        config.SourceCommand.BypassProxy = source.BypassProxy;
                        config.SourceCommand.AllowSelfService = source.AllowSelfService;
                        config.SourceCommand.VisibleToAdminsOnly = source.VisibleToAdminsOnly;
                    });

                await _choco.RunAsync();

                if (source.Disabled)
                {
                    await DisableSource(source.Id);
                }
                else
                {
                    await EnableSource(source.Id);
                }
            }
        }

        public async Task DisableSource(string id)
        {
            _choco.Set(
                config =>
                {
                    config.CommandName = "source";
                    config.SourceCommand.Command = SourceCommandType.disable;
                    config.SourceCommand.Name = id;
                });

            await _choco.RunAsync();
        }

        public async Task EnableSource(string id)
        {
            _choco.Set(
                config =>
                {
                    config.CommandName = "source";
                    config.SourceCommand.Command = SourceCommandType.enable;
                    config.SourceCommand.Name = id;
                });

            await _choco.RunAsync();
        }

        public async Task UpdateSource(string id, ChocolateySource source)
        {
            // NOTE:  The strategy of first removing, and then re-adding the source
            // is due to the fact that there is no "edit source" command that can
            // be used.  This has the side effect of "having" to decrypt the password
            // for an authenticated source, otherwise, when re-adding the source,
            // the encrypted password, is re-encrypted, making it no longer work.
            if (id != source.Id)
            {
                await RemoveSource(id);
            }

            await AddSource(source);
        }

        public async Task<bool> RemoveSource(string id)
        {
            using (await Lock.WriteLockAsync())
            {
                var chocoConfig = await GetConfigFile();
                var sources = chocoConfig.Sources.Select(_mapper.Map<ChocolateySource>).ToList();

                if (sources.All(source => source.Id != id))
                {
                    return false;
                }

                _choco.Set(
                        config =>
                        {
                            config.CommandName = "source";
                            config.SourceCommand.Command = SourceCommandType.remove;
                            config.SourceCommand.Name = id;
                        });

                await _choco.RunAsync();
                return true;
            }
        }

        private static Package GetMappedPackage(GetChocolatey choco, PackageResult package, IMapper mapper, bool forceInstalled = false)
        {
            var mappedPackage = package == null ? null : mapper.Map<Package>(package.Package);
            if (mappedPackage != null)
            {
                var packageInfoService = choco.Container().GetInstance<IChocolateyPackageInformationService>();
                var packageInfo = packageInfoService.get_package_information(package.Package);
                mappedPackage.IsPinned = packageInfo.IsPinned;
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation) || forceInstalled;
                mappedPackage.IsSideBySide = packageInfo.IsSideBySide;

                mappedPackage.IsPrerelease = !string.IsNullOrWhiteSpace(mappedPackage.Version.SpecialVersion);

                // Add a sanity check here for pre-release packages
                // By default, pre-release packages are marked as IsLatestVersion = false, however, IsLatestVersion is
                // what is used to show/hide the Out of Date message in the UI.  In these cases, if it is a pre-release
                // mark IsLatestVersion as true, and then the outcome of the call to choco outdated will correct whether
                // it is actually Out of Date or not
                if (mappedPackage.IsPrerelease && mappedPackage.IsAbsoluteLatestVersion && !mappedPackage.IsLatestVersion)
                {
                    mappedPackage.IsLatestVersion = true;
                }
            }

            return mappedPackage;
        }

        private static List<string> GetErrors(out Action<LogMessage> grabErrors)
        {
            var errors = new List<string>();
            grabErrors = m =>
            {
                switch (m.LogLevel)
                {
                    case LogLevel.Warn:
                    case LogLevel.Error:
                    case LogLevel.Fatal:
                        errors.Add(m.Message);
                        break;
                }
            };
            return errors;
        }

        private async Task<PackageOperationResult> RunCommand(GetChocolatey choco, SerilogLogger logger)
        {
            Action<LogMessage> grabErrors;
            var errors = GetErrors(out grabErrors);

            using (logger.Intercept(grabErrors))
            {
                try
                {
                    await choco.RunAsync();
                }
                catch (Exception ex)
                {
                    return new PackageOperationResult { Successful = false, Messages = errors.ToArray(), Exception = ex };
                }

                if (Environment.ExitCode != 0)
                {
                    Environment.ExitCode = 0;
                    return new PackageOperationResult { Successful = false, Messages = errors.ToArray() };
                }

                return PackageOperationResult.SuccessfulCached;
            }
        }

        private async Task<ConfigFileSettings> GetConfigFile()
        {
            var xmlService = _choco.Container().GetInstance<IXmlService>();
            var config =
                await Task.Run(
                    () => xmlService.deserialize<ConfigFileSettings>(chocolatey.infrastructure.app.ApplicationParameters.GlobalConfigFileLocation));
            return config;
        }
    }
}