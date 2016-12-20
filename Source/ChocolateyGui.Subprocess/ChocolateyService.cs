using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AutoMapper;
using chocolatey;
using chocolatey.infrastructure.app;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.app.nuget;
using chocolatey.infrastructure.results;
using chocolatey.infrastructure.services;
using ChocolateyGui.Interface;
using ChocolateyGui.Models;
using ChocolateyGui.Subprocess.Models;
using Nito.AsyncEx;
using NuGet;
using WampSharp.V2.Core.Contracts;
using ChocolateySource = ChocolateyGui.Subprocess.Models.ChocolateySource;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Subprocess
{
    internal class ChocolateyService : IChocolateyService
    {
        private static readonly ILogger Logger = Serilog.Log.ForContext<ChocolateyService>();
        private static readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();

        private readonly StreamingLogger _streamingLogger;

        public ChocolateyService(ISubject<StreamingLogMessage> loggingSubject)
        {
            Logger.Debug("Connecting to streaming log endpoint.");
            _streamingLogger = new StreamingLogger(loggingSubject);
        }

        public Task<bool> IsElevated()
        {
            return Task.FromResult(Hacks.IsElevated);
        }

        public async Task<IReadOnlyList<Package>> GetInstalledPackages()
        {
            using (await Lock.ReaderLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.list.ToString();
                            config.ListCommand.LocalOnly = true;
                        });
                return
                    (await choco.ListAsync<PackageResult>()).Select(package => GetMappedPackage(package, true)).ToList();
            }
        }

        public async Task<IReadOnlyList<Tuple<string, string>>> GetOutdatedPackages(bool includePrerelease = false)
        {
            using (await Lock.ReaderLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
                choco.Set(
                    config =>
                        {
                            config.CommandName = "outdated";
                            config.RegularOutput = false;
                            config.Prerelease = false;
                        });

                var chocoConfig = choco.GetConfiguration();
                var nugetLogger = new ChocolateyNugetLogger();
                var packageManager = NugetCommon.GetPackageManager(
                    chocoConfig,
                    nugetLogger,
                    installSuccessAction: null,
                    uninstallSuccessAction: null,
                    addUninstallHandler: false);

                var packageInfoService = Hacks.GetPackageInformationService();
                var ids =
                    packageManager.LocalRepository.GetPackages()
                        .Where(p => !packageInfoService.get_package_information(p).IsPinned);
                var updateable =
                    await Task.Run(() => packageManager.SourceRepository.GetUpdates(ids, false, false).ToList());
                return updateable.Select(p => Tuple.Create(p.Id, p.Version.ToNormalizedString())).ToList();
            }
        }

        public async Task<PackageOperationResult> InstallPackage(string id, string version = null, Uri source = null,
            bool force = false)
        {
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
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

                var errors = new List<string>();
                Action<StreamingLogMessage> grabErrors = m =>
                    {
                        switch (m.LogLevel)
                        {
                            case StreamingLogLevel.Warn:
                            case StreamingLogLevel.Error:
                            case StreamingLogLevel.Fatal:
                                errors.Add(m.Message);
                                break;
                        }
                    };

                using (_streamingLogger.Intercept(grabErrors))
                {
                    await choco.RunAsync();

                    if (Environment.ExitCode != 0)
                    {
                        Environment.ExitCode = 0;
                        return new PackageOperationResult { Successful = false, Messages = errors };
                    }
                    return PackageOperationResult.SuccessfulCached;
                }
            }
        }
        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options)
        {
            using (await Lock.ReaderLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger).Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.list.ToString();
                            config.Input = query;
                            config.AllowUnofficialBuild = true;
                            config.AllVersions = options.IncludeAllVersions;
                            config.Prerelease = options.IncludePrerelease;
                            config.ListCommand.Page = options.CurrentPage;
                            config.ListCommand.PageSize = options.PageSize;
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

                var packages = (await choco.ListAsync<PackageResult>()).Select(pckge => GetMappedPackage(pckge));

                return new PackageSearchResults
                           {
                               Packages = packages,
                               TotalCount = await Task.Run(() => choco.ListCount())
                           };
            }
        }

        public async Task<Package> GetByVersionAndIdAsync(string id, string version, bool isPrerelease)
        {
            using (await Lock.ReaderLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.list.ToString();
                            config.Input = id;
                            config.Version = version;
#if !DEBUG
                config.Verbose = false;
#endif // DEBUG
                        });

                var chocoConfig = choco.GetConfiguration();
                var nugetLogger = new ChocolateyNugetLogger();
                var packageManager = NugetCommon.GetPackageManager(
                    chocoConfig,
                    nugetLogger,
                    installSuccessAction: null,
                    uninstallSuccessAction: null,
                    addUninstallHandler: false);

                var rawPackage =
                    await Task.Run(
                        () =>
                            packageManager.SourceRepository.FindPackage(
                                id,
                                SemanticVersion.Parse(version),
                                allowPrereleaseVersions: isPrerelease,
                                allowUnlisted: true));
                if (rawPackage == null)
                {
                    throw new WampException(new Dictionary<string, object>(), "com.chocolatey.no_package");
                }

                return GetMappedPackage(new PackageResult(rawPackage, null, chocoConfig.Sources));
            }
        }

        public async Task<PackageOperationResult> UninstallPackage(string id, string version, bool force = false)
        {
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
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

                var errors = new List<string>();
                Action<StreamingLogMessage> grabErrors = m =>
                    {
                        switch (m.LogLevel)
                        {
                            case StreamingLogLevel.Warn:
                            case StreamingLogLevel.Error:
                            case StreamingLogLevel.Fatal:
                                errors.Add(m.Message);
                                break;
                        }
                    };

                using (_streamingLogger.Intercept(grabErrors))
                {
                    try
                    {
                        await choco.RunAsync();
                    }
                    catch (Exception ex)
                    {
                        return new PackageOperationResult { Successful = false, Messages = errors, Exception = ex };
                    }

                    if (Environment.ExitCode != 0)
                    {
                        Environment.ExitCode = 0;
                        return new PackageOperationResult { Successful = false, Messages = errors };
                    }
                    return PackageOperationResult.SuccessfulCached;
                }
            }
        }

        public async Task<PackageOperationResult> UpdatePackage(string id, Uri source = null)
        {
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.upgrade.ToString();
                            config.PackageNames = id;
                            config.Features.UsePackageExitCodes = false;
                        });


                var errors = new List<string>();
                Action<StreamingLogMessage> grabErrors = m =>
                    {
                        switch (m.LogLevel)
                        {
                            case StreamingLogLevel.Warn:
                            case StreamingLogLevel.Error:
                            case StreamingLogLevel.Fatal:
                                errors.Add(m.Message);
                                break;
                        }
                    };

                using (_streamingLogger.Intercept(grabErrors))
                {
                    try
                    {
                        await choco.RunAsync();
                    }
                    catch (Exception ex)
                    {
                        return new PackageOperationResult { Successful = false, Messages = errors, Exception = ex };
                    }

                    if (Environment.ExitCode != 0)
                    {
                        Environment.ExitCode = 0;
                        return new PackageOperationResult { Successful = false, Messages = errors };
                    }
                    return PackageOperationResult.SuccessfulCached;
                }
            }
        }

        public async Task<PackageOperationResult> PinPackage(string id, string version)
        {
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
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
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger);
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

        public async Task<IReadOnlyList<ChocolateyFeature>> GetFeatures()
        {
            using (await Lock.ReaderLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var config =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                return (config.Features.Select(Mapper.Map<ChocolateyFeature>).ToList());
            }
        }

        public async Task SetFeature(ChocolateyFeature feature)
        {
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger).Set(
                    config =>
                    {
                        config.CommandName = "feature";
                        config.FeatureCommand.Command = feature.Enabled ? FeatureCommandType.enable : FeatureCommandType.disable;
                        config.FeatureCommand.Name = feature.Name;
                    });

                await choco.RunAsync();
            }
        }

        public async Task<IReadOnlyList<ChocolateySetting>> GetSettings()
        {
            using (await Lock.ReaderLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var config =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                return (config.ConfigSettings.Select(Mapper.Map<ChocolateySetting>).ToList());
            }
        }

        public async Task SetSetting(ChocolateySetting setting)
        {
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger).Set(
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

        public async Task<IReadOnlyList<ChocolateySource>> GetSources()
        {
            using (await Lock.ReaderLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var config =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                return (config.Sources.Select(Mapper.Map<ChocolateySource>).ToList());
            }
        }

        public async Task AddSource(ChocolateySource source)
        {
            using (await Lock.WriterLockAsync())
            {
                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger).Set(
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
            using (await Lock.WriterLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var chocoCOnfig =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                var sources = (chocoCOnfig.Sources.Select(Mapper.Map<ChocolateySource>).ToList());

                if (sources.All(source => source.Id != id))
                {
                    return false;
                }

                var choco = Lets.GetChocolatey().SetCustomLogging(_streamingLogger).Set(
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

        public void Exit()
        {
            Program.CanceledEvent.Set();
        }

        private static Package GetMappedPackage(PackageResult package, bool forceInstalled = false)
        {
            var mappedPackage = package == null ? null : Mapper.Map<Package>(package.Package);
            if (mappedPackage != null)
            {
                var packageInfo = Hacks.GetPackageInformationService().get_package_information(package.Package);
                mappedPackage.IsPinned = packageInfo.IsPinned;
                mappedPackage.IsInstalled = !string.IsNullOrWhiteSpace(package.InstallLocation) || forceInstalled;
            }
            return mappedPackage;
        }
    }
}
