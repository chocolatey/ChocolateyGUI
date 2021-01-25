// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShellViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Providers;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Windows.Commands;
using ChocolateyGui.Common.Windows.Utilities;
using ChocolateyGui.Common.Windows.ViewModels.Items;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive,
        IHandle<ShowPackageDetailsMessage>,
        IHandle<ShowSourcesMessage>,
        IHandle<ShowSettingsMessage>,
        IHandle<ShowAboutMessage>,
        IHandle<SettingsGoBackMessage>,
        IHandle<AboutGoBackMessage>,
        IHandle<FeatureModifiedMessage>
    {
        private readonly IChocolateyService _chocolateyPackageService;
        private readonly IVersionNumberProvider _versionNumberProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly SourcesViewModel _sourcesViewModel;
        private readonly IConfigService _configService;
        private object _lastActiveItem;

        public ShellViewModel(
            IChocolateyService chocolateyPackageService,
            IVersionNumberProvider versionNumberProvider,
            IEventAggregator eventAggregator,
            SourcesViewModel sourcesViewModel,
            IConfigService configService)
        {
            _chocolateyPackageService = chocolateyPackageService;
            _versionNumberProvider = versionNumberProvider;
            _eventAggregator = eventAggregator;
            _sourcesViewModel = sourcesViewModel;
            _configService = configService;
            Sources = new BindableCollection<SourceViewModel>();
            ActiveItem = _sourcesViewModel;
            GoToSourceCommand = new RelayCommand(GoToSource, CanGoToSource);
        }

        public ICommand GoToSourceCommand { get; }

        public string AboutInformation
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.ABOUT.md");

        public string ReleaseNotes
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.CHANGELOG.md");

        public string Credits
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.CREDITS.md");

        public string VersionNumber => _versionNumberProvider.Version;

        public BindableCollection<SourceViewModel> Sources { get; set; }

        public SourcesViewModel SourcesSelectorViewModel => _sourcesViewModel;

        public virtual bool CanShowSettings
        {
            get { return true; }
        }

        public void Handle(ShowPackageDetailsMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Package == null)
            {
                throw new ArgumentNullException(nameof(message.Package));
            }

            var packageViewModel = ActiveItem as PackageViewModel;
            if (packageViewModel != null && packageViewModel.Package.Id == message.Package.Id)
            {
                return;
            }

            var packageVm = IoC.Get<PackageViewModel>();
            packageVm.Package = message.Package;
            SetActiveItem(packageVm);
        }

        public void Handle(ShowSourcesMessage message)
        {
            SetActiveItem(_sourcesViewModel);
        }

        public void Handle(ShowSettingsMessage message)
        {
            ShowSettings();
        }

        public void Handle(ShowAboutMessage message)
        {
            ShowAbout();
        }

        public void Handle(SettingsGoBackMessage message)
        {
            SetActiveItem(_sourcesViewModel);
        }

        public void Handle(AboutGoBackMessage message)
        {
            SetActiveItem(_sourcesViewModel);
        }

        public void Handle(FeatureModifiedMessage message)
        {
            NotifyOfPropertyChange(nameof(CanShowSettings));
        }

        public void ShowSettings()
        {
            if (ActiveItem is SettingsViewModel)
            {
                return;
            }

            SetActiveItem(IoC.Get<SettingsViewModel>());
        }

        public void ShowAbout()
        {
            if (ActiveItem is AboutViewModel)
            {
                return;
            }

            SetActiveItem(IoC.Get<AboutViewModel>());
        }

        protected override void OnInitialize()
        {
            _eventAggregator.Subscribe(this);
        }

        private bool CanGoToSource(object obj)
        {
            if (!_configService.GetEffectiveConfiguration().UseKeyboardBindings ?? true)
            {
                return false;
            }

            var sourceIndex = obj as int?;
            return sourceIndex.HasValue && sourceIndex > 0 && sourceIndex <= _sourcesViewModel.Items.Count;
        }

        private void GoToSource(object obj)
        {
            var sourceIndex = obj as int?;
            if (sourceIndex.HasValue)
            {
                --sourceIndex;

                if (sourceIndex < 0 || sourceIndex > _sourcesViewModel.Items.Count)
                {
                    return;
                }

                _sourcesViewModel.ActivateItem(_sourcesViewModel.Items[sourceIndex.Value]);
            }
        }

        private void SetActiveItem<T>(T newItem)
        {
            if (_lastActiveItem != null && _lastActiveItem.Equals(newItem))
            {
                _lastActiveItem = null;
            }
            else
            {
                _lastActiveItem = ActiveItem;
            }

            ActivateItem(newItem);
            if (_lastActiveItem is PackageViewModel)
            {
                this.CloseItem(_lastActiveItem);
            }
        }

        private async void GetSources()
        {
            var sources =
                (await _chocolateyPackageService.GetSources()).Select(
                    source => new SourceViewModel { Name = source.Id, Url = source.Value });
            Sources.AddRange(sources);
        }
    }
}