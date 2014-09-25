// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceControl.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Views.Controls
{
    using System;
    using System.Windows.Input;
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Controls;
    using ChocolateyGui.ViewModels.Items;

    /// <summary>
    /// Interaction logic for LocalSourceControl.xaml
    /// </summary>
    public partial class LocalSourceControl
    {
        private readonly Lazy<INavigationService> _navigationService; 

        public LocalSourceControl(ILocalSourceControlViewModel viewModel, Lazy<INavigationService> navigationService)
        {
            InitializeComponent();
            DataContext = viewModel;

            this._navigationService = navigationService;

            Loaded += viewModel.Loaded;
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
