// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShellView.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using CefSharp;
using ChocolateyGui.Controls.Dialogs;
using ChocolateyGui.Providers;
using ChocolateyGui.Services;
using MahApps.Metro.Controls.Dialogs;

namespace ChocolateyGui.Views
{
    /// <summary>
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView
    {
        private readonly IChocolateyConfigurationProvider _chocolateyConfigurationProvider;
        private readonly IProgressService _progressService;

        public ShellView(IProgressService progressService,
            IChocolateyConfigurationProvider chocolateyConfigurationProvider)
        {
            InitializeComponent();

            var service = progressService as ProgressService;
            if (service != null)
            {
                service.ShellView = this;
            }

            _progressService = progressService;
            _chocolateyConfigurationProvider = chocolateyConfigurationProvider;

            CheckOperatingSystemCompatibility();

            var settings = new CefSettings();
            settings.RegisterScheme(new CefCustomScheme { SchemeName = ChocolateyCustomSchemeProvider.SchemeName, SchemeHandlerFactory = new ChocolateyCustomSchemeProvider() });
            settings.CefCommandLineArgs.Add("no-proxy-server", "1");
            settings.CachePath = Path.Combine(Bootstrapper.LocalAppDataPath, "CefCache");
            settings.PackLoadingDisabled = true;
            settings.SetOffScreenRenderingBestPerformanceArgs();
            if (!Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null))
            {
                return;
            }
        }

        public void CheckOperatingSystemCompatibility()
        {
            var operatingSystemVersion = Environment.OSVersion;

            if (operatingSystemVersion.Version.Major == 10 &&
                !_chocolateyConfigurationProvider.IsChocolateyExecutableBeingUsed)
            {
                MessageBox.Show(
                    "Usage of the PowerShell Version of Chocolatey (i.e. <= 0.9.8.33) has been detected.  ChocolateyGUI does not support using this version of Chocolatey on Windows 10.  Please update Chocolatey to the new C# Version (i.e. > 0.9.9.0) and restart ChocolateyGUI.  This application will now close.",
                    "Incompatible Operating System Version",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.ServiceNotification);

                Application.Current.Shutdown();
            }
        }

        public Task<ChocolateyDialogController> ShowChocolateyDialogAsync(string title, bool isCancelable = false,
            MetroDialogSettings settings = null)
        {
            return Dispatcher.Invoke(async () =>
            {
                // create the dialog control
                var dialog = new ChocolateyDialog(this)
                {
                    Title = title,
                    IsCancelable = isCancelable,
                    OutputBufferCollection = _progressService.Output
                };

                if (settings == null)
                {
                    settings = MetroDialogOptions;
                }

                dialog.NegativeButtonText = settings.NegativeButtonText;

                await this.ShowMetroDialogAsync(dialog);
                return new ChocolateyDialogController(dialog, () => this.HideMetroDialogAsync(dialog));
            });
        }

        private bool _closeInitiated = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_closeInitiated)
            {
                e.Cancel = true;
                _closeInitiated = true;
                var bootstrapper = (Bootstrapper) Application.Current.FindResource("Bootstrapper");
#pragma warning disable 4014
                // ReSharper disable once PossibleNullReferenceException
                bootstrapper.OnExitAsync().ContinueWith(t => Execute.OnUIThread(Close));
#pragma warning restore 4014
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