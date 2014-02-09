using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Autofac;
using Autofac.Core;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Pages;
using Chocolatey.Gui.ViewModels.Windows;
using Chocolatey.Gui.Views.Pages;

namespace Chocolatey.Gui.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly INavigationService _navigationService;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            _navigationService = App.Container.Resolve<INavigationService>();
            _navigationService.SetNavigationItem(PackagesPageFrame);
        }

        private void SourcesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var target = e.AddedItems[0] as SourceModel;
            if (target.Url == "Local" && !(PackagesPageFrame.NavigationService.Content != null && PackagesPageFrame.NavigationService.Content.GetType() == typeof(LocalSourcePage)))
            {
                _navigationService.Navigate(typeof(LocalSourcePage));
            }
            else
            {
                _navigationService.Navigate(typeof(RemoteSourcePage));
            }
        }
    }
}
