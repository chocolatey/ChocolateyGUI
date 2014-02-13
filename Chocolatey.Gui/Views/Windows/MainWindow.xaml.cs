using System.ComponentModel;
using System.Windows;
using Autofac;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Windows;
using Chocolatey.Gui.Views.Controls;

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
            var navigationService = App.Container.Resolve<INavigationService>();
            navigationService.SetNavigationItem(GlobalFrame);
            navigationService.Navigate(typeof(SourcesControl));

            var progressService = App.Container.Resolve<IProgressService>();
            LoadingOverlay.DataContext = progressService;
        }
    }
}
