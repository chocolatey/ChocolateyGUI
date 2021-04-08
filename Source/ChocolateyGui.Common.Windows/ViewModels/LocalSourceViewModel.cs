// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using AutoMapper;
using Caliburn.Micro;
using chocolatey;
using ChocolateyGui.Common.Base;
using ChocolateyGui.Common.Enums;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.ViewModels;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using MahApps.Metro.Controls.Dialogs;
using Serilog;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public sealed class LocalSourceViewModel : Screen, ISourceViewModelBase, IHandleWithTask<PackageChangedMessage>
    {
        private static readonly ILogger Logger = Log.ForContext<LocalSourceViewModel>();
        private readonly IChocolateyService _chocolateyService;
        private readonly List<IPackageViewModel> _packages;
        private readonly IPersistenceService _persistenceService;
        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;
        private readonly IProgressService _progressService;
        private readonly IConfigService _configService;
        private readonly IAllowedCommandsService _allowedCommandsService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMapper _mapper;
        private readonly IDialogCoordinator _dialogCoordinator;
        private bool _exportAll = true;
        private bool _hasLoaded;
        private bool _matchWord;
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        private string _searchQuery;
        private bool _showOnlyPackagesWithUpdate;
        private bool _isShowOnlyPackagesWithUpdateEnabled;
        private string _sortColumn;
        private bool _sortDescending;
        private bool _isLoading;
        private bool _firstLoadIncomplete = true;
        private ListViewMode _listViewMode;
        private bool _showAdditionalPackageInformation;

        public LocalSourceViewModel(
            IChocolateyService chocolateyService,
            IProgressService progressService,
            IPersistenceService persistenceService,
            IChocolateyGuiCacheService chocolateyGuiCacheService,
            IConfigService configService,
            IAllowedCommandsService allowedCommandsService,
            IEventAggregator eventAggregator,
            string displayName,
            IMapper mapper,
            IDialogCoordinator dialogCoordinator)
        {
            _chocolateyService = chocolateyService;
            _progressService = progressService;
            _persistenceService = persistenceService;
            _chocolateyGuiCacheService = chocolateyGuiCacheService;
            _configService = configService;
            _allowedCommandsService = allowedCommandsService;

            DisplayName = displayName;

            _packages = new List<IPackageViewModel>();
            Packages = new ObservableCollection<IPackageViewModel>();
            PackageSource = CollectionViewSource.GetDefaultView(Packages);
            PackageSource.Filter = FilterPackage;

            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            _eventAggregator = eventAggregator;
            _mapper = mapper;
            _dialogCoordinator = dialogCoordinator;
            _eventAggregator.Subscribe(this);
        }

        public bool HasLoaded
        {
            get { return _hasLoaded; }
            set { this.SetPropertyValue(ref _hasLoaded, value); }
        }

        public ListViewMode ListViewMode
        {
            get { return _listViewMode; }
            set { this.SetPropertyValue(ref _listViewMode, value); }
        }

        public bool ShowAdditionalPackageInformation
        {
            get { return _showAdditionalPackageInformation; }
            set { this.SetPropertyValue(ref _showAdditionalPackageInformation, value); }
        }

        public bool ShowOnlyPackagesWithUpdate
        {
            get { return _showOnlyPackagesWithUpdate; }
            set { this.SetPropertyValue(ref _showOnlyPackagesWithUpdate, value); }
        }

        public bool IsShowOnlyPackagesWithUpdateEnabled
        {
            get { return _isShowOnlyPackagesWithUpdateEnabled; }
            set { this.SetPropertyValue(ref _isShowOnlyPackagesWithUpdateEnabled, value); }
        }

        public bool MatchWord
        {
            get { return _matchWord; }
            set { this.SetPropertyValue(ref _matchWord, value); }
        }

        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return _packageViewModels; }
            set { this.SetPropertyValue(ref _packageViewModels, value); }
        }

        public ICollectionView PackageSource { get; }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set { this.SetPropertyValue(ref _searchQuery, value); }
        }

        public string SortColumn
        {
            get { return _sortColumn; }
            set { this.SetPropertyValue(ref _sortColumn, value); }
        }

        public bool SortDescending
        {
            get { return _sortDescending; }
            set { this.SetPropertyValue(ref _sortDescending, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.SetPropertyValue(ref _isLoading, value); }
        }

        public bool FirstLoadIncomplete
        {
            get { return _firstLoadIncomplete; }
            set { this.SetPropertyValue(ref _firstLoadIncomplete, value); }
        }

        public bool IsUpgradeAllowed
        {
            get { return _allowedCommandsService.IsUpgradeCommandAllowed; }
        }

        public bool CanUpdateAll()
        {
            return Packages.Any(p => p.CanUpdate) && _allowedCommandsService.IsUpgradeCommandAllowed;
        }

        public async void UpdateAll()
        {
            try
            {
                var result = await _dialogCoordinator.ShowMessageAsync(
                    this,
                    Resources.Dialog_AreYouSureTitle,
                    Resources.Dialog_AreYouSureUpdateAllMessage,
                    MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings
                    {
                        AffirmativeButtonText = Resources.Dialog_Yes,
                        NegativeButtonText = Resources.Dialog_No
                    });

                if (result == MessageDialogResult.Affirmative)
                {
                    await _progressService.StartLoading(Resources.LocalSourceViewModel_Packages, true);
                    IsLoading = true;

                    _progressService.WriteMessage(Resources.LocalSourceViewModel_FetchingPackages);
                    var token = _progressService.GetCancellationToken();
                    var packages = Packages.Where(p => p.CanUpdate && !p.IsPinned).ToList();
                    double current = 0.0f;
                    foreach (var package in packages)
                    {
                        if (token.IsCancellationRequested)
                        {
                            await _progressService.StopLoading();
                            IsLoading = false;
                            return;
                        }

                        _progressService.Report(Math.Min(current++ / packages.Count, 100));
                        await package.Update();
                    }

                    await _progressService.StopLoading();
                    IsLoading = false;
                    ShowOnlyPackagesWithUpdate = false;
                    RefreshPackages();
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Updated all has failed.", ex);
                throw;
            }
        }

        public void ExportAll()
        {
            _exportAll = false;

            try
            {
                using (var fileStream = _persistenceService.SaveFile("*.config", Resources.LocalSourceViewModel_ConfigFiles.format_with("(.config)|*.config")))
                {
                    if (fileStream == null)
                    {
                        return;
                    }

                    var settings = new XmlWriterSettings { Indent = true };

                    using (var xw = XmlWriter.Create(fileStream, settings))
                    {
                        xw.WriteStartDocument();
                        xw.WriteStartElement("packages");

                        foreach (var package in Packages)
                        {
                            xw.WriteStartElement("package");
                            xw.WriteAttributeString("id", package.Id);
                            xw.WriteAttributeString("version", package.Version.ToString());
                            xw.WriteEndElement();
                        }

                        xw.WriteEndElement();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Export all has failed.", ex);
                throw;
            }
            finally
            {
                _exportAll = true;
            }
        }

        public bool CanExportAll()
        {
            return _exportAll;
        }

        public bool CanCheckForOutdatedPackages()
        {
            return HasLoaded && !IsLoading;
        }

        public async Task CheckForOutdatedPackages()
        {
            _chocolateyGuiCacheService.PurgeOutdatedPackages();
            await CheckOutdated(true);
        }

        public bool CanRefreshPackages()
        {
            return HasLoaded && !IsLoading;
        }

        public async void RefreshPackages()
        {
            await LoadPackages();
        }

        public async Task Handle(PackageChangedMessage message)
        {
            switch (message.ChangeType)
            {
                case PackageChangeType.Pinned:
                    PackageSource.Refresh();
                    break;
                case PackageChangeType.Unpinned:
                    var package = Packages.First(p => p.Id == message.Id);
                    if (package.LatestVersion != null)
                    {
                        PackageSource.Refresh();
                    }
                    else
                    {
                        var outOfDatePackages =
                            await _chocolateyService.GetOutdatedPackages(package.IsPrerelease, package.Id, false);
                        foreach (var update in outOfDatePackages)
                        {
                            await _eventAggregator.PublishOnUIThreadAsync(new PackageHasUpdateMessage(update.Id, update.Version));
                        }

                        PackageSource.Refresh();
                    }

                    break;

                case PackageChangeType.Uninstalled:
                    Packages.Remove(Packages.First(p => p.Id == message.Id));
                    break;

                default:
                    await LoadPackages();
                    break;
            }
        }

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        protected override async void OnInitialize()
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                if (HasLoaded)
                {
                    return;
                }

                ListViewMode = _configService.GetEffectiveConfiguration().DefaultToTileViewForLocalSource ?? true ? ListViewMode.Tile : ListViewMode.Standard;
                ShowAdditionalPackageInformation = _configService.GetEffectiveConfiguration().ShowAdditionalPackageInformation ?? false;

                Observable.FromEventPattern<EventArgs>(_configService, "SettingsChanged")
                    .ObserveOnDispatcher()
                    .Subscribe(eventPattern =>
                    {
                        var appConfig = (AppConfiguration)eventPattern.Sender;

                        ListViewMode = appConfig.DefaultToTileViewForLocalSource ?? false
                                ? ListViewMode.Tile
                                : ListViewMode.Standard;
                        ShowAdditionalPackageInformation = appConfig.ShowAdditionalPackageInformation ?? false;
                    });

                await LoadPackages();

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(
                        eventPattern =>
                            eventPattern.EventArgs.PropertyName == nameof(MatchWord) ||
                            eventPattern.EventArgs.PropertyName == nameof(SearchQuery) ||
                            eventPattern.EventArgs.PropertyName == nameof(ShowOnlyPackagesWithUpdate))
                    .ObserveOnDispatcher()
                    .Subscribe(eventPattern => PackageSource.Refresh());

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(eventPattern => eventPattern.EventArgs.PropertyName == nameof(ListViewMode))
                    .ObserveOnDispatcher()
                    .Subscribe(eventPattern =>
                    {
                        if (ListViewMode == ListViewMode.Tile)
                        {
                            // reset custom sorting for now
                            var listColView = PackageSource as ListCollectionView;
                            if (listColView != null)
                            {
                                listColView.CustomSort = null;
                            }
                        }
                    });

                HasLoaded = true;

                var chocoPackage = _packages.FirstOrDefault(p => p.Id.ToLower() == "chocolatey");
                if (chocoPackage != null && chocoPackage.CanUpdate)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _progressService.ShowMessageAsync(Resources.LocalSourceViewModel_Chocolatey, Resources.LocalSourceViewModel_UpdateAvailableForChocolatey)
                        .ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Local source control view model failed to load.", ex);
                throw;
            }
        }

        private bool FilterPackage(object packageObject)
        {
            var package = (IPackageViewModel)packageObject;
            var include = true;
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                if (MatchWord)
                {
                    include &= string.Compare(
                        package.Title ?? package.Id,
                        SearchQuery,
                        StringComparison.OrdinalIgnoreCase) == 0;
                }
                else
                {
                    include &= CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                                   package.Title ?? package.Id,
                                   SearchQuery,
                                   CompareOptions.OrdinalIgnoreCase) >= 0;
                }
            }

            if (ShowOnlyPackagesWithUpdate)
            {
                include &= package.CanUpdate && !package.IsPinned;
            }

            return include;
        }

        private async Task LoadPackages()
        {
            if (IsLoading)
            {
                return;
            }

            IsLoading = true;
            IsShowOnlyPackagesWithUpdateEnabled = false;

            _packages.Clear();
            Packages.Clear();

            var packages = (await _chocolateyService.GetInstalledPackages())
                .Select(Mapper.Map<IPackageViewModel>).ToList();

            foreach (var packageViewModel in packages)
            {
                _packages.Add(packageViewModel);
                Packages.Add(packageViewModel);
            }

            FirstLoadIncomplete = false;

            await CheckOutdated(false);
        }

        private async Task CheckOutdated(bool forceCheckForOutdated)
        {
            IsLoading = true;

            try
            {
                var updates = await _chocolateyService.GetOutdatedPackages(false, null, forceCheckForOutdated);

                // Use a list of task for correct async loop
                var listOfTasks = updates.Select(update => _eventAggregator.PublishOnUIThreadAsync(new PackageHasUpdateMessage(update.Id, update.Version))).ToList();
                await Task.WhenAll(listOfTasks);

                PackageSource.Refresh();
            }
            catch (ConnectionClosedException)
            {
                Logger.Warning("Threw connection closed message while processing load packages.");
            }
            catch (Exception ex)
            {
                Logger.Fatal("Packages failed to load", ex);
                throw;
            }
            finally
            {
                IsLoading = false;

                // Only enable the "Show only outdated packages" when it makes sense.
                // It does not make sense to enable the checkbox when we haven't checked for
                // outdated packages. We should only enable the checkbox here when: (or)
                // 1. the "Prevent Automated Outdated Packages Check" is disabled
                // 2. forced a check for outdated packages.
                IsShowOnlyPackagesWithUpdateEnabled = forceCheckForOutdated || !(_configService.GetEffectiveConfiguration().PreventAutomatedOutdatedPackagesCheck ?? false);

                // Force invalidating the command stuff.
                // This helps us to prevent disabled buttons after executing this routine.
                // But IMO it has something to do with Caliburn.
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}