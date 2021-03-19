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
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.nuget;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.filesystem;
using chocolatey.infrastructure.services;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Providers;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.ViewModels;
using ChocolateyGui.Common.Windows.Views;
using LiteDB;
using MahApps.Metro.Controls.Dialogs;
using NuGet;
using ChocolateySource = chocolatey.infrastructure.app.configuration.ChocolateySource;
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

            // Register ViewModels
            builder.RegisterAssemblyTypes(viewModelAssembly)
                .Where(type => type.Name.EndsWith("ViewModel", StringComparison.Ordinal))
                .Where(type => type.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<PackageViewModel>().As<IPackageViewModel>();

            var choco = Lets.GetChocolatey();
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

                config.CreateMap<DataServicePackage, Package>()
                    .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Split(',')))
                    .ForMember(dest => dest.Owners, opt => opt.MapFrom(src => src.Owners.Split(',')));
                config.CreateMap<IPackage, Package>();

                config.CreateMap<ConfigFileFeatureSetting, ChocolateyFeature>();
                config.CreateMap<ConfigFileConfigSetting, ChocolateySetting>();
                config.CreateMap<ConfigFileSourceSetting, Common.Models.ChocolateySource>()
                    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => NugetEncryptionUtility.DecryptString(src.Password)))
                    .ForMember(dest => dest.CertificatePassword, opt => opt.MapFrom(src => NugetEncryptionUtility.DecryptString(src.CertificatePassword)));

                config.CreateMap<ChocolateySource, Common.Models.ChocolateySource>()
                    .ForMember(dest => dest.VisibleToAdminsOnly, opt => opt.MapFrom(src => src.VisibleToAdminOnly));
            });

            builder.RegisterType<BundledThemeService>().As<IBundledThemeService>().SingleInstance();
            builder.RegisterInstance(DialogCoordinator.Instance).As<IDialogCoordinator>();
            builder.RegisterInstance(mapperConfiguration.CreateMapper()).As<IMapper>();

            try
            {
                var userDatabase = new LiteDatabase($"filename={Path.Combine(Bootstrapper.LocalAppDataPath, "data.db")};upgrade=true");

                LiteDatabase globalDatabase;
                if (Hacks.IsElevated)
                {
                    globalDatabase = new LiteDatabase($"filename={Path.Combine(Bootstrapper.AppDataPath, "Config", "data.db")};upgrade=true");
                }
                else
                {
                    if (!File.Exists(Path.Combine(Bootstrapper.AppDataPath, "Config", "data.db")))
                    {
                        // Since the global configuration database file doesn't exist, we must be running in a state where an administrator user
                        // has never run Chocolatey GUI. In this case, use null, which will mean attempts to use the global database will be ignored.
                        globalDatabase = null;
                    }
                    else
                    {
                        // Since this is a non-administrator user, they should only have read permissions to this database
                        globalDatabase = new LiteDatabase($"filename={Path.Combine(Bootstrapper.AppDataPath, "Config", "data.db")};readonly=true");
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
                Bootstrapper.Logger.Error(ex, Resources.Error_DatabaseAccessGui);
                throw;
            }

            builder.RegisterType<ImageService>().As<IImageService>().SingleInstance();
            builder.RegisterType<VersionService>().As<IVersionService>().SingleInstance();
            builder.RegisterType<SplashScreenService>().As<ISplashScreenService>().SingleInstance();
        }
    }
}