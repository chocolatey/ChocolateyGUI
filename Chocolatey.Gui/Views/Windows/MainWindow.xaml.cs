using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Properties;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Items;
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

            var progressService = App.Container.Resolve<IProgressService>();
            LoadingOverlay.DataContext = progressService;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            AutoMapper.Mapper.CreateMap<V2FeedPackage, PackageViewModel>();
            AutoMapper.Mapper.CreateMap<PackageMetadata, PackageViewModel>();

            var navigationService = App.Container.Resolve<INavigationService>();
            navigationService.SetNavigationItem(GlobalFrame);
            navigationService.Navigate(typeof(SourcesControl));

            InitializeChocoDirectory();
        }

        private async void InitializeChocoDirectory()
        {
            await Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(Settings.Default.chocolateyInstall))
                {
                    var chocoDirectoryPath = Environment.GetEnvironmentVariable("ChocolateyInstall");
                    if (string.IsNullOrWhiteSpace(chocoDirectoryPath))
                    {
                        var pathVar = Environment.GetEnvironmentVariable("PATH");
                        if (!string.IsNullOrWhiteSpace(pathVar))
                        {
                            chocoDirectoryPath =
                                pathVar.Split(';')
                                    .SingleOrDefault(
                                        path => path.IndexOf("Chocolatey", StringComparison.OrdinalIgnoreCase) > -1);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(chocoDirectoryPath))
                    {
                        Settings.Default.chocolateyInstall = chocoDirectoryPath;
                        Settings.Default.Save();
                    }
                }

            });
        }
    }
}
