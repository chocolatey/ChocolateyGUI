// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShellView.xaml.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using chocolatey.infrastructure.filesystem;
using ChocolateyGui.Common.Providers;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Windows.Controls.Dialogs;
using ChocolateyGui.Common.Windows.Services;
using MahApps.Metro.Controls.Dialogs;
using ChocolateyDialog = ChocolateyGui.Common.Windows.Controls.Dialogs.ChocolateyDialog;

namespace ChocolateyGui.Common.Windows.Views
{
    /// <summary>
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView
    {
        private readonly IChocolateyConfigurationProvider _chocolateyConfigurationProvider;
        private readonly IConfigService _configService;
        private readonly IProgressService _progressService;
        private readonly IFileSystem _fileSystem;
        private readonly IImageService _imageService;

        private bool _closeInitiated = false;

        public ShellView(
            IProgressService progressService,
            IChocolateyConfigurationProvider chocolateyConfigurationProvider,
            IConfigService configService,
            IFileSystem fileSystem,
            IImageService imageService)
        {
            InitializeComponent();

            var service = progressService as ProgressService;
            if (service != null)
            {
                service.ShellView = this;
            }

            _progressService = progressService;
            _chocolateyConfigurationProvider = chocolateyConfigurationProvider;
            _configService = configService;
            _fileSystem = fileSystem;
            _imageService = imageService;

            this.Icon = BitmapFrame.Create(_imageService.ToolbarIconUri);

            CheckOperatingSystemCompatibility();

            // Certain things like Cef (our markdown browser engine) get unhappy when GUI is started from a different cwd.
            // If we're in a different one, reset it to our app files directory.
            if (_fileSystem.get_directory_name(Environment.CurrentDirectory) != Bootstrapper.ApplicationFilesPath)
            {
                Environment.CurrentDirectory = Bootstrapper.ApplicationFilesPath;
            }
        }

        public void CheckOperatingSystemCompatibility()
        {
            var operatingSystemVersion = Environment.OSVersion;

            if (operatingSystemVersion.Version.Major == 10 &&
                !_chocolateyConfigurationProvider.IsChocolateyExecutableBeingUsed)
            {
                MessageBox.Show(
                    "Usage of the PowerShell Version of Chocolatey (i.e. <= 0.9.8.33) has been detected.  Chocolatey GUI does not support using this version of Chocolatey on Windows 10.  Please update Chocolatey to the new C# Version (i.e. > 0.9.9.0) and restart Chocolatey GUI.  This application will now close.",
                    "Incompatible Operating System Version",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.ServiceNotification);

                Application.Current.Shutdown();
            }
        }

        public Task<ChocolateyDialogController> ShowChocolateyDialogAsync(
            string title,
            bool isCancelable = false,
            MetroDialogSettings settings = null)
        {
            return Dispatcher.Invoke(async () =>
            {
                // create the dialog control
                var dialog = new ChocolateyDialog(this, _configService.GetEffectiveConfiguration().ShowConsoleOutput ?? false)
                {
                    Title = title,
                    IsCancelable = isCancelable,
                    OutputBufferCollection = _progressService.Output
                };

                if (settings == null)
                {
                    settings = MetroDialogOptions;
                    settings.NegativeButtonText = Common.Properties.Resources.ChocolateyDialog_Cancel;
                    settings.AffirmativeButtonText = Common.Properties.Resources.ChocolateyDialog_OK;
                }

                dialog.NegativeButtonText = settings.NegativeButtonText;

                await this.ShowMetroDialogAsync(dialog);
                return new ChocolateyDialogController(dialog, () => this.HideMetroDialogAsync(dialog));
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_closeInitiated)
            {
                e.Cancel = true;
                _closeInitiated = true;
                var bootstrapper = (Bootstrapper)Application.Current.FindResource("Bootstrapper");
#pragma warning disable 4014

                // ReSharper disable once PossibleNullReferenceException
                bootstrapper.OnExitAsync().ContinueWith(t => Execute.OnUIThread(Close));
#pragma warning restore 4014

                // fire other Closing events too
                base.OnClosing(e);
            }
        }

        private void CanGoToPage(object sender, CanExecuteRoutedEventArgs e)
        {
            // GEP: I can't think of any reason that we would want to prevent going to the linked
            // page, so just going to default this to returning true
            e.CanExecute = true;
        }

        private void PerformGoToPage(object sender, ExecutedRoutedEventArgs e)
        {
            // https://github.com/theunrepentantgeek/Markdown.XAML/issues/5
            Process.Start(new ProcessStartInfo(e.Parameter.ToString()));
            e.Handled = true;
        }
    }
}