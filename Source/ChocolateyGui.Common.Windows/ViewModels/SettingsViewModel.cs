// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SettingsViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
using ChocolateyGui.Common.Utilities;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.Startup;
using ChocolateyGui.Common.Windows.Theming;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using MahApps.Metro.Controls.Dialogs;
using ChocolateySource = ChocolateyGui.Common.Models.ChocolateySource;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public sealed class SettingsViewModel : ViewModelScreen
    {
        private const string ChocolateyLicensedSourceId = "chocolatey.licensed";
        private readonly IChocolateyService _chocolateyService;

        private readonly IDialogService _dialogService;
        private readonly IProgressService _progressService;
        private readonly IConfigService _configService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;
        private readonly IFileSystem _fileSystem;

        private Subject<ChocolateyFeature> _changedChocolateyFeature;
        private Subject<ChocolateySetting> _changedChocolateySetting;
        private Subject<ChocolateyGuiFeature> _changedChocolateyGuiFeature;
        private Subject<ChocolateyGuiSetting> _changedChocolateyGuiSetting;
        private AppConfiguration _config;
        private ChocolateySource _selectedSource;
        private ChocolateySource _draftSource;
        private TranslationSource _translationSource;
        private string _originalId;
        private bool _isNewItem;
        private string _chocolateyGuiFeatureSearchQuery;
        private string _chocolateyGuiSettingSearchQuery;
        private string _chocolateyFeatureSearchQuery;
        private string _chocolateySettingsSearchQuery;

        public SettingsViewModel(
            IChocolateyService chocolateyService,
            IDialogService dialogService,
            IProgressService progressService,
            IConfigService configService,
            IEventAggregator eventAggregator,
            IChocolateyGuiCacheService chocolateyGuiCacheService,
            IFileSystem fileSystem,
            TranslationSource translationSource)
            : base(translationSource)
        {
            _chocolateyService = chocolateyService;
            _dialogService = dialogService;
            _progressService = progressService;
            _configService = configService;
            _eventAggregator = eventAggregator;
            _chocolateyGuiCacheService = chocolateyGuiCacheService;
            _fileSystem = fileSystem;
            _translationSource = translationSource;
            DisplayName = L(nameof(Resources.SettingsViewModel_DisplayName));
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

        public CultureInfo UseLanguage
        {
            get
            {
                if (string.IsNullOrEmpty(_config.UseLanguage))
                {
                    return Internationalization.GetFallbackCulture();
                }
                else
                {
                    return new CultureInfo(_config.UseLanguage);
                }
            }

            set
            {
                _config.UseLanguage = value.Name;
                NotifyOfPropertyChange();
                Internationalization.UpdateLanguage(value.Name);

                // We explicitly update settings when the language changes
                _configService.SetConfigValue(nameof(UseLanguage), value.Name);
                ChocolateyGuiFeaturesView.Refresh();
                ChocolateyGuiSettingsView.Refresh();
            }
        }

        public ObservableCollection<CultureInfo> AllLanguages { get; private set; } =
            new ObservableCollection<CultureInfo>();

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
            // When the flow direction gets changed, this results in the feature
            // being null some times immediately. As such, if the feature is null
            // then just return so we don't encounter an exception.
            if (feature == null)
            {
                return;
            }

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
                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.General_UnauthorisedException_Title)),
                    L(nameof(Resources.General_UnauthorisedException_Description)));
            }
        }

        public async Task UpdateChocolateySetting(ChocolateySetting setting)
        {
            await _chocolateyService.SetSetting(setting);
        }

        public async Task PurgeIconCache()
        {
            var result = MessageDialogResult.Affirmative;
            if (!_config.SkipModalDialogConfirmation.GetValueOrDefault(false))
            {
                result = await _dialogService.ShowConfirmationMessageAsync(
                    L(nameof(Resources.Dialog_AreYouSureTitle)),
                    L(nameof(Resources.Dialog_AreYouSureIconsMessage)));
            }

            if (result == MessageDialogResult.Affirmative)
            {
                _chocolateyGuiCacheService.PurgeIcons();
            }
        }

        public async Task PurgeOutdatedPackagesCache()
        {
            var result = MessageDialogResult.Affirmative;
            if (!_config.SkipModalDialogConfirmation.GetValueOrDefault(false))
            {
                result = await _dialogService.ShowConfirmationMessageAsync(
                    L(nameof(Resources.Dialog_AreYouSureTitle)),
                    L(nameof(Resources.Dialog_AreYouSureOutdatedPackagesMessage)));
            }

            if (result == MessageDialogResult.Affirmative)
            {
                _chocolateyGuiCacheService.PurgeOutdatedPackages();
            }
        }

        public void New()
        {
            SelectedSource = new ChocolateySource();
        }

        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(DraftSource.Id))
            {
                await _dialogService.ShowMessageAsync(L(nameof(Resources.SettingsViewModel_SavingSource)), L(nameof(Resources.SettingsViewModel_SourceMissingId)));
                return;
            }

            if (string.IsNullOrWhiteSpace(DraftSource.Value))
            {
                await _dialogService.ShowMessageAsync(L(nameof(Resources.SettingsViewModel_SavingSource)), L(nameof(Resources.SettingsViewModel_SourceMissingValue)));
                return;
            }

            await _progressService.StartLoading(L(nameof(Resources.SettingsViewModel_SavingSourceLoading)));
            try
            {
                if (_isNewItem)
                {
                    if (DraftSource.Id == ChocolateyLicensedSourceId)
                    {
                        await _progressService.StopLoading();
                        await _dialogService.ShowMessageAsync(L(nameof(Resources.SettingsViewModel_SavingSource)), L(nameof(Resources.SettingsViewModel_InvalidSourceId)));
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
                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.General_UnauthorisedException_Title)),
                    L(nameof(Resources.General_UnauthorisedException_Description)));
            }
            finally
            {
                SelectedSource = null;
                await _progressService.StopLoading();
            }
        }

        public async Task Remove()
        {
            var result = MessageDialogResult.Affirmative;
            if (!_config.SkipModalDialogConfirmation.GetValueOrDefault(false))
            {
                result = await _dialogService.ShowConfirmationMessageAsync(
                    L(nameof(Resources.Dialog_AreYouSureTitle)),
                    L(nameof(Resources.Dialog_AreYourSureRemoveSourceMessage), _originalId));
            }

            if (result == MessageDialogResult.Affirmative)
            {
                await _progressService.StartLoading(L(nameof(Resources.SettingsViewModel_RemovingSource)));
                try
                {
                    await _chocolateyService.RemoveSource(_originalId);
                    Sources.Remove(SelectedSource);
                    SelectedSource = null;
                    await _eventAggregator.PublishOnUIThreadAsync(new SourcesUpdatedMessage());
                }
                catch (UnauthorizedAccessException)
                {
                    await _dialogService.ShowMessageAsync(
                        L(nameof(Resources.General_UnauthorisedException_Title)),
                        L(nameof(Resources.General_UnauthorisedException_Description)));
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
                AffirmativeButtonText = L(nameof(Resources.SettingsView_ButtonSave)),
                NegativeButtonText = L(nameof(Resources.SettingsView_ButtonCancel)),
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

            var result = await _dialogService.ShowLoginAsync(L(nameof(Resources.SettingsViewModel_SetSourceUsernameAndPasswordTitle)), L(nameof(Resources.SettingsViewModel_SetSourceUsernameAndPasswordMessage)), loginDialogSettings);

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
                AffirmativeButtonText = L(nameof(Resources.SettingsView_ButtonSave)),
                NegativeButtonText = L(nameof(Resources.SettingsView_ButtonCancel)),
                NegativeButtonVisibility = Visibility.Visible,
                InitialUsername = DraftSource.Certificate,
                InitialPassword = DraftSource.CertificatePassword,
                UsernameWatermark = L(nameof(Resources.SettingsViewModel_SetSourceCertificateAndPasswordUsernameWatermark)),
                PasswordWatermark = L(nameof(Resources.SettingsViewModel_SetSourceCertificateAndPasswordPasswordWatermark))
            };

            // Only allow the previewing of a password when creating a new source
            // not when modifying an existing source
            if (_isNewItem)
            {
                loginDialogSettings.EnablePasswordPreview = true;
            }

            var result = await _dialogService.ShowLoginAsync(L(nameof(Resources.SettingsViewModel_SetSourceCertificateAndPasswordTitle)), L(nameof(Resources.SettingsViewModel_SetSourceCertificateAndPasswordMessage)), loginDialogSettings);

            if (result != null)
            {
                DraftSource.Certificate = result.Username;
                DraftSource.CertificatePassword = result.Password;
                NotifyOfPropertyChange(nameof(DraftSource));
            }
        }

        protected override void OnLanguageChanged()
        {
            DisplayName = L(nameof(Resources.SettingsViewModel_DisplayName));
        }

        private async void OnActivated(object sender, ActivationEventArgs activationEventArgs)
        {
            _config = _configService.GetEffectiveConfiguration();

            var chocolateyFeatures = await _chocolateyService.GetFeatures();
            foreach (var chocolateyFeature in chocolateyFeatures)
            {
#if !DEBUG // We hide this during DEBUG as it is a dark feature
                var descriptionKey = "Chocolatey_" + chocolateyFeature.Name + "Description";

                var newDescription = _translationSource[descriptionKey];

                if (string.IsNullOrEmpty(newDescription))
                {
                    descriptionKey = chocolateyFeature.Description;
                    newDescription = _translationSource[descriptionKey];
                }

                if (!string.IsNullOrEmpty(newDescription))
                {
                    chocolateyFeature.Description = newDescription;
                    _translationSource.PropertyChanged += (s, e) =>
                    {
                        chocolateyFeature.Description = _translationSource[descriptionKey];
                    };
                }
#endif
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
#if !DEBUG // We hide this during DEBUG as it is a dark feature
                var descriptionKey = "Chocolatey_" + chocolateySetting.Key + "Description";

                var newDescription = _translationSource[descriptionKey];

                if (string.IsNullOrEmpty(newDescription))
                {
                    descriptionKey = chocolateySetting.Description;
                    newDescription = _translationSource[descriptionKey];
                }

                if (!string.IsNullOrEmpty(newDescription))
                {
                    chocolateySetting.Description = newDescription;
                    _translationSource.PropertyChanged += (s, e) =>
                    {
                        chocolateySetting.Description = _translationSource[descriptionKey];
                    };
                }
#endif
                ChocolateySettings.Add(chocolateySetting);
            }

            _changedChocolateySetting = new Subject<ChocolateySetting>();
            _changedChocolateySetting
                        .Select(s => Observable.FromAsync(() => UpdateChocolateySetting(s)))
                        .Concat()
                        .Subscribe();

            var chocolateyGuiFeatures = _configService.GetFeatures(global: false, useResourceKeys: true);
            foreach (var chocolateyGuiFeature in chocolateyGuiFeatures)
            {
                chocolateyGuiFeature.DisplayTitle = _translationSource["ChocolateyGUI_" + chocolateyGuiFeature.Title + "Title"];
#if DEBUG
                var descriptionKey = string.Empty;
#else
                var descriptionKey = "ChocolateyGUI_" + chocolateyGuiFeature.Title + "Description";
#endif

                var newDescription = _translationSource[descriptionKey];

                if (string.IsNullOrEmpty(newDescription))
                {
                    descriptionKey = chocolateyGuiFeature.Description;
                    newDescription = _translationSource[descriptionKey];
                }

                if (!string.IsNullOrEmpty(newDescription))
                {
                    chocolateyGuiFeature.Description = newDescription;
                    _translationSource.PropertyChanged += (s, e) =>
                    {
                        chocolateyGuiFeature.DisplayTitle = _translationSource["ChocolateyGUI_" + chocolateyGuiFeature.Title + "Title"];
                        chocolateyGuiFeature.Description = _translationSource[descriptionKey];
                    };
                }

                ChocolateyGuiFeatures.Add(chocolateyGuiFeature);
            }

            _changedChocolateyGuiFeature = new Subject<ChocolateyGuiFeature>();
            _changedChocolateyGuiFeature
                .Select(s => Observable.FromAsync(() => UpdateChocolateyGuiFeature(s)))
                .Concat()
                .Subscribe();

            var chocolateyGuiSettings = _configService.GetSettings(global: false, useResourceKeys: true);
            foreach (var chocolateyGuiSetting in chocolateyGuiSettings.Where(c => !string.Equals(c.Key, nameof(UseLanguage), StringComparison.OrdinalIgnoreCase)))
            {
                chocolateyGuiSetting.DisplayName = _translationSource["ChocolateyGUI_" + chocolateyGuiSetting.Key + "Title"];
#if DEBUG
                var descriptionKey = string.Empty;
#else
                var descriptionKey = "ChocolateyGUI_" + chocolateyGuiSetting.Key + "Description";
#endif

                var newDescription = _translationSource[descriptionKey];

                if (string.IsNullOrEmpty(newDescription))
                {
                    descriptionKey = chocolateyGuiSetting.Description;
                    newDescription = _translationSource[descriptionKey];
                }

                if (!string.IsNullOrEmpty(newDescription))
                {
                    chocolateyGuiSetting.Description = newDescription;
                    _translationSource.PropertyChanged += (s, e) =>
                    {
                        chocolateyGuiSetting.DisplayName =
                            _translationSource["ChocolateyGUI_" + chocolateyGuiSetting.Key + "Title"];
                        chocolateyGuiSetting.Description = _translationSource[descriptionKey];
                    };
                }

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

            AllLanguages.Clear();

            foreach (var language in Internationalization.GetAllSupportedCultures().OrderBy(c => c.NativeName))
            {
                AllLanguages.Add(language);
            }

            var selectedLanguage = _config.UseLanguage;

            // We set it to the configuration itself, instead of the property
            // as we do not want to save the configuration file when it is not needed.
            _config.UseLanguage = Internationalization.GetSupportedCultureInfo(selectedLanguage).Name;
            NotifyOfPropertyChange(nameof(UseLanguage));
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
                   || chocolateyGuiFeature.Title.IndexOf(ChocolateyGuiFeatureSearchQuery, StringComparison.OrdinalIgnoreCase) != -1
                   || chocolateyGuiFeature.DisplayTitle.IndexOf(ChocolateyGuiFeatureSearchQuery, StringComparison.OrdinalIgnoreCase) != -1;
        }

        private bool FilterChocolateyGuiSettings(ChocolateyGuiSetting chocolateyGuiSetting)
        {
            return ChocolateyGuiSettingSearchQuery == null
                   || chocolateyGuiSetting.Key.IndexOf(ChocolateyGuiSettingSearchQuery, StringComparison.OrdinalIgnoreCase) != -1
                   || chocolateyGuiSetting.DisplayName.IndexOf(ChocolateyGuiSettingSearchQuery, StringComparison.OrdinalIgnoreCase) != -1;
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