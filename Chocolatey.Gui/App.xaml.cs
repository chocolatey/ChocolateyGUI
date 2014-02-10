using Autofac;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.IoC;
using Chocolatey.Gui.ViewModels.Items;

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

            AutoMapper.Mapper.CreateMap<V2FeedPackage, PackageViewModel>();
        }
    }
}
