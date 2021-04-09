// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SettingsViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using chocolatey.infrastructure.filesystem;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.Theming;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using MahApps.Metro.Controls.Dialogs;
using ChocolateySource = ChocolateyGui.Common.Models.ChocolateySource;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public sealed class SettingsViewModel : Screen
    {
        private const string ChocolateyLicensedSourceId = "chocolatey.licensed";
        private readonly IChocolateyService _chocolateyService;

        private readonly IProgressService _progressService;
        private readonly IConfigService _configService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;
        private readonly IFileSystem _fileSystem;

        private Subject<ChocolateyFeature> _changedChocolateyFeature;
        private Subject<ChocolateySetting> _changedChocolateySetting;
        private Subject<ChocolateyGuiFeature> _changedChocolateyGuiFeature;
        private Subject<ChocolateyGuiSetting> _changedChocolateyGuiSetting;
        private AppConfiguration _config;
        private ChocolateySource _selectedSource;
        private ChocolateySource _draftSource;
        private string _originalId;
        private bool _isNewItem;
        private string _chocolateyGuiFeatureSearchQuery;
        private string _chocolateyGuiSettingSearchQuery;
        private string _chocolateyFeatureSearchQuery;
        private string _chocolateySettingsSearchQuery;

        public SettingsViewModel(
            IChocolateyService chocolateyService,
            IProgressService progressService,
            IConfigService configService,
            IEventAggregator eventAggregator,
            IDialogCoordinator dialogCoordinator,
            IChocolateyGuiCacheService chocolateyGuiCacheService,
            IFileSystem fileSystem)
        {
            _chocolateyService = chocolateyService;
            _progressService = progressService;
            _configService = configService;
            _eventAggregator = eventAggregator;
            _dialogCoordinator = dialogCoordinator;
            _chocolateyGuiCacheService = chocolateyGuiCacheService;
            _fileSystem = fileSystem;
            DisplayName = Resources.SettingsViewModel_DisplayName;
            Activated += OnActivated;
            Deactivated += OnDeactivated;

            ChocolateyGuiFeaturesView.Filter = new Predicate<object>(o => FilterChocolateyGuiFeatures(o as ChocolateyGuiFeature));
            ChocolateyGuiSettingsView.Filter = new Predicate<object>(o => FilterChocolateyGuiSettings(o as ChocolateyGuiSetting));
            ChocolateyFeaturesView.Filter = new Predicate<object>(o => FilterChocolateyFeatures(o as ChocolateyFeature));
            ChocolateySettingsView.Filter = new Predicate<object>(o => FilterChocolateySettings(o as ChocolateySetting));
        }

        public string ChocolateyGuiFeatureSearchQuery
        {
            get
            {
                return _chocolateyGuiFeatureSearchQuery;
            }

            set
            {
                _chocolateyGuiFeatureSearchQuery = value;
                NotifyOfPropertyChange(nameof(ChocolateyGuiFeatureSearchQuery));
                ChocolateyGuiFeaturesView.Refresh();
            }
        }

        public string ChocolateyGuiSettingSearchQuery
        {
            get
            {
                return _chocolateyGuiSettingSearchQuery;
            }

            set
            {
                _chocolateyGuiSettingSearchQuery = value;
                NotifyOfPropertyChange(nameof(ChocolateyGuiSettingSearchQuery));
                ChocolateyGuiSettingsView.Refresh();
            }
        }

        public string ChocolateyFeatureSearchQuery
        {
            get
            {
                return _chocolateyFeatureSearchQuery;
            }

            set
            {
                _chocolateyFeatureSearchQuery = value;
                NotifyOfPropertyChange(nameof(ChocolateyFeatureSearchQuery));
                ChocolateyFeaturesView.Refresh();
            }
        }

        public string ChocolateySettingSearchQuery
        {
            get
            {
                return _chocolateySettingsSearchQuery;
            }

            set
            {
                _chocolateySettingsSearchQuery = value;
                NotifyOfPropertyChange(nameof(ChocolateySettingSearchQuery));
                ChocolateySettingsView.Refresh();
            }
        }

        public ObservableCollection<ChocolateyGuiFeature> ChocolateyGuiFeatures { get; } = new ObservableCollection<ChocolateyGuiFeature>();

        public ObservableCollection<ChocolateyGuiSetting> ChocolateyGuiSettings { get; } = new ObservableCollection<ChocolateyGuiSetting>();

        public ObservableCollection<ChocolateyFeature> ChocolateyFeatures { get; } = new ObservableCollection<ChocolateyFeature>();

        public ObservableCollection<ChocolateySetting> ChocolateySettings { get; } = new ObservableCollection<ChocolateySetting>();

        public ObservableCollection<ChocolateySource> Sources { get; } = new ObservableCollection<ChocolateySource>();

        public ICollectionView ChocolateyGuiFeaturesView
        {
            get { return CollectionViewSource.GetDefaultView(ChocolateyGuiFeatures); }
        }

        public ICollectionView ChocolateyGuiSettingsView
        {
            get { return CollectionViewSource.GetDefaultView(ChocolateyGuiSettings); }
        }

        public ICollectionView ChocolateyFeaturesView
        {
            get { return CollectionViewSource.GetDefaultView(ChocolateyFeatures); }
        }

        public ICollectionView ChocolateySettingsView
        {
            get { return CollectionViewSource.GetDefaultView(ChocolateySettings); }
        }

        public bool CanSave => SelectedSource != null;

        public bool CanRemove => SelectedSource != null && !_isNewItem && SelectedSource.Id != ChocolateyLicensedSourceId;

        public bool CanCancel => SelectedSource != null;

        public ChocolateySource SelectedSource
        {
            get
            {
                return _selectedSource;
            }

            set
            {
                this.SetPropertyValue(ref _selectedSource, value);
                if (value != null && value.Id == null)
                {
                    _isNewItem = true;
                }
                else
                {
                    _isNewItem = false;
                    _originalId = value?.Id;
                }

                DraftSource = value == null ? null : new ChocolateySource(value);
                NotifyOfPropertyChange(nameof(CanSave));
                NotifyOfPropertyChange(nameof(CanRemove));
                NotifyOfPropertyChange(nameof(CanCancel));
                NotifyOfPropertyChange(nameof(IsSourceEditable));
                NotifyOfPropertyChange(nameof(IsChocolateyLicensedSource));
            }
        }

        public ChocolateySource DraftSource
        {
            get
            {
                return _draftSource;
            }

            set
            {
                this.SetPropertyValue(ref _draftSource, value);
            }
        }

        public bool IsSourceEditable
        {
            get { return DraftSource != null && DraftSource.Id != ChocolateyLicensedSourceId; }
        }

        public bool IsChocolateyLicensedSource
        {
            get { return DraftSource != null && DraftSource.Id == ChocolateyLicensedSourceId; }
        }

        public void ChocolateyFeatureToggled(ChocolateyFeature feature)
        {
            _changedChocolateyFeature.OnNext(feature);
        }

        public void ChocolateyGuiFeatureToggled(ChocolateyGuiFeature feature)
        {
            _changedChocolateyGuiFeature.OnNext(feature);
        }

        public async void ChocolateySettingsRowEditEnding(DataGridRowEditEndingEventArgs eventArgs)
        {
            await Task.Delay(100);
            _changedChocolateySetting.OnNext((ChocolateySetting)eventArgs.Row.Item);
        }

        public async void ChocolateyGuiSettingsRowEditEnding(DataGridRowEditEndingEventArgs eventArgs)
        {
            await Task.Delay(100);
            _changedChocolateyGuiSetting.OnNext((ChocolateyGuiSetting)eventArgs.Row.Item);
        }

        public void SourceSelectionChanged(object source)
        {
            var sourceItem = (ChocolateySource)source;
            SelectedSource = sourceItem;
            return;
        }

        public async Task UpdateChocolateyGuiFeature(ChocolateyGuiFeature feature)
        {
            var configuration = new ChocolateyGuiConfiguration();
            configuration.CommandName = "feature";
            configuration.FeatureCommand.Name = feature.Title;

            if (feature.Enabled)
            {
                configuration.FeatureCommand.Command = FeatureCommandType.Enable;
                await Task.Run(() => _configService.ToggleFeature(configuration, true));
            }
            else
            {
                configuration.FeatureCommand.Command = FeatureCommandType.Disable;
                await Task.Run(() => _configService.ToggleFeature(configuration, false));
            }

            _eventAggregator.PublishOnUIThread(new FeatureModifiedMessage());

            if (feature.Title == "ShowAggregatedSourceView")
            {
                await _eventAggregator.PublishOnUIThreadAsync(new SourcesUpdatedMessage());
            }

            if (feature.Title == "DefaultToDarkMode")
            {
                ThemeAssist.BundledTheme.IsLightTheme = !feature.Enabled;
                ThemeAssist.BundledTheme.ToggleTheme.Execute(null);
            }
        }

        public async Task UpdateChocolateyGuiSetting(ChocolateyGuiSetting setting)
        {
            var configuration = new ChocolateyGuiConfiguration();
            configuration.CommandName = "config";
            configuration.ConfigCommand.Name = setting.Key;
            configuration.ConfigCommand.ConfigValue = setting.Value;

            await Task.Run(() => _configService.SetConfigValue(configuration));
        }

        public async Task UpdateChocolateyFeature(ChocolateyFeature feature)
        {
            try
            {
                await _chocolateyService.SetFeature(feature);
            }
            catch (UnauthorizedAccessException)
            {
                await _progressService.ShowMessageAsync(
                    Resources.General_UnauthorisedException_Title,
                    Resources.General_UnauthorisedException_Description);
            }
        }

        public async Task UpdateChocolateySetting(ChocolateySetting setting)
        {
            await _chocolateyService.SetSetting(setting);
        }

        public async Task PurgeIconCache()
        {
            var result = await _dialogCoordinator.ShowMessageAsync(
                this,
                Resources.Dialog_AreYouSureTitle,
                Resources.Dialog_AreYouSureIconsMessage,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = Resources.Dialog_Yes,
                    NegativeButtonText = Resources.Dialog_No
                });

            if (result == MessageDialogResult.Affirmative)
            {
                _chocolateyGuiCacheService.PurgeIcons();
            }
        }

        public async Task PurgeOutdatedPackagesCache()
        {
            var result = await _dialogCoordinator.ShowMessageAsync(
                this,
                Resources.Dialog_AreYouSureTitle,
                Resources.Dialog_AreYouSureOutdatedPackagesMessage,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = Resources.Dialog_Yes,
                    NegativeButtonText = Resources.Dialog_No
                });

            if (result == MessageDialogResult.Affirmative)
            {
                _chocolateyGuiCacheService.PurgeOutdatedPackages();
            }
        }

        public void New()
        {
            SelectedSource = new ChocolateySource();
        }

        public async void Save()
        {
            if (string.IsNullOrWhiteSpace(DraftSource.Id))
            {
                await _progressService.ShowMessageAsync(Resources.SettingsViewModel_SavingSource, Resources.SettingsViewModel_SourceMissingId);
                return;
            }

            if (string.IsNullOrWhiteSpace(DraftSource.Value))
            {
                await _progressService.ShowMessageAsync(Resources.SettingsViewModel_SavingSource, Resources.SettingsViewModel_SourceMissingValue);
                return;
            }

            await _progressService.StartLoading(Resources.SettingsViewModel_SavingSourceLoading);
            try
            {
                if (_isNewItem)
                {
                    if (DraftSource.Id == ChocolateyLicensedSourceId)
                    {
                        await _progressService.StopLoading();
                        await _progressService.ShowMessageAsync(Resources.SettingsViewModel_SavingSource, Resources.SettingsViewModel_InvalidSourceId);
                        return;
                    }

                    await _chocolateyService.AddSource(DraftSource);
                    _isNewItem = false;
                    Sources.Add(DraftSource);
                    NotifyOfPropertyChange(nameof(CanRemove));
                }
                else
                {
                    if (DraftSource.Id == ChocolateyLicensedSourceId)
                    {
                        if (DraftSource.Disabled)
                        {
                            await _chocolateyService.DisableSource(DraftSource.Id);
                        }
                        else
                        {
                            await _chocolateyService.EnableSource(DraftSource.Id);
                        }
                    }
                    else
                    {
                        await _chocolateyService.UpdateSource(_originalId, DraftSource);
                    }

                    Sources[Sources.IndexOf(SelectedSource)] = DraftSource;
                }

                _originalId = DraftSource?.Id;
                await _eventAggregator.PublishOnUIThreadAsync(new SourcesUpdatedMessage());
            }
            catch (UnauthorizedAccessException)
            {
                await _progressService.ShowMessageAsync(
                    Resources.General_UnauthorisedException_Title,
                    Resources.General_UnauthorisedException_Description);
            }
            finally
            {
                SelectedSource = null;
                await _progressService.StopLoading();
            }
        }

        public async void Remove()
        {
            var result = await _dialogCoordinator.ShowMessageAsync(
                this,
                Resources.Dialog_AreYouSureTitle,
                string.Format(Resources.Dialog_AreYourSureRemoveSourceMessage, _originalId),
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = Resources.Dialog_Yes,
                    NegativeButtonText = Resources.Dialog_No
                });

            if (result == MessageDialogResult.Affirmative)
            {
                await _progressService.StartLoading(Resources.SettingsViewModel_RemovingSource);
                try
                {
                    await _chocolateyService.RemoveSource(_originalId);
                    Sources.Remove(SelectedSource);
                    SelectedSource = null;
                    await _eventAggregator.PublishOnUIThreadAsync(new SourcesUpdatedMessage());
                }
                catch (UnauthorizedAccessException)
                {
                    await _progressService.ShowMessageAsync(
                        Resources.General_UnauthorisedException_Title,
                        Resources.General_UnauthorisedException_Description);
                }
                finally
                {
                    await _progressService.StopLoading();
                }
            }
        }

        public void Cancel()
        {
            DraftSource = new ChocolateySource(SelectedSource);
        }

        public void Back()
        {
            _eventAggregator.PublishOnUIThread(new SettingsGoBackMessage());
        }

        public async void SetUserAndPassword()
        {
            var loginDialogSettings = new LoginDialogSettings
            {
                AffirmativeButtonText = Resources.SettingsView_ButtonSave,
                NegativeButtonText = Resources.SettingsView_ButtonCancel,
                NegativeButtonVisibility = Visibility.Visible,
                InitialUsername = DraftSource.UserName,
                InitialPassword = DraftSource.Password
            };

            // Only allow the previewing of a password when creating a new source
            // not when modifying an existing source
            if (_isNewItem)
            {
                loginDialogSettings.EnablePasswordPreview = true;
            }

            var result = await _dialogCoordinator.ShowLoginAsync(this, Resources.SettingsViewModel_SetSourceUsernameAndPasswordTitle, Resources.SettingsViewModel_SetSourceUsernameAndPasswordMessage, loginDialogSettings);

            if (result != null)
            {
                DraftSource.UserName = result.Username;
                DraftSource.Password = result.Password;
                NotifyOfPropertyChange(nameof(DraftSource));
            }
        }

        public async void SetCertificateAndPassword()
        {
            var loginDialogSettings = new LoginDialogSettings
            {
                AffirmativeButtonText = Resources.SettingsView_ButtonSave,
                NegativeButtonText = Resources.SettingsView_ButtonCancel,
                NegativeButtonVisibility = Visibility.Visible,
                InitialUsername = DraftSource.Certificate,
                InitialPassword = DraftSource.CertificatePassword,
                UsernameWatermark = Resources.SettingsViewModel_SetSourceCertificateAndPasswordUsernameWatermark,
                PasswordWatermark = Resources.SettingsViewModel_SetSourceCertificateAndPasswordPasswordWatermark
            };

            // Only allow the previewing of a password when creating a new source
            // not when modifying an existing source
            if (_isNewItem)
            {
                loginDialogSettings.EnablePasswordPreview = true;
            }

            var result = await _dialogCoordinator.ShowLoginAsync(this, Resources.SettingsViewModel_SetSourceCertificateAndPasswordTitle, Resources.SettingsViewModel_SetSourceCertificateAndPasswordMessage, loginDialogSettings);

            if (result != null)
            {
                DraftSource.Certificate = result.Username;
                DraftSource.CertificatePassword = result.Password;
                NotifyOfPropertyChange(nameof(DraftSource));
            }
        }

        private async void OnActivated(object sender, ActivationEventArgs activationEventArgs)
        {
            _config = _configService.GetEffectiveConfiguration();

            var chocolateyFeatures = await _chocolateyService.GetFeatures();
            foreach (var chocolateyFeature in chocolateyFeatures)
            {
                ChocolateyFeatures.Add(chocolateyFeature);
            }

            _changedChocolateyFeature = new Subject<ChocolateyFeature>();
            _changedChocolateyFeature
                        .Select(f => Observable.FromAsync(() => UpdateChocolateyFeature(f)))
                        .Concat()
                        .Subscribe();

            var chocolateySettings = await _chocolateyService.GetSettings();
            foreach (var chocolateySetting in chocolateySettings)
            {
                ChocolateySettings.Add(chocolateySetting);
            }

            _changedChocolateySetting = new Subject<ChocolateySetting>();
            _changedChocolateySetting
                        .Select(s => Observable.FromAsync(() => UpdateChocolateySetting(s)))
                        .Concat()
                        .Subscribe();

            var chocolateyGuiFeatures = _configService.GetFeatures(global: false);
            foreach (var chocolateyGuiFeature in chocolateyGuiFeatures)
            {
                ChocolateyGuiFeatures.Add(chocolateyGuiFeature);
            }

            _changedChocolateyGuiFeature = new Subject<ChocolateyGuiFeature>();
            _changedChocolateyGuiFeature
                .Select(s => Observable.FromAsync(() => UpdateChocolateyGuiFeature(s)))
                .Concat()
                .Subscribe();

            var chocolateyGuiSettings = _configService.GetSettings(global: false);
            foreach (var chocolateyGuiSetting in chocolateyGuiSettings)
            {
                ChocolateyGuiSettings.Add(chocolateyGuiSetting);
            }

            _changedChocolateyGuiSetting = new Subject<ChocolateyGuiSetting>();
            _changedChocolateyGuiSetting
                .Select(s => Observable.FromAsync(() => UpdateChocolateyGuiSetting(s)))
                .Concat()
                .Subscribe();

            var sources = await _chocolateyService.GetSources();
            foreach (var source in sources)
            {
                Sources.Add(source);
            }
        }

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            _changedChocolateyFeature.OnCompleted();
            _changedChocolateySetting.OnCompleted();
            _changedChocolateyGuiFeature.OnCompleted();
            _changedChocolateyGuiSetting.OnCompleted();
        }

        private bool FilterChocolateyGuiFeatures(ChocolateyGuiFeature chocolateyGuiFeature)
        {
            return ChocolateyGuiFeatureSearchQuery == null
                   || chocolateyGuiFeature.Title.IndexOf(ChocolateyGuiFeatureSearchQuery, StringComparison.OrdinalIgnoreCase) != -1;
        }

        private bool FilterChocolateyGuiSettings(ChocolateyGuiSetting chocolateyGuiSetting)
        {
            return ChocolateyGuiSettingSearchQuery == null
                   || chocolateyGuiSetting.Key.IndexOf(ChocolateyGuiSettingSearchQuery, StringComparison.OrdinalIgnoreCase) != -1;
        }

        private bool FilterChocolateyFeatures(ChocolateyFeature chocolateyFeature)
        {
            return ChocolateyFeatureSearchQuery == null
                   || chocolateyFeature.Name.IndexOf(ChocolateyFeatureSearchQuery, StringComparison.OrdinalIgnoreCase) != -1;
        }

        private bool FilterChocolateySettings(ChocolateySetting chocolateySetting)
        {
            return ChocolateySettingSearchQuery == null
                   || chocolateySetting.Key.IndexOf(ChocolateySettingSearchQuery, StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
}