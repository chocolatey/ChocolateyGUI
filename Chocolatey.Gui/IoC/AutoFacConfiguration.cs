using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.ViewModels.Pages;
using Chocolatey.Gui.ViewModels.Windows;
using Chocolatey.Gui.Views.Pages;
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
            builder.RegisterType<LocalSourcePageViewModel>().As<ILocalSourcePageViewModel>();
            builder.RegisterType<RemoteSourcePageViewModel>().As<IRemoteSourcePageViewModel>();
            builder.RegisterType<PackageViewModel>().As<IPackageViewModel>();
            builder.RegisterType<PackagePageViewModel>().As<IPackagePageViewModel>();

            // Register Services
            builder.Register(c => new NavigationService()).As<INavigationService>().SingleInstance();

            // Register Views
            builder.RegisterType<MainWindow>();
            builder.RegisterType<LocalSourcePage>();
            builder.RegisterType<RemoteSourcePage>();
            builder.Register((c,pvm) => new PackagePage(c.Resolve<IPackagePageViewModel>(), pvm.TypedAs<PackageViewModel>()));

            return builder.Build();
        }
    }
}
