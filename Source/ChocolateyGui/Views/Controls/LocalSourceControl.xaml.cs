using System;
using System.Windows.Input;
using ChocolateyGui.Services;
using ChocolateyGui.ViewModels.Controls;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Views.Controls
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
            if (item == null)
            {
                return;
            }

            await item.EnsureIsLoaded();
            _navigationService.Value.Navigate(typeof(PackageControl), item);
        }
    }
}
