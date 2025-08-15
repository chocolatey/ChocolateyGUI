// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyGuiModule.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.IO;
using Autofac;
using AutoMapper;
using Caliburn.Micro;
using chocolatey;
using chocolatey.infrastructure.adapters;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.nuget;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.cryptography;
using chocolatey.infrastructure.filesystem;
using chocolatey.infrastructure.services;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Providers;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Utilities;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.ViewModels;
using ChocolateyGui.Common.Windows.Views;
using LiteDB;
using NuGet.Protocol.Core.Types;
using ChocolateySource = chocolatey.infrastructure.app.configuration.ChocolateySource;
using Environment = System.Environment;
using PackageViewModel = ChocolateyGui.Common.Windows.ViewModels.Items.PackageViewModel;

namespace ChocolateyGui.Common.Windows.Startup
{
    internal class ChocolateyGuiModule : Module
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Not Relevant")]
        protected override void Load(ContainerBuilder builder)
        {
            var viewModelAssembly = typeof(ShellViewModel).Assembly;
            var viewAssembly = typeof(ShellView).Assembly;

            // Register Providers
            builder.RegisterType<VersionNumberProvider>().As<IVersionNumberProvider>().SingleInstance();
            builder.RegisterType<Elevation>().SingleInstance();
            builder.RegisterType<ChocolateyConfigurationProvider>().As<IChocolateyConfigurationProvider>().SingleInstance();
            builder.RegisterType<ChocolateyService>().As<IChocolateyService>().SingleInstance();
            builder.RegisterType<DotNetFileSystem>().As<chocolatey.infrastructure.filesystem.IFileSystem>().SingleInstance();
            builder.RegisterType<PackageArgumentsService>().As<IPackageArgumentsService>().SingleInstance();
            builder.RegisterType<DefaultEncryptionUtility>().As<IEncryptionUtility>().SingleInstance();
            builder.RegisterType<ConfigFileWatcher>().As<IConfigFileWatcher>().SingleInstance();

            // Register ViewModels
            builder.RegisterAssemblyTypes(viewModelAssembly)
                .Where(type => type.Name.EndsWith("ViewModel", StringComparison.Ordinal))
                .Where(type => type.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<PackageViewModel>().As<IPackageViewModel>();

            var choco = Lets.GetChocolatey(initializeLogging: true);
            builder.RegisterInstance(choco.Container().GetInstance<IChocolateyConfigSettingsService>())
                .As<IChocolateyConfigSettingsService>().SingleInstance();
            builder.RegisterInstance(choco.Container().GetInstance<IXmlService>())
                .As<IXmlService>().SingleInstance();

            // Register Views
            builder.RegisterAssemblyTypes(viewAssembly)
                .Where(type => type.Name.EndsWith("View", StringComparison.Ordinal))
                .AsSelf()
                .InstancePerDependency();

            // Register the single window manager for this container
            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();

            // Register the single event aggregator for this container
            builder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();

            // Register Services
            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<ProgressService>().As<IProgressService>().SingleInstance();
            builder.RegisterType<PersistenceService>().As<IPersistenceService>().SingleInstance();
            builder.RegisterType<LiteDBFileStorageService>().As<IFileStorageService>().SingleInstance();
            builder.RegisterType<ChocolateyGuiCacheService>().As<IChocolateyGuiCacheService>().SingleInstance();
            builder.RegisterType<AllowedCommandsService>().As<IAllowedCommandsService>().SingleInstance();

            // Register Mapper
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<IPackageViewModel, IPackageViewModel>()
                    .ForMember(vm => vm.IsInstalled, options => options.Ignore());

                config.CreateMap<IPackageSearchMetadata, Package>()
                    .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Identity.Version))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Identity.Id))
                    .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Split(',')))
                    .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners.Split(',')));

                config.CreateMap<ConfigFileFeatureSetting, ChocolateyFeature>();
                config.CreateMap<ConfigFileConfigSetting, ChocolateySetting>();
                config.CreateMap<ConfigFileSourceSetting, Common.Models.ChocolateySource>()
                    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => NugetEncryptionUtility.DecryptString(src.Password)))
                    .ForMember(dest => dest.CertificatePassword, opt => opt.MapFrom(src => NugetEncryptionUtility.DecryptString(src.CertificatePassword)));

                config.CreateMap<ChocolateySource, Common.Models.ChocolateySource>()
                    .ForMember(dest => dest.VisibleToAdminsOnly, opt => opt.MapFrom(src => src.VisibleToAdminOnly));

                config.CreateMap<AdvancedInstallViewModel, AdvancedInstall>()
                    .ForMember(
                        dest => dest.DownloadChecksum,
                        opt => opt.Condition(source => !source.IgnoreChecksums))
                    .ForMember(
                        dest => dest.DownloadChecksumType,
                        opt => opt.Condition(source =>
                            !source.IgnoreChecksums && !string.IsNullOrEmpty(source.DownloadChecksum)))
                    .ForMember(
                        dest => dest.DownloadChecksum64bit,
                        opt => opt.Condition(source =>
                            Environment.Is64BitOperatingSystem
                            && !source.IgnoreChecksums
                            && !source.Forcex86))
                    .ForMember(
                        dest => dest.DownloadChecksumType64bit,
                        opt => opt.Condition(source =>
                            Environment.Is64BitOperatingSystem
                            && !source.IgnoreChecksums
                            && !source.Forcex86
                            && !string.IsNullOrEmpty(source.DownloadChecksum64bit)))
                    .ForMember(
                        dest => dest.PackageParameters,
                        opt => opt.Condition(source => !source.SkipPowerShell))
                    .ForMember(
                        dest => dest.InstallArguments,
                        opt => opt.Condition(source => !source.SkipPowerShell && !source.NotSilent));
            });

            builder.RegisterType<BundledThemeService>().As<IBundledThemeService>().SingleInstance();
            builder.RegisterInstance(mapperConfiguration.CreateMapper()).As<IMapper>();

            builder.Register(c => TranslationSource.Instance).SingleInstance();

            try
            {
                var userDatabase = new LiteDatabase($"filename={Path.Combine(Bootstrapper.LocalAppDataPath, "data.db")};upgrade=true");

                LiteDatabase globalDatabase;
                var globalConfigDirectory = Path.Combine(Bootstrapper.AppDataPath, "Config");
                var globalConfigDatabaseFile = Path.Combine(globalConfigDirectory, "data.db");

                if (Hacks.IsElevated)
                {
                    if (!Directory.Exists(globalConfigDirectory))
                    {
                        Directory.CreateDirectory(globalConfigDirectory);
                        Hacks.LockDirectory(globalConfigDirectory);
                    }

                    globalDatabase = new LiteDatabase($"filename={globalConfigDatabaseFile};upgrade=true");
                }
                else
                {
                    if (!File.Exists(globalConfigDatabaseFile))
                    {
                        // Since the global configuration database file doesn't exist, we must be running in a state where an administrator user
                        // has never run Chocolatey GUI. In this case, use null, which will mean attempts to use the global database will be ignored.
                        globalDatabase = null;
                    }
                    else
                    {
                        // Since this is a non-administrator user, they should only have read permissions to this database
                        globalDatabase = new LiteDatabase($"filename={globalConfigDatabaseFile};readonly=true");
                    }
                }

                if (globalDatabase != null)
                {
                    builder.RegisterInstance(globalDatabase).As<LiteDatabase>().SingleInstance().Named<LiteDatabase>(Bootstrapper.GlobalConfigurationDatabaseName);
                }

                var configService = new ConfigService(globalDatabase, userDatabase);
                configService.SetEffectiveConfiguration();

                var iconService = new PackageIconService(userDatabase);

                builder.RegisterInstance(iconService).As<IPackageIconService>().SingleInstance();
                builder.RegisterInstance(configService).As<IConfigService>().SingleInstance();
                builder.RegisterInstance(new LiteDBFileStorageService(userDatabase)).As<IFileStorageService>().SingleInstance();

                // Since there are two instances of LiteDB, they are added as named instances, so that they can be retrieved when required
                builder.RegisterInstance(userDatabase).As<LiteDatabase>().SingleInstance().Named<LiteDatabase>(Bootstrapper.UserConfigurationDatabaseName);
            }
            catch (IOException ex)
            {
                Bootstrapper.Logger.Error(ex, TranslationSource.Instance[nameof(Resources.Error_DatabaseAccessGui)]);
                throw;
            }

            builder.RegisterType<ImageService>().As<IImageService>().SingleInstance();
            builder.RegisterType<VersionService>().As<IVersionService>().SingleInstance();
            builder.RegisterType<SplashScreenService>().As<ISplashScreenService>().SingleInstance();
        }
    }
}