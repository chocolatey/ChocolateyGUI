// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceControl.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Views.Controls
{
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Controls;
    using ChocolateyGui.ViewModels.Items;
    using System;
    using System.Windows.Input;

    public partial class LocalSourceControl
    {
        private readonly Lazy<INavigationService> _navigationService; 

        public LocalSourceControl(ILocalSourceControlViewModel vm, Lazy<INavigationService> navigationService)
        {
            InitializeComponent();
            DataContext = vm;

            this._navigationService = navigationService;

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
            this._navigationService.Value.Navigate(typeof(PackageControl), item);
        }
    }
}
