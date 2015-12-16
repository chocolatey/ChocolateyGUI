﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AutoFacConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.IoC
{
    using System;
    using Autofac;
    using ChocolateyGui.Providers;
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Controls;
    using ChocolateyGui.ViewModels.Items;
    using ChocolateyGui.ViewModels.Windows;
    using ChocolateyGui.Views.Controls;
    using ChocolateyGui.Views.Windows;

    public static class AutoFacConfiguration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is really a requirement due to required registrations.")]
        public static IContainer RegisterAutoFac()
        {
            var builder = new ContainerBuilder();

            // Register Providers
            builder.RegisterType<VersionNumberProvider>().As<IVersionNumberProvider>().SingleInstance();

            var configurationProvider = new ChocolateyConfigurationProvider();
            builder.RegisterInstance(configurationProvider).As<IChocolateyConfigurationProvider>().SingleInstance();

            if (configurationProvider.IsChocolateyExecutableBeingUsed)
            {
                RegisterCSharpServices(builder);
            }
            else
            {
                RegisterPowerShellServices(builder);
            }

            // Register View Models
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();
            builder.RegisterType<SourceViewModel>();
            builder.Register(
                (c, parameters) =>
                new SourceTabViewModel(
                    c.Resolve(
                        typeof(Lazy<>).MakeGenericType(parameters.TypedAs<Type>()),
                        new TypedParameter(typeof(Uri), parameters.TypedAs<Uri>())),
                    parameters.TypedAs<string>()));

            builder.RegisterType<SourcesControlViewModel>().As<ISourcesControlViewModel>();
            builder.RegisterType<LocalSourceControlViewModel>().As<ILocalSourceControlViewModel>();
            builder.RegisterType<RemoteSourceControlViewModel>().As<IRemoteSourceControlViewModel>();
            builder.RegisterType<PackageControlViewModel>().As<IPackageControlViewModel>();
            builder.Register(c => new PackageViewModel(c.Resolve<IPackageService>(), c.Resolve<IChocolateyPackageService>(), c.Resolve<INavigationService>())).As<IPackageViewModel>();

            // Register Services
            builder.Register((c, parameters) => new Log4NetLoggingService(parameters.TypedAs<Type>())).As<ILogService>();           
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<PackageService>().As<IPackageService>().SingleInstance();
            builder.RegisterType<ProgressService>().As<IProgressService>().SingleInstance();
            builder.RegisterType<PersistenceService>().As<IPersistenceService>().SingleInstance();

            // Register Views
            builder.RegisterType<MainWindow>();
            builder.RegisterType<SourcesControl>();
            builder.RegisterType<LocalSourceControl>();
            builder.Register((c, parameters) =>
                new RemoteSourceControl(c.Resolve<IRemoteSourceControlViewModel>(parameters), c.Resolve<Lazy<INavigationService>>()));
            builder.Register((c, pvm) => new PackageControl(c.Resolve<IPackageControlViewModel>(), pvm.TypedAs<PackageViewModel>()));

            return builder.Build();
        }

        private static void RegisterPowerShellServices(ContainerBuilder builder)
        {
            builder.RegisterType<PowerShellChocolateyPackageService>().As<IChocolateyPackageService>().SingleInstance();
            builder.RegisterType<SettingsSourceService>().As<ISourceService>().SingleInstance();
        }

        private static void RegisterCSharpServices(ContainerBuilder builder)
        {
            builder.RegisterType<CSharpChocolateyPackageService>().As<IChocolateyPackageService>().SingleInstance();
            builder.RegisterType<ChocoSettingsSourceService>().As<ISourceService>().SingleInstance();
        }
    }
}