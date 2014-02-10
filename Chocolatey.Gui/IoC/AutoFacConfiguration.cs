using Autofac;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Controls;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.ViewModels.Windows;
using Chocolatey.Gui.Views.Controls;
using Chocolatey.Gui.Views.Windows;

namespace Chocolatey.Gui.IoC
{
    public class AutoFacConfiguration
    {
        public static IContainer RegisterAutoFac()
        {
            var builder = new ContainerBuilder();

            // Register View Models
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();
            builder.RegisterType<SourcesControlViewModel>().As<ISourcesControlViewModel>();
            builder.RegisterType<LocalSourceControlViewModel>().As<ILocalSourceControlViewModel>();
            builder.RegisterType<RemoteSourceControlViewModel>().As<IRemoteSourceControlViewModel>();
            builder.RegisterType<PackageViewModel>().As<IPackageViewModel>();
            builder.RegisterType<PackageControlViewModel>().As<IPackageControlViewModel>();

            // Register Services
            builder.Register(c => new NavigationService()).As<INavigationService>().SingleInstance();
            builder.RegisterType<PackageService>().As<IPackageService>().SingleInstance();

            // Register Views
            builder.RegisterType<MainWindow>();
            builder.RegisterType<SourcesControl>();
            builder.RegisterType<LocalSourceControl>();
            builder.RegisterType<RemoteSourceControl>();
            builder.Register((c,pvm) => new PackageControl(c.Resolve<IPackageControlViewModel>(), pvm.TypedAs<PackageViewModel>()));

            return builder.Build();
        }
    }
}
