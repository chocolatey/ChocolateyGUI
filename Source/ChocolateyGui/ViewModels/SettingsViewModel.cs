// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SettingsViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Controls;
using Caliburn.Micro;
using ChocolateyGui.Models;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Services;
using ChocolateyGui.Subprocess.Models;
using ChocolateyGui.Utilities.Extensions;
using NuGet;
using Serilog;
using WampSharp.V2.Core.Contracts;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.ViewModels
{
    public sealed class SettingsViewModel : Screen
    {
        private static readonly ILogger Logger = Log.ForContext<SettingsViewModel>();

        private static readonly HashSet<string> ConfigProperties = new HashSet<string>
                                                                       {
                                                                           nameof(ElevateByDefault),
                                                                           nameof(ShowConsoleOutput)
                                                                       };

        private readonly IChocolateyPackageService _packageService;
        private readonly IProgressService _progressService;
        private readonly IConfigService _configService;

        private readonly IEventAggregator _eventAggregator;

        private Subject<ChocolateyFeature> _changedFeature;
        private Subject<ChocolateySetting> _changedSetting;
        private IDisposable _configSubscription;
        private AppConfiguration _config;
        private ChocolateySource _selectedSource;
        private string _originalId;
        private bool _isNewItem;

        public SettingsViewModel(
            IChocolateyPackageService packageService,
            IProgressService progressService,
            IConfigService configService,
            IEventAggregator eventAggregator)
        {
            _packageService = packageService;
            _progressService = progressService;
            _configService = configService;
            _eventAggregator = eventAggregator;
            DisplayName = "ChocolateyGUI - Settings";
            Activated += OnActivated;
            Deactivated += OnDeactivated;
        }

        public ObservableCollection<ChocolateyFeature> Features { get; } = new ObservableCollection<ChocolateyFeature>();

        public ObservableCollection<ChocolateySetting> Settings { get; } = new ObservableCollection<ChocolateySetting>();

        public ObservableCollection<ChocolateySource> Sources { get; } = new ObservableCollection<ChocolateySource>();

        public bool ElevateByDefault
        {
            get
            {
                return _config.ElevateByDefault;
            }

            set
            {
                _config.ElevateByDefault = value;
                NotifyOfPropertyChange();
            }
        }

        public bool ShowConsoleOutput
        {
            get
            {
                return _config.ShowConsoleOutput;
            }

            set
            {
                _config.ShowConsoleOutput = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanSave => SelectedSource != null;

        public bool CanRemove => SelectedSource != null && !_isNewItem;

        public bool CanCanel => SelectedSource != null;

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

                NotifyOfPropertyChange(nameof(CanSave));
                NotifyOfPropertyChange(nameof(CanRemove));
            }
        }

        public void FeatureToggled(ChocolateyFeature feature)
        {
            _changedFeature.OnNext(feature);
        }

        public async void RowEditEnding(DataGridRowEditEndingEventArgs eventArgs)
        {
            await Task.Delay(100);
            _changedSetting.OnNext((ChocolateySetting)eventArgs.Row.Item);
        }

        public void SourceSelectionChanged(object source)
        {
            var sourceItem = (ChocolateySource)source;
            SelectedSource = sourceItem;
            return;
        }

        public void UpdateConfig()
        {
            _configService.UpdateSettings(_config);
        }

        public async Task UpdateFeature(ChocolateyFeature feature)
        {
            try
            {
                await _packageService.SetFeature(feature);
            }
            catch (WampException ex)
            {
                await LogError(ex);
            }
        }

        public async Task UpdateSetting(ChocolateySetting setting)
        {
            try
            {
                await _packageService.SetSetting(setting);
            }
            catch (WampException ex)
            {
                await LogError(ex);
            }
        }

        public void New()
        {
            SelectedSource = new ChocolateySource();
        }

        public async void Save()
        {
            if (string.IsNullOrWhiteSpace(SelectedSource.Id))
            {
                await _progressService.ShowMessageAsync("Saving Source", "Source must have an Id!");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedSource.Value))
            {
                await _progressService.ShowMessageAsync("Saving Source", "Source must have an value!");
                return;
            }

            await _progressService.StartLoading("Saving source...");
            try
            {
                if (_isNewItem)
                {
                    await _packageService.AddSource(SelectedSource);
                    _isNewItem = false;
                    Sources.Add(SelectedSource);
                    NotifyOfPropertyChange(nameof(CanRemove));
                }
                else
                {
                    await _packageService.UpdateSource(_originalId, SelectedSource);
                }

                _originalId = SelectedSource?.Id;
                await _eventAggregator.PublishOnUIThreadAsync(new SourcesUpdatedMessage());
            }
            finally
            {
                await _progressService.StopLoading();
            }
        }

        public async void Remove()
        {
            await _progressService.StartLoading("Removing source...");
            try
            {
                await _packageService.RemoveSource(_originalId);
                Sources.Remove(SelectedSource);
                SelectedSource = null;
                await _eventAggregator.PublishOnUIThreadAsync(new SourcesUpdatedMessage());
            }
            finally
            {
                await _progressService.StopLoading();
            }
        }

        public void Cancel()
        {
            SelectedSource = null;
        }

        public void Back()
        {
            _eventAggregator.PublishOnUIThread(new SettingsGoBackMessage());
        }

        private async void OnActivated(object sender, ActivationEventArgs activationEventArgs)
        {
            WireUpConfig();

            var features = await _packageService.GetFeatures();
            Features.AddRange(features);

            _changedFeature = new Subject<ChocolateyFeature>();
            _changedFeature
                        .Select(f => Observable.FromAsync(() => UpdateFeature(f)))
                        .Concat()
                        .Subscribe();

            var settings = await _packageService.GetSettings();
            Settings.AddRange(settings);

            _changedSetting = new Subject<ChocolateySetting>();
            _changedSetting
                        .Select(s => Observable.FromAsync(() => UpdateSetting(s)))
                        .Concat()
                        .Subscribe();

            var sources = await _packageService.GetSources();
            Sources.AddRange(sources);
        }

        private void WireUpConfig()
        {
            _config = _configService.GetSettings();
            foreach (var prop in ConfigProperties)
            {
                NotifyOfPropertyChange(prop);
            }

            _configSubscription = Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(p => ConfigProperties.Contains(p.EventArgs.PropertyName))
                .Subscribe(_ => UpdateConfig());
        }

        private void OnDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            _changedFeature.OnCompleted();
            _changedSetting.OnCompleted();
            _configSubscription.Dispose();
        }

        private async Task LogError(WampException ex)
        {
            Logger.Error(ex, "Failed to update features list!\nMessage: {Message}\nArguments: {Arguments}", ex.Message, ex.Arguments);
            await _progressService.ShowMessageAsync(
                "Feature Updates Error",
                $"Failed to update features. Error:\n{string.Join("\v", ex.Arguments)}");
        }
    }
}