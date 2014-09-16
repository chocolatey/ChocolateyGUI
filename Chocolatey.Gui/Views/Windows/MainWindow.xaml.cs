using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.Controls.Dialogs;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Properties;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.ViewModels.Windows;
using Chocolatey.Gui.Views.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Chocolatey.Gui.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IProgressService _progressService;

        public MainWindow(IMainWindowViewModel vm, INavigationService navigationService, IProgressService progressService)
        {
            InitializeComponent();
            DataContext = vm;

            if (progressService is ProgressService)
                (progressService as ProgressService).MainWindow = this;
            _progressService = progressService;

            AutoMapper.Mapper.CreateMap<V2FeedPackage, PackageViewModel>();
            AutoMapper.Mapper.CreateMap<PackageMetadata, PackageViewModel>();

            navigationService.SetNavigationItem(GlobalFrame);
            navigationService.Navigate(typeof(SourcesControl));

            InitializeChocoDirectory();
        }

        public Task<ChocolateyDialogController> ShowChocolateyDialogAsync(string title, bool isCancelable = false, MetroDialogSettings settings = null)
        {
            return ((Task<ChocolateyDialogController>)Dispatcher.Invoke(new Func<Task<ChocolateyDialogController>>(async () =>
            {
                //create the dialog control
                var dialog = new ChocolateyDialog(this);
                dialog.Title = title;
                dialog.IsCancelable = isCancelable;
                dialog.OutputBuffer = _progressService.Output;

                if (settings == null)
                    settings = MetroDialogOptions;

                dialog.NegativeButtonText = settings.NegativeButtonText;

                await this.ShowMetroDialogAsync(dialog);
                return new ChocolateyDialogController(dialog, () =>
                {
                    return this.HideMetroDialogAsync(dialog);
                });
            })));
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
            SettingsFlyout.Width = Width / 3;
            SettingsFlyout.IsOpen = !SettingsFlyout.IsOpen;
        }

        private void SourcesButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsFlyout.IsOpen = false;
            SourcesFlyout.IsOpen = true;
            SourcesFlyout.IsOpenChanged += SourcesFlyout_IsOpenChanged;
        }

        private void SourcesFlyout_IsOpenChanged(object sender, EventArgs e)
        {
            if (SourcesFlyout.IsOpen == false)
            {
                SettingsFlyout.IsOpen = true;
                SourcesFlyout.IsOpenChanged -= SourcesFlyout_IsOpenChanged;
            }
        }
    }
}