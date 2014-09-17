using System;
using Autofac;
using ChocolateyGui.Services;
using ChocolateyGui.ViewModels.Controls;
using ChocolateyGui.ViewModels.Items;
using ChocolateyGui.ViewModels.Windows;
using ChocolateyGui.Views.Controls;
using ChocolateyGui.Views.Windows;

namespace ChocolateyGui.IoC
{
    public class AutoFacConfiguration
    {
        public static IContainer RegisterAutoFac()
        {
            var builder = new ContainerBuilder();

            // Register View Models
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();
            builder.RegisterType<SourceViewModel>();
            builder.Register((c, parameters) =>
                new SourceTabViewModel(c.Resolve(typeof(Lazy<>).MakeGenericType(parameters.TypedAs<Type>()),
                    new TypedParameter(typeof(Uri), parameters.TypedAs<Uri>())),
                    parameters.TypedAs<String>()));
            builder.RegisterType<SourcesControlViewModel>().As<ISourcesControlViewModel>();
            builder.RegisterType<LocalSourceControlViewModel>().As<ILocalSourceControlViewModel>();
            builder.RegisterType<RemoteSourceControlViewModel>().As<IRemoteSourceControlViewModel>();
            builder.RegisterType<PackageControlViewModel>().As<IPackageControlViewModel>();
            builder.Register(c => new PackageViewModel(c.Resolve<IPackageService>(), c.Resolve<IChocolateyService>(), c.Resolve<INavigationService>())).As<IPackageViewModel>();

            // Register Services
            builder.Register((c,parameters) => new Log4NetLoggingService(parameters.TypedAs<Type>())).As<ILogService>();
            builder.RegisterType<SettingsSourceService>().As<ISourceService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<PackageService>().As<IPackageService>().SingleInstance();
            builder.RegisterType<ChocolateyService>().As<IChocolateyService>().SingleInstance();
            builder.RegisterType<ProgressService>().As<IProgressService>().SingleInstance();

            // Register Views
            builder.RegisterType<MainWindow>();
            builder.RegisterType<SourcesControl>();
            builder.RegisterType<LocalSourceControl>();
            builder.Register((c, parameters) => 
                new RemoteSourceControl(c.Resolve<IRemoteSourceControlViewModel>(parameters), c.Resolve<Lazy<INavigationService>>()));
            builder.Register((c,pvm) => new PackageControl(c.Resolve<IPackageControlViewModel>(), pvm.TypedAs<PackageViewModel>()));


            return builder.Build();
        }
    }
}
