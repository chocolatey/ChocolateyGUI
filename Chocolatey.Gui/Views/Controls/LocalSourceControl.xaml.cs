using System;
using System.Windows.Input;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Controls;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Views.Controls
{
    public partial class LocalSourceControl
    {
        private readonly Lazy<INavigationService> _navigationService; 
        public LocalSourceControl(ILocalSourceControlViewModel vm, Lazy<INavigationService> navigationService)
        {
            InitializeComponent();
            DataContext = vm;

            _navigationService = navigationService;

            Loaded += vm.Loaded;
        }

        private async void PackageDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic source = e.OriginalSource;
            var item = source.DataContext as IPackageViewModel;

            await item.EnsureIsLoaded();
            _navigationService.Value.Navigate(typeof(PackageControl), item);
        }
    }
}
