using System.Windows.Input;
using Autofac;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Controls;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Views.Controls
{
    public partial class LocalSourceControl
    {
        public LocalSourceControl(ILocalSourceControlViewModel vm)
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
                    navigationService.Navigate(typeof(PackageControl), item);
                }
            }
        }
    }
}
