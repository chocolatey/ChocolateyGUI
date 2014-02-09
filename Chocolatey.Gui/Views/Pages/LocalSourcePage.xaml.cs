using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Autofac;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.ViewModels.Pages;

namespace Chocolatey.Gui.Views.Pages
{
    /// <summary>
    /// Interaction logic for LocalSourcePage.xaml
    /// </summary>
    public partial class LocalSourcePage
    {
        public LocalSourcePage(ILocalSourcePageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void PackageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic source = e.OriginalSource;
            var item = source.DataContext as IPackageViewModel;
            if (item != null)
            {
                using (var scope = App.Container.BeginLifetimeScope())
                {
                    var navigationService = scope.Resolve<INavigationService>();
                    navigationService.Navigate(typeof(PackagePage), item);
                }
            }
        }
    }
}
