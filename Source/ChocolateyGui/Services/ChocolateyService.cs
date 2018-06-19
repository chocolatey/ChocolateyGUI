// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
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
    using Microsoft.VisualStudio.Threading;
    using Models;
    using NuGet;
    using ChocolateySource = Models.ChocolateySource;
    using ILogger = Serilog.ILogger;

    internal class ChocolateyService : IChocolateyService
    {
        private static readonly ILogger Logger = Serilog.Log.ForContext<ChocolateyService>();
        private static readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();
        private readonly IMapper _mapper;
        private readonly IProgressService _progressService;
        private readonly IChocolateyConfigSettingsService _configSettingsService;
#pragma warning disable SA1401 // Fields must be private
#pragma warning restore SA1401 // Fields must be private

        public ChocolateyService(IMapper mapper, IProgressService progressService, IChocolateyConfigSettingsService configSettingsService)
        {
            _mapper = mapper;
            _progressService = progressService;
            _configSettingsService = configSettingsService;
        }

        public Task<bool> IsElevated()
        {
            return Task.FromResult(Hacks.IsElevated);
        }

        public async Task<IEnumerable<Package>> GetInstalledPackages()
        {
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.list.ToString();
                            config.ListCommand.LocalOnly = true;
                        });

                return (await choco.ListAsync<PackageResult>())
                     .Select(package => GetMappedPackage(choco, package, _mapper, true))
                     .ToArray();
            }
        }

        public async Task<IReadOnlyList<Tuple<string, SemanticVersion>>> GetOutdatedPackages(bool includePrerelease = false, string packageName = null)
        {
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                    config =>
                        {
                            config.CommandName = "outdated";
                            config.PackageNames = packageName ?? ApplicationParameters.AllPackages;
                            config.UpgradeCommand.NotifyOnlyAvailableUpgrades = true;
                            config.RegularOutput = false;
                            config.QuietOutput = true;
                            config.Prerelease = false;
                        });
                var chocoConfig = choco.GetConfiguration();

                var nugetService = choco.Container().GetInstance<INugetService>();
                var packages = await Task.Run(() => nugetService.upgrade_noop(chocoConfig, null));
                var results = packages
                    .Where(p => !p.Value.Inconclusive)
                    .Select(p => Tuple.Create(p.Value.Package.Id.ToLower(), p.Value.Package.Version.ToNormalizedString()))
                    .ToArray();
                var parsed = results.Select(result => Tuple.Create(result.Item1, new SemanticVersion(result.Item2)));

                return parsed.ToList();
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
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.list.ToString();
                            config.Input = query;
                            config.AllVersions = options.IncludeAllVersions;
                            config.ListCommand.Page = options.CurrentPage;
                            config.ListCommand.PageSize = options.PageSize;
                            config.Prerelease = options.IncludePrerelease;
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
                    (await choco.ListAsync<PackageResult>()).Select(
                        pckge => GetMappedPackage(choco, pckge, _mapper));

                return new PackageResults
                {
                    Packages = packages.ToArray(),
                    TotalCount = await Task.Run(() => choco.ListCount())
                };
            }
        }

        public async Task<Package> GetByVersionAndIdAsync(string id, string version, bool isPrerelease)
        {
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
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
                var chocoConfig = choco.GetConfiguration();

                var nugetLogger = choco.Container().GetInstance<NuGet.ILogger>();
                var semvar = new SemanticVersion(version);
                var nugetPackage = (NugetList.GetPackages(chocoConfig, nugetLogger) as IQueryable<IPackage>).FirstOrDefault(p => p.Version == semvar);
                if (nugetPackage == null)
                {
                    throw new Exception("No Package Found");
                }

                return GetMappedPackage(choco, new PackageResult(nugetPackage, null, chocoConfig.Sources), _mapper);
            }
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
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                    config =>
                        {
                            config.CommandName = "pin";
                            config.PinCommand.Command = PinCommandType.add;
                            config.PinCommand.Name = id;
                            config.Version = version;
                        });

                try
                {
                    await choco.RunAsync();
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
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                    config =>
                        {
                            config.CommandName = "pin";
                            config.PinCommand.Command = PinCommandType.remove;
                            config.PinCommand.Name = id;
                            config.Version = version;
                        });
                try
                {
                    await choco.RunAsync();
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
            using (await Lock.ReadLockAsync())
            {
                var config = await GetConfigFile();
                return config.Features.Select(_mapper.Map<ChocolateyFeature>).ToArray();
            }
        }

        public async Task SetFeature(ChocolateyFeature feature)
        {
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                    config =>
                    {
                        config.CommandName = "feature";
                        config.FeatureCommand.Command = feature.Enabled ? FeatureCommandType.enable : FeatureCommandType.disable;
                        config.FeatureCommand.Name = feature.Name;
                    });

                await choco.RunAsync();
            }
        }

        public async Task<ChocolateySetting[]> GetSettings()
        {
            using (await Lock.ReadLockAsync())
            {
                var config = await GetConfigFile();
                return config.ConfigSettings.Select(_mapper.Map<ChocolateySetting>).ToArray();
            }
        }

        public async Task SetSetting(ChocolateySetting setting)
        {
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                    config =>
                        {
                            config.CommandName = "config";
                            config.ConfigCommand.Command = ConfigCommandType.set;
                            config.ConfigCommand.Name = setting.Key;
                            config.ConfigCommand.ConfigValue = setting.Value;
                        });

                await choco.RunAsync();
            }
        }

        public async Task<ChocolateySource[]> GetSources()
        {
            using (await Lock.ReadLockAsync())
            {
                var sources = _configSettingsService.source_list(_choco.GetConfiguration());
                var mappedSources = sources.Select(_mapper.Map<ChocolateySource>).ToArray();
                return mappedSources;
            }
        }

        public async Task AddSource(ChocolateySource source)
        {
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
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

                await choco.RunAsync();

                if (source.Disabled)
                {
                    choco.Set(
                        config =>
                            {
                                config.CommandName = "source";
                                config.SourceCommand.Command = SourceCommandType.disable;
                                config.SourceCommand.Name = source.Id;
                            });
                    await choco.RunAsync();
                }
                else
                {
                    choco.Set(
                       config =>
                       {
                           config.CommandName = "source";
                           config.SourceCommand.Command = SourceCommandType.enable;
                           config.SourceCommand.Name = source.Id;
                       });
                    await choco.RunAsync();
                }
            }
        }

        public async Task UpdateSource(string id, ChocolateySource source)
        {
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

                var choco = Lets.GetChocolatey().SetCustomLogging(new SerilogLogger(Logger, _progressService));
                choco.Set(
                        config =>
                        {
                            config.CommandName = "source";
                            config.SourceCommand.Command = SourceCommandType.remove;
                            config.SourceCommand.Name = id;
                        });

                await choco.RunAsync();
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
            var choco = Lets.GetChocolatey();
            var xmlService = choco.Container().GetInstance<IXmlService>();
            var config =
                await Task.Run(
                    () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
            return config;
        }
    }
}