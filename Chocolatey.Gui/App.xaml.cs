using Autofac;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.ViewModels.Pages;
using Chocolatey.Gui.ViewModels.Windows;

namespace Chocolatey.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal static IContainer Container { get; set; }

        static App()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();
            builder.RegisterType<LocalSourcePageViewModel>().As<ILocalSourcePageViewModel>();
            builder.RegisterType<RemoteSourcePageViewModel>().As<IRemoteSourcePageViewModel>();
            builder.RegisterType<PackageViewModel>().As<IPackageViewModel>();

            Container = builder.Build();
        }
    }
}
