// --------------------------------------------------------------------------------------------------------------------
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
        public static IContainer RegisterAutoFac()
        {
            var builder = new ContainerBuilder();

            // Register Providers
            builder.RegisterType<VersionNumberProvider>().As<IVersionNumberProvider>().SingleInstance();

            var configurationProvider = new ChocolateyConfigurationProvider();
            builder.RegisterInstance(configurationProvider).As<IChocolateyConfigurationProvider>().SingleInstance();

            if (configurationProvider.IsChocolateyExecutableBeingUsed)
            {
                RegisterCSharpService(builder);
            }
            else
            {
                RegisterPowerShellService(builder);
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
            builder.RegisterType<SettingsSourceService>().As<ISourceService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<PackageService>().As<IPackageService>().SingleInstance();
            builder.RegisterType<ProgressService>().As<IProgressService>().SingleInstance();

            // Register Views
            builder.RegisterType<MainWindow>();
            builder.RegisterType<SourcesControl>();
            builder.RegisterType<LocalSourceControl>();
            builder.Register((c, parameters) =>
                new RemoteSourceControl(c.Resolve<IRemoteSourceControlViewModel>(parameters), c.Resolve<Lazy<INavigationService>>()));
            builder.Register((c, pvm) => new PackageControl(c.Resolve<IPackageControlViewModel>(), pvm.TypedAs<PackageViewModel>()));

            return builder.Build();
        }

        private static void RegisterPowerShellService(ContainerBuilder builder)
        {
            builder.RegisterType<PowerShellChocolateyPackageService>().As<IChocolateyPackageService>().SingleInstance();
        }

        private static void RegisterCSharpService(ContainerBuilder builder)
        {
            builder.RegisterType<CSharpChocolateyPackageService>().As<IChocolateyPackageService>().SingleInstance();
        }
    }
}