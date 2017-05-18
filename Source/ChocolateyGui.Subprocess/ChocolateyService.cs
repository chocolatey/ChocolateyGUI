// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using chocolatey;
using chocolatey.infrastructure.app;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.app.nuget;
using chocolatey.infrastructure.results;
using chocolatey.infrastructure.services;
using ChocolateyGui.Models;
using Microsoft.VisualStudio.Threading;
using NuGet;
using ChocolateySource = ChocolateyGui.Models.ChocolateySource;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Subprocess
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class ChocolateyService : IIpcChocolateyService
    {
#pragma warning disable SA1401 // Fields must be private
        public static int ConnectedClients;
#pragma warning restore SA1401 // Fields must be private
        private static readonly ILogger Logger = Serilog.Log.ForContext<ChocolateyService>();
        private static readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();

        public void Register()
        {
            OperationContext.Current.Channel.Faulted += (sender, args) => Interlocked.Decrement(ref ConnectedClients);
            OperationContext.Current.Channel.Closed += (sender, args) => Interlocked.Decrement(ref ConnectedClients);
            Interlocked.Increment(ref ConnectedClients);
        }

        public Task<bool> IsElevated()
        {
            return Task.FromResult(Hacks.IsElevated);
        }

        public async Task<Package[]> GetInstalledPackages()
        {
            var operationContext = OperationContext.Current;
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.list.ToString();
                            config.ListCommand.LocalOnly = true;
                        });
                return
                    (await choco.ListAsync<PackageResult>()).Select(package => GetMappedPackage(package, true)).ToArray();
            }
        }

        public async Task<Tuple<string, string>[]> GetOutdatedPackages(bool includePrerelease = false)
        {
            var operationContext = OperationContext.Current;
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
                choco.Set(
                    config =>
                        {
                            config.CommandName = "outdated";
                            config.RegularOutput = false;
                            config.Prerelease = false;
                        });

                var packageManager = NugetCommon.GetPackageManager(
                    choco.GetConfiguration(),
                    new ChocolateyNugetLogger(),
                    installSuccessAction: null,
                    uninstallSuccessAction: null,
                    addUninstallHandler: false);

                var packageInfoService = Hacks.GetPackageInformationService();
                var ids =
                    packageManager.LocalRepository.GetPackages()
                        .Where(p => !packageInfoService.get_package_information(p).IsPinned);
                var updateable =
                    await Task.Run(() => packageManager.SourceRepository.GetUpdates(ids, false, false).ToList());
                return updateable.Select(p => Tuple.Create(p.Id, p.Version.ToNormalizedString())).ToArray();
            }
        }

        public async Task<PackageOperationResult> InstallPackage(
            string id,
            string version = null,
            Uri source = null,
            bool force = false)
        {
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                StreamingLogger logger;
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext, out logger);
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

                Action<StreamingLogMessage> grabErrors;
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
            var operationContext = OperationContext.Current;
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
                choco.Set(
                    config =>
                        {
                            config.CommandName = CommandNameType.list.ToString();
                            config.Input = query;
                            config.AllVersions = options.IncludeAllVersions;
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

                return new PackageResults
                {
                    Packages = packages.ToArray(),
                    TotalCount = await Task.Run(() => choco.ListCount())
                };
            }
        }

        public async Task<Package> GetByVersionAndIdAsync(string id, string version, bool isPrerelease)
        {
            var operationContext = OperationContext.Current;
            using (await Lock.ReadLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
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
                                NuGet.SemanticVersion.Parse(version),
                                allowPrereleaseVersions: isPrerelease,
                                allowUnlisted: true));
                if (rawPackage == null)
                {
                    throw new Exception("No Package Found");
                }

                return GetMappedPackage(new PackageResult(rawPackage, null, chocoConfig.Sources));
            }
        }

        public async Task<PackageOperationResult> UninstallPackage(string id, string version, bool force = false)
        {
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                StreamingLogger logger;
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext, out logger);
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
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                StreamingLogger logger;
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext, out logger);
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
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
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
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
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
            var operationContext = OperationContext.Current;
            using (await Lock.ReadLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var config =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                return config.Features.Select(Mapper.Map<ChocolateyFeature>).ToArray();
            }
        }

        public async Task SetFeature(ChocolateyFeature feature)
        {
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
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
            var operationContext = OperationContext.Current;
            using (await Lock.ReadLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var config =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                return config.ConfigSettings.Select(Mapper.Map<ChocolateySetting>).ToArray();
            }
        }

        public async Task SetSetting(ChocolateySetting setting)
        {
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
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
            var operationContext = OperationContext.Current;
            using (await Lock.ReadLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var config =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                return config.Sources.Select(Mapper.Map<ChocolateySource>).ToArray();
            }
        }

        public async Task AddSource(ChocolateySource source)
        {
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
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
            var operationContext = OperationContext.Current;
            using (await Lock.WriteLockAsync())
            {
                var xmlService = Hacks.GetInstance<IXmlService>();
                var chocoCOnfig =
                    await Task.Run(
                        () => xmlService.deserialize<ConfigFileSettings>(ApplicationParameters.GlobalConfigFileLocation));
                var sources = chocoCOnfig.Sources.Select(Mapper.Map<ChocolateySource>).ToList();

                if (sources.All(source => source.Id != id))
                {
                    return false;
                }

                var choco = Lets.GetChocolatey().SetLoggerContext(operationContext);
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

        public void Exit(bool restartingForAdmin = false)
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

        private static List<string> GetErrors(out Action<StreamingLogMessage> grabErrors)
        {
            var errors = new List<string>();
            grabErrors = m =>
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
            return errors;
        }

        private async Task<PackageOperationResult> RunCommand(GetChocolatey choco, StreamingLogger logger)
        {
            Action<StreamingLogMessage> grabErrors;
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
    }
}