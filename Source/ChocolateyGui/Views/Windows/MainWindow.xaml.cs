// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="MainWindow.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Views.Windows
{
    using ChocolateyGui.ChocolateyFeedService;
    using ChocolateyGui.Controls.Dialogs;
    using ChocolateyGui.Models;
    using ChocolateyGui.Properties;
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Items;
    using ChocolateyGui.ViewModels.Windows;
    using ChocolateyGui.Views.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IProgressService _progressService;
        private readonly ILogService _logService;

        public MainWindow(IMainWindowViewModel vm, INavigationService navigationService, 
            IProgressService progressService, ILogService logService)
        {
            InitializeComponent();
            DataContext = vm;

            if (progressService is ProgressService)
            {
                (progressService as ProgressService).MainWindow = this;
            }

            this._progressService = progressService;
            this._logService = logService;

            // RichiCoder1 (21-09-2014) - Why are we doing this, especially here?
            AutoMapper.Mapper.CreateMap<V2FeedPackage, PackageViewModel>();
            AutoMapper.Mapper.CreateMap<PackageMetadata, PackageViewModel>();

            navigationService.SetNavigationItem(GlobalFrame);
            navigationService.Navigate(typeof(SourcesControl));

            this.InitializeChocoDirectory();
        }

        public Task<ChocolateyDialogController> ShowChocolateyDialogAsync(string title, bool isCancelable = false, MetroDialogSettings settings = null)
        {
            return ((Task<ChocolateyDialogController>)Dispatcher.Invoke(new Func<Task<ChocolateyDialogController>>(async () =>
            {
                // create the dialog control
                var dialog = new ChocolateyDialog(this)
                {
                    Title = title,
                    IsCancelable = isCancelable,
                    OutputBuffer = _progressService.Output
                };

                if (settings == null)
                    settings = MetroDialogOptions;

                dialog.NegativeButtonText = settings.NegativeButtonText;

                await this.ShowMetroDialogAsync(dialog);
                return new ChocolateyDialogController(dialog, () => this.HideMetroDialogAsync(dialog));
            })));
        }

        private void InitializeChocoDirectory()
        {
            TaskEx.Run(() =>
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
                    else
                    {
                        _logService.Warn("Unable to find chocolatey install directory!");
                    }
                }
            }).ConfigureAwait(false);
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
            SourcesFlyout.IsOpenChanged += this.SourcesFlyout_IsOpenChanged;
        }

        private void SourcesFlyout_IsOpenChanged(object sender, EventArgs e)
        {
            if (SourcesFlyout.IsOpen == false)
            {
                SettingsFlyout.IsOpen = true;
                SourcesFlyout.IsOpenChanged -= this.SourcesFlyout_IsOpenChanged;
            }
        }
    }
}