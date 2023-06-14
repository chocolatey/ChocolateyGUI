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
using chocolatey.infrastructure.app;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.app.nuget;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.results;
using chocolatey.infrastructure.services;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Utilities;
using Microsoft.VisualStudio.Threading;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using ChocolateySource = ChocolateyGui.Common.Models.ChocolateySource;
using IFileSystem = chocolatey.infrastructure.filesystem.IFileSystem;

namespace ChocolateyGui.Common.Windows.Services
{
    using ChocolateySource = ChocolateySource;
    using ILogger = Serilog.ILogger;
    using LogLevel = Models.LogLevel;
    using LogMessage = Models.LogMessage;

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
            _choco = Lets.GetChocolatey(initializeLogging: false).SetCustomLogging(new SerilogLogger(Logger, _progressService), logExistingMessages: false, addToExistingLoggers: true);

            _localAppDataPath = _fileSystem.CombinePaths(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Chocolatey GUI");
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
                        config.CommandName = CommandNameType.List.ToString();
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
                var packages = await Task.Run(() => nugetService.List(chocoConfig));
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

            var outdatedPackagesFile = _fileSystem.CombinePaths(_localAppDataPath, "outdatedPackages.xml");

            var outdatedPackagesCacheDurationInMinutesSetting = _configService.GetEffectiveConfiguration().OutdatedPackagesCacheDurationInMinutes;
            int outdatedPackagesCacheDurationInMinutes = 0;
            if (!string.IsNullOrWhiteSpace(outdatedPackagesCacheDurationInMinutesSetting))
            {
                int.TryParse(outdatedPackagesCacheDurationInMinutesSetting, out outdatedPackagesCacheDurationInMinutes);
            }

            if (_fileSystem.FileExists(outdatedPackagesFile) && (DateTime.Now - _fileSystem.GetFileModifiedDate(outdatedPackagesFile)).TotalMinutes < outdatedPackagesCacheDurationInMinutes)
            {
                return _xmlService.Deserialize<List<OutdatedPackage>>(outdatedPackagesFile);
            }
            else
            {
                var choco = Lets.GetChocolatey(initializeLogging: false);
                choco.Set(
                    config =>
                    {
                        config.CommandName = "outdated";
                        config.PackageNames = packageName ?? chocolatey.infrastructure.app.ApplicationParameters.AllPackages;
                        config.UpgradeCommand.NotifyOnlyAvailableUpgrades = true;
                        config.RegularOutput = false;
                        config.QuietOutput = true;
                        config.Prerelease = false;

                        if (forceCheckForOutdatedPackages)
                        {
                            config.SetCacheExpirationInMinutes(0);
                        }
                    });
                var chocoConfig = choco.GetConfiguration();

                // If there are no Sources configured, for example, if they are all disabled, then figuring out
                // which packages are outdated can't be completed.
                if (chocoConfig.Sources != null)
                {
                    var nugetService = choco.Container().GetInstance<INugetService>();
                    var packages = await Task.Run(() => nugetService.UpgradeDryRun(chocoConfig, null));
                    var results = packages
                        .Where(p => !p.Value.Inconclusive)
                        .Select(p => new OutdatedPackage
                        { Id = p.Value.Name, VersionString = p.Value.Version })
                        .ToArray();

                    try
                    {
                        // The XmlService won't create a new file, if the file already exists with the same hash,
                        // i.e. the list of outdated packages hasn't changed. Currently, we check for new outdated
                        // packages, when the serialized file has become old/stale, so we NEED the file to be re-written
                        // when this check is done, so that it isn't always doing the check. Therefore, when we are
                        // getting ready to serialize the list of outdated packages, if the file already exists, delete it.
                        if (_fileSystem.FileExists(outdatedPackagesFile))
                        {
                            _fileSystem.DeleteFile(outdatedPackagesFile);
                        }

                        _xmlService.Serialize(results, outdatedPackagesFile);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, L(nameof(Resources.Application_OutdatedPackagesError)));
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
            bool force = false,
            AdvancedInstall advancedInstallOptions = null)
        {
            using (await Lock.WriteLockAsync())
            {
                var logger = new SerilogLogger(Logger, _progressService);
                var choco = Lets.GetChocolatey(initializeLogging: false).SetCustomLogging(logger, logExistingMessages: false, addToExistingLoggers: true);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.Install.ToString();
                            config.PackageNames = id;
                            config.Features.UsePackageExitCodes = false;

                            if (version != null)
                            {
                                config.Version = version;
                            }

                            if (source != null)
                            {
                                config.Sources = source.ToString();
                            }

                            if (force)
                            {
                                config.Force = true;
                            }

                            if (advancedInstallOptions != null)
                            {
                                config.InstallArguments = advancedInstallOptions.InstallArguments;
                                config.PackageParameters = advancedInstallOptions.PackageParameters;
                                config.CommandExecutionTimeoutSeconds = advancedInstallOptions.ExecutionTimeoutInSeconds;

                                if (!string.IsNullOrEmpty(advancedInstallOptions.LogFile))
                                {
                                    config.AdditionalLogFileLocation = advancedInstallOptions.LogFile;
                                }

                                config.Prerelease = advancedInstallOptions.PreRelease;
                                config.ForceX86 = advancedInstallOptions.Forcex86;
                                config.OverrideArguments = advancedInstallOptions.OverrideArguments;
                                config.NotSilent = advancedInstallOptions.NotSilent;
                                config.ApplyInstallArgumentsToDependencies = advancedInstallOptions.ApplyInstallArgumentsToDependencies;
                                config.ApplyPackageParametersToDependencies = advancedInstallOptions.ApplyPackageParametersToDependencies;
                                config.AllowDowngrade = advancedInstallOptions.AllowDowngrade;
                                config.IgnoreDependencies = advancedInstallOptions.IgnoreDependencies;
                                config.ForceDependencies = advancedInstallOptions.ForceDependencies;
                                config.SkipPackageInstallProvider = advancedInstallOptions.SkipPowerShell;
                                config.Features.ChecksumFiles = !advancedInstallOptions.IgnoreChecksums;
                                config.Features.AllowEmptyChecksums = advancedInstallOptions.AllowEmptyChecksums;
                                config.Features.AllowEmptyChecksumsSecure = advancedInstallOptions.AllowEmptyChecksumsSecure;

                                if (advancedInstallOptions.RequireChecksums)
                                {
                                    config.Features.AllowEmptyChecksums = false;
                                    config.Features.AllowEmptyChecksumsSecure = false;
                                }

                                if (!string.IsNullOrEmpty(advancedInstallOptions.CacheLocation))
                                {
                                    config.CacheLocation = advancedInstallOptions.CacheLocation;
                                }

                                config.DownloadChecksum = advancedInstallOptions.DownloadChecksum;
                                config.DownloadChecksum64 = advancedInstallOptions.DownloadChecksum64bit;
                                config.DownloadChecksumType = advancedInstallOptions.DownloadChecksumType;
                                config.DownloadChecksumType64 = advancedInstallOptions.DownloadChecksumType64bit;

                                if (advancedInstallOptions.IgnoreHTTPCache)
                                {
                                    config.SetCacheExpirationInMinutes(0);
                                }
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
                        config.CommandName = "search";
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

            var nugetLogger = _choco.Container().GetInstance<NuGet.Common.ILogger>();
            var origVer = chocoConfig.Version;
            chocoConfig.Version = version;
            var nugetPackage = await Task.Run(() => (NugetList.GetPackages(chocoConfig, nugetLogger, _fileSystem) as IQueryable<IPackageSearchMetadata>).FirstOrDefault());
            chocoConfig.Version = origVer;
            if (nugetPackage == null)
            {
                throw new Exception("No Package Found");
            }

            return GetMappedPackage(_choco, new PackageResult(nugetPackage, null, chocoConfig.Sources), _mapper);
        }

        public async Task<List<NuGetVersion>> GetAvailableVersionsForPackageIdAsync(string id, int page, int pageSize, bool includePreRelease)
        {
            _choco.Set(
                config =>
                {
                    config.CommandName = "list";
                    config.Input = id;
                    config.ListCommand.Exact = true;
                    config.ListCommand.Page = page;
                    config.ListCommand.PageSize = pageSize;
                    config.Prerelease = includePreRelease;
                    config.AllVersions = true;
                    config.QuietOutput = true;
                    config.RegularOutput = false;
#if !DEBUG
                                config.Verbose = false;
#endif // DEBUG
                });
            var chocoConfig = _choco.GetConfiguration();
            var packages = await _choco.ListAsync<PackageResult>();
            return packages.Select(p => NuGetVersion.Parse(p.Version)).OrderByDescending(p => p.Version).ToList();
        }

        public async Task<PackageOperationResult> UninstallPackage(string id, string version, bool force = false)
        {
            using (await Lock.WriteLockAsync())
            {
                var logger = new SerilogLogger(Logger, _progressService);
                var choco = Lets.GetChocolatey(initializeLogging: false).SetCustomLogging(logger, logExistingMessages: false, addToExistingLoggers: true);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.Uninstall.ToString();
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
                var choco = Lets.GetChocolatey(initializeLogging: false).SetCustomLogging(logger, logExistingMessages: false, addToExistingLoggers: true);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.Upgrade.ToString();
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
                            config.PinCommand.Command = PinCommandType.Add;
                            config.PinCommand.Name = id;
                            config.Version = version;
                            config.Sources = ApplicationParameters.PackagesLocation;
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
                            config.PinCommand.Command = PinCommandType.Remove;
                            config.PinCommand.Name = id;
                            config.Version = version;
                            config.Sources = ApplicationParameters.PackagesLocation;
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
            if (feature == null)
            {
                return;
            }

            using (await Lock.WriteLockAsync())
            {
                _choco.Set(
                    config =>
                    {
                        config.CommandName = "feature";
                        config.FeatureCommand.Command = feature.Enabled ? chocolatey.infrastructure.app.domain.FeatureCommandType.Enable : chocolatey.infrastructure.app.domain.FeatureCommandType.Disable;
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
                            config.ConfigCommand.Command = chocolatey.infrastructure.app.domain.ConfigCommandType.Set;
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

            var filteredSourceIds = _configSettingsService.ListSources(_choco.GetConfiguration()).Select(s => s.Id).ToArray();

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
                        config.SourceCommand.Command = SourceCommandType.Add;
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
                    config.SourceCommand.Command = SourceCommandType.Disable;
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
                    config.SourceCommand.Command = SourceCommandType.Enable;
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
                            config.SourceCommand.Command = SourceCommandType.Remove;
                            config.SourceCommand.Name = id;
                        });

                await _choco.RunAsync();
                return true;
            }
        }

        public async Task ExportPackages(string exportFilePath, bool includeVersionNumbers)
        {
            _choco.Set(
                config =>
                {
                    config.CommandName = "export";
                    config.ExportCommand.OutputFilePath = exportFilePath;
                    config.ExportCommand.IncludeVersionNumbers = includeVersionNumbers;
                });

            await _choco.RunAsync();
        }

        private static Package GetMappedPackage(GetChocolatey choco, PackageResult package, IMapper mapper, bool forceInstalled = false)
        {
            var mappedPackage = package == null ? null : mapper.Map<Package>(package.SearchMetadata);
            if (mappedPackage != null)
            {
                if (package.PackageMetadata != null)
                {
                    mappedPackage.ReleaseNotes = package.PackageMetadata.ReleaseNotes;
                    mappedPackage.Language = package.PackageMetadata.Language;
                    mappedPackage.Copyright = package.PackageMetadata.Copyright;
                }

                var packageInfoService = choco.Container().GetInstance<IChocolateyPackageInformationService>();
                var packageInfo = packageInfoService.Get(package.PackageMetadata);
                mappedPackage.IsPinned = packageInfo.IsPinned;
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation) || forceInstalled;

                mappedPackage.IsPrerelease = mappedPackage.Version.IsPrerelease;
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

        private static string L(string key)
        {
            return TranslationSource.Instance[key];
        }

        private static string L(string key, params object[] parameters)
        {
            return TranslationSource.Instance[key, parameters];
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
                    () => xmlService.Deserialize<ConfigFileSettings>(chocolatey.infrastructure.app.ApplicationParameters.GlobalConfigFileLocation));
            return config;
        }
    }
}