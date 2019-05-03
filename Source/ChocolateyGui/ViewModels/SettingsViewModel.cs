// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SettingsViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Controls;
using Caliburn.Micro;
using ChocolateyGui.CliCommands;
using ChocolateyGui.Models;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;
using ChocolateyGui.Utilities.Extensions;
using ChocolateySource = ChocolateyGui.Models.ChocolateySource;

namespace ChocolateyGui.ViewModels
{
    public sealed class SettingsViewModel : Screen
    {
        private readonly IChocolateyService _chocolateyService;
        private readonly IProgressService _progressService;
        private readonly IConfigService _configService;
        private readonly IEventAggregator _eventAggregator;

        private Subject<ChocolateyFeature> _changedChocolateyFeature;
        private Subject<ChocolateySetting> _changedChocolateySetting;
        private Subject<ChocolateyGuiFeature> _changedChocolateyGuiFeature;
        private Subject<ChocolateyGuiSetting> _changedChocolateyGuiSetting;
        private AppConfiguration _config;
        private ChocolateySource _selectedSource;
        private ChocolateySource _draftSource;
        private string _originalId;
        private bool _isNewItem;

        public SettingsViewModel(
            IChocolateyService chocolateyService,
            IProgressService progressService,
            IConfigService configService,
            IEventAggregator eventAggregator)
        {
            _chocolateyService = chocolateyService;
            _progressService = progressService;
            _configService = configService;
            _eventAggregator = eventAggregator;
            DisplayName = Resources.SettingsViewModel_DisplayName;
            Activated += OnActivated;
            Deactivated += OnDeactivated;
        }

        public ObservableCollection<ChocolateyGuiFeature> ChocolateyGuiFeatures { get; } = new ObservableCollection<ChocolateyGuiFeature>();

        public ObservableCollection<ChocolateyGuiSetting> ChocolateyGuiSettings { get; } = new ObservableCollection<ChocolateyGuiSetting>();

        public ObservableCollection<ChocolateyFeature> ChocolateyFeatures { get; } = new ObservableCollection<ChocolateyFeature>();

        public ObservableCollection<ChocolateySetting> ChocolateySettings { get; } = new ObservableCollection<ChocolateySetting>();

        public ObservableCollection<ChocolateySource> Sources { get; } = new ObservableCollection<ChocolateySource>();

        public bool CanSave => SelectedSource != null;

        public bool CanRemove => SelectedSource != null && !_isNewItem;

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
                await Task.Run(() => _configService.EnableFeature(configuration));
            }
            else
            {
                configuration.FeatureCommand.Command = FeatureCommandType.Disable;
                await Task.Run(() => _configService.DisableFeature(configuration));
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
                    await _chocolateyService.AddSource(DraftSource);
                    _isNewItem = false;
                    Sources.Add(DraftSource);
                    NotifyOfPropertyChange(nameof(CanRemove));
                }
                else
                {
                    await _chocolateyService.UpdateSource(_originalId, DraftSource);
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
                await _progressService.StopLoading();
            }
        }

        public async void Remove()
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

        public void Cancel()
        {
            DraftSource = new ChocolateySource(SelectedSource);
        }

        public void Back()
        {
            _eventAggregator.PublishOnUIThread(new SettingsGoBackMessage());
        }

        private async void OnActivated(object sender, ActivationEventArgs activationEventArgs)
        {
            _config = _configService.GetAppConfiguration();

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

            var chocolateyGuiFeatures = _configService.GetFeatures();
            foreach (var chocolateyGuiFeature in chocolateyGuiFeatures)
            {
                ChocolateyGuiFeatures.Add(chocolateyGuiFeature);
            }

            _changedChocolateyGuiFeature = new Subject<ChocolateyGuiFeature>();
            _changedChocolateyGuiFeature
                .Select(s => Observable.FromAsync(() => UpdateChocolateyGuiFeature(s)))
                .Concat()
                .Subscribe();

            var chocolateyGuiSettings = _configService.GetSettings();
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
    }
}