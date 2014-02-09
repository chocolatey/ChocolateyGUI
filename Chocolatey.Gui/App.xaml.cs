using Autofac;
using Chocolatey.Gui.IoC;
using Chocolatey.Gui.Services;
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
            Container = AutoFacConfiguration.RegisterAutoFac();
        }
    }
}
