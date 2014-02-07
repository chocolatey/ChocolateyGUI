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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void SourcesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var target = e.AddedItems[0] as SourceModel;
            if (target.Url == "Local" && !(PackagesPageFrame.NavigationService.Content != null && PackagesPageFrame.NavigationService.Content.GetType() == typeof(LocalSourcePage)))
            {
                PackagesPageFrame.Navigate(new LocalSourcePage(App.Container.Resolve<ILocalSourcePageViewModel>()));
            }
            else
            {
                PackagesPageFrame.Navigate(
                    new RemoteSourcePage(App.Container.Resolve<IRemoteSourcePageViewModel>()));
            }
        }
    }
}
