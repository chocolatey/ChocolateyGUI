using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Properties;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.ViewModels.Windows;
using Chocolatey.Gui.Views.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Chocolatey.Gui.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(IMainWindowViewModel vm, INavigationService navigationService, IProgressService progressService)
        {
            InitializeComponent();
            DataContext = vm;

            if (progressService is ProgressService)
                (progressService as ProgressService).MainWindow = this;

            AutoMapper.Mapper.CreateMap<V2FeedPackage, PackageViewModel>();
            AutoMapper.Mapper.CreateMap<PackageMetadata, PackageViewModel>();

            navigationService.SetNavigationItem(GlobalFrame);
            navigationService.Navigate(typeof(SourcesControl));

            InitializeChocoDirectory();
        }

        private async void InitializeChocoDirectory()
        {
            await TaskEx.Run(() =>
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

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.Width = Width/3;
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        private void SourcesButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = false;
            SourcesFlyout.IsOpen = true;
            SourcesFlyout.IsOpenChanged += SourcesFlyout_IsOpenChanged;
        }

        void SourcesFlyout_IsOpenChanged(object sender, EventArgs e)
        {
            if (SourcesFlyout.IsOpen == false)
            {
                SettingsFlyout.IsOpen = true;
                SourcesFlyout.IsOpenChanged -= SourcesFlyout_IsOpenChanged;
            }
        }
    }
}
