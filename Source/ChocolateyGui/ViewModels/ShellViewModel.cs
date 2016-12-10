// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShellViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Providers;
using ChocolateyGui.Services;
using ChocolateyGui.Utilities;
using ChocolateyGui.ViewModels.Items;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;

namespace ChocolateyGui.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IHandle<ShowPackageDetailsMessage>, IHandle<ShowSourcesMessage>
    {
        private readonly IVersionNumberProvider _versionNumberProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgressService _progressService;
        private readonly SourcesViewModel _sourcesViewModel;

        public ShellViewModel(ISourceService sourceService,
            IVersionNumberProvider versionNumberProvider,
            IEventAggregator eventAggregator,
            IProgressService progressService,
            SourcesViewModel sourcesViewModel)
        {
            if (sourceService == null)
            {
                throw new ArgumentNullException(nameof(sourceService));
            }

            _versionNumberProvider = versionNumberProvider;
            _eventAggregator = eventAggregator;
            _progressService = progressService;
            _sourcesViewModel = sourcesViewModel;
            Sources = new BindableCollection<SourceViewModel>(sourceService.GetSources());
            ActiveItem = _sourcesViewModel;
        }

        public void Handle(ShowPackageDetailsMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            ActiveItem = new PackageViewModel(_eventAggregator) { Package = message.Package };
        }

        public void Handle(ShowSourcesMessage message)
        {
            ActiveItem = _sourcesViewModel;
        }

        public string AboutInformation
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.ABOUT.md");

        public string ReleaseNotes
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.CHANGELOG.md");

        public string Credits
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.CREDITS.md");

        public string VersionNumber => _versionNumberProvider.Version;

        public BindableCollection<SourceViewModel> Sources { get; set; }

        public SourcesViewModel SourcesSelectorViewModel => _sourcesViewModel;

        protected override async void OnInitialize()
        {
            _eventAggregator.Subscribe(this);

            if (Bootstrapper.Configuration.GetValue<bool>("FirstRun"))
            {
                Bootstrapper.UpdateSetting("FirstRun", "false");

                var result =
                    await _progressService.ShowMessageAsync("Administrator Elevation",
                        "Do you want to always elevate ChocolateyGUI to administrator by default?",
                        MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    Bootstrapper.UpdateSetting("RequireAdmin", "true");
                    if (!Privileged.IsElevated)
                    {
                        var rawArgs = Environment.CommandLine;
                        rawArgs = rawArgs.Remove(Environment.GetCommandLineArgs()[0].Length);
                        if (!Privileged.Elevate(rawArgs))
                        {
                            var result2 =
                                MessageBox.Show(
                                    "Failed to start application as administator. Would you like to continue as unelevated?",
                                    "Error Elevating", MessageBoxButton.YesNo);

                            if (result2 == MessageBoxResult.No)
                            {
                                Application.Current.Shutdown(1);
                                return;
                            }
                        }
                        else
                        {
                            Application.Current.Shutdown(0);
                        }
                    }
                }
            }
        }
    }
}