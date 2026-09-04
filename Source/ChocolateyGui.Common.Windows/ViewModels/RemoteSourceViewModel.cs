// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceViewModel.cs">
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
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using AutoMapper;
using Caliburn.Micro;
using ChocolateyGui.Common.Enums;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Utilities;
using ChocolateyGui.Common.ViewModels;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.Utilities;
using ChocolateyGui.Common.Windows.Utilities.Extensions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public sealed class RemoteSourceViewModel : ViewModelScreen, ISourceViewModelBase
    {
        private const int PrefetchPageCount = 2;
        private const int UiAddChunkSize = 5;

        private static readonly ILogger Logger = Log.ForContext<RemoteSourceViewModel>();
        private readonly IChocolateyService _chocolateyPackageService;
        private readonly IDialogService _dialogService;
        private readonly IProgressService _progressService;
        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;
        private readonly IConfigService _configService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMapper _mapper;
        private bool _hasLoaded;
        private bool _hasMore;
        private bool _isLoadingMore;
        private bool _shouldShowPreventPreloadMessage;
        private bool _includeAllVersions;
        private bool _includePrerelease;
        private bool _matchWord;
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        private int _pageSize = 50;
        private int _totalCount;
        private int _nextPage;
        private int _loadId;
        private IList<Package> _installedPackages = new List<Package>();
        private string _searchQuery;
        private string _sortSelection;
        private string _sortSelectionName;
        private ListViewMode _listViewMode;
        private bool _showAdditionalPackageInformation;
        private string _resourceId;

        private IDisposable _searchQuerySubscription;

        public RemoteSourceViewModel(
            IChocolateyService chocolateyPackageService,
            IDialogService dialogService,
            IProgressService progressService,
            IChocolateyGuiCacheService chocolateyGuiCacheService,
            IConfigService configService,
            IEventAggregator eventAggregator,
            ChocolateySource source,
            IMapper mapper,
            TranslationSource translator)
            : base(translator)
        {
            Source = source;
            _chocolateyPackageService = chocolateyPackageService;
            _dialogService = dialogService;
            _progressService = progressService;
            _chocolateyGuiCacheService = chocolateyGuiCacheService;
            _configService = configService;
            _eventAggregator = eventAggregator;
            _mapper = mapper;

            Packages = new ObservableCollection<IPackageViewModel>();
            PackageSource = CollectionViewSource.GetDefaultView(Packages);

            if (source.Id[0] == '[' && source.Id[source.Id.Length - 1] == ']')
            {
                _resourceId = source.Id.Trim('[', ']');
                DisplayName = translator[_resourceId];
                translator.PropertyChanged += (sender, e) =>
                {
                    DisplayName = translator[_resourceId];
                };
            }
            else
            {
                DisplayName = source.Id;
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            _eventAggregator.Subscribe(this);

            AddSortOptions();

            SortSelection = L(nameof(Resources.RemoteSourceViewModel_SortSelectionPopularity));
        }

        public bool HasLoaded
        {
            get { return _hasLoaded; }
            set { this.SetPropertyValue(ref _hasLoaded, value); }
        }

        public bool HasMore
        {
            get { return _hasMore; }
            set { this.SetPropertyValue(ref _hasMore, value); }
        }

        public bool ShowShouldPreventPreloadMessage
        {
            get { return _shouldShowPreventPreloadMessage; }
            set { this.SetPropertyValue(ref _shouldShowPreventPreloadMessage, value); }
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

        public ChocolateySource Source { get; }

        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return _packageViewModels; }
            set { this.SetPropertyValue(ref _packageViewModels, value); }
        }

        public ICollectionView PackageSource { get; }

        public int LoadedCount
        {
            get { return Packages == null ? 0 : Packages.Count; }
        }

        public int TotalCount
        {
            get { return _totalCount; }
            set { this.SetPropertyValue(ref _totalCount, value); }
        }

        public bool IncludeAllVersions
        {
            get { return _includeAllVersions; }
            set { this.SetPropertyValue(ref _includeAllVersions, value); }
        }

        public bool IncludePrerelease
        {
            get { return _includePrerelease; }
            set { this.SetPropertyValue(ref _includePrerelease, value); }
        }

        public bool MatchWord
        {
            get { return _matchWord; }
            set { this.SetPropertyValue(ref _matchWord, value); }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { this.SetPropertyValue(ref _pageSize, value); }
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set { this.SetPropertyValue(ref _searchQuery, value); }
        }

        public ObservableCollection<string> SortOptions { get; } = new ObservableCollection<string>();

        public string SortSelection
        {
            get
            {
                return _sortSelection;
            }

            set
            {
                _sortSelectionName = value == L(nameof(Resources.RemoteSourceViewModel_SortSelectionPopularity))
                    ? "DownloadCount"
                    : "Title";
                this.SetPropertyValue(ref _sortSelection, value);
            }
        }

        public bool CanSearchForPackages()
        {
            return HasLoaded;
        }

        public void SearchForPackages()
        {
#pragma warning disable 4014
            LoadPackages(false);
#pragma warning restore 4014
        }

        public bool CanLoadRemotePackages()
        {
            return HasLoaded;
        }

        public void RefreshRemotePackages()
        {
#pragma warning disable 4014
            LoadPackages(false);
#pragma warning restore 4014
        }

        public async Task LoadPackages(bool forceCheckForOutdatedPackages)
        {
            await LoadPackages(true, forceCheckForOutdatedPackages);
        }

        public async Task LoadMorePackages()
        {
            if (!HasLoaded || _isLoadingMore || !HasMore)
            {
                return;
            }

            await LoadPackages(false, false);
        }

        public bool CanCheckForOutdatedPackages()
        {
            return HasLoaded;
        }

        public async void CheckForOutdatedPackages()
        {
            _chocolateyGuiCacheService.PurgeOutdatedPackages(Source, IncludePrerelease);
            await LoadPackages(true);
        }

        protected override async void OnActivate()
        {
            if (!HasLoaded)
            {
                await LoadPackages(false);
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            _eventAggregator.Subscribe(view);
        }

        protected override void OnInitialize()
        {
            try
            {
                ListViewMode = _configService.GetEffectiveConfiguration().DefaultToTileViewForRemoteSource ?? true ? ListViewMode.Tile : ListViewMode.Standard;
                ShowAdditionalPackageInformation = _configService.GetEffectiveConfiguration().ShowAdditionalPackageInformation ?? false;

                Observable.FromEventPattern<EventArgs>(_configService, "SettingsChanged")
                    .ObserveOnDispatcher()
                    .Subscribe(eventPattern =>
                    {
                        var appConfig = (AppConfiguration)eventPattern.Sender;

                        _searchQuerySubscription?.Dispose();
                        if (appConfig.UseDelayedSearch ?? false)
                        {
                            SubscribeToLoadPackagesOnSearchQueryChange();
                        }

                        ListViewMode = appConfig.DefaultToTileViewForRemoteSource ?? false ? ListViewMode.Tile : ListViewMode.Standard;
                        ShowAdditionalPackageInformation = appConfig.ShowAdditionalPackageInformation ?? false;
                    });

                var immediateProperties = new[]
                {
                    "IncludeAllVersions", "IncludePrerelease", "MatchWord", "SortSelection"
                };

                if (_configService.GetEffectiveConfiguration().UseDelayedSearch ?? false)
                {
                    SubscribeToLoadPackagesOnSearchQueryChange();
                }

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => immediateProperties.Contains(e.EventArgs.PropertyName))
                    .ObserveOnDispatcher()
#pragma warning disable 4014
                    .Subscribe(e => LoadPackages(false));
#pragma warning restore 4014
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error(ex, "Failed to initialize remote source view model.");
                var message = L(nameof(Resources.RemoteSourceViewModel_UnableToConnectToFeed));
                var caption = L(nameof(Resources.RemoteSourceViewModel_FeedSearchError));
                ChocolateyMessageBox.Show(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        message,
                        Source.Value),
                    caption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.ServiceNotification);
            }
        }

        protected override void OnLanguageChanged()
        {
            AddSortOptions();

            SortSelection = _sortSelectionName == "DownloadCount"
                ? L(nameof(Resources.RemoteSourceViewModel_SortSelectionPopularity))
                : L(nameof(Resources.RemoteSourceViewModel_SortSelectionAtoZ));

            RemoveOldSortOptions();
        }

        private async Task LoadPackages(bool reset, bool forceCheckForOutdatedPackages)
        {
            try
            {
                if (!IsActive || (!CanLoadRemotePackages() && Packages.Any()))
                {
                    return;
                }

                if (reset && !HasLoaded && (_configService.GetEffectiveConfiguration().PreventPreload ?? false))
                {
                    ShowShouldPreventPreloadMessage = true;
                    HasLoaded = true;
                    HasMore = false;
                    return;
                }

                if (!reset && (!HasLoaded || _isLoadingMore || !HasMore))
                {
                    return;
                }

                var loadId = reset ? ++_loadId : _loadId;

                if (reset)
                {
                    HasLoaded = false;
                    ShowShouldPreventPreloadMessage = false;
                    _nextPage = 0;
                    HasMore = true;
                    Packages.Clear();
                    NotifyListCounts();
                }
                else
                {
                    _isLoadingMore = true;
                }

                try
                {
                    if (reset)
                    {
                        await _progressService.StartLoading(L(nameof(Resources.RemoteSourceViewModel_FetchingPackages)));
                        _progressService.WriteMessage(L(nameof(Resources.RemoteSourceViewModel_FetchingPackages)));
                    }

                    try
                    {
                        if (reset)
                        {
                            _installedPackages = (await GetInstalledPackagesAsync()).ToList();
                        }

                        var added = 0;
                        do
                        {
                            if (loadId != _loadId)
                            {
                                return;
                            }

                            var page = _nextPage;
                            var result = await SearchPackagesAsync(
                                new PackageSearchOptions(
                                    PageSize,
                                    page,
                                    _sortSelectionName,
                                    IncludePrerelease,
                                    IncludeAllVersions,
                                    MatchWord,
                                    Source.Value,
                                    reset && page == 0));

                            if (loadId != _loadId)
                            {
                                return;
                            }

                            added = await ApplySearchResultAsync(result, reset, page);
                        }
                        while (added == 0 && HasMore);

                        if (reset)
                        {
                            var outdatedPackages = await GetOutdatedPackagesAsync(forceCheckForOutdatedPackages);

                            foreach (var update in outdatedPackages)
                            {
                                await _eventAggregator.PublishOnUIThreadAsync(new PackageOutdatedMessage(update.Id, update.Version, source: Source));
                            }
                        }
                    }
                    finally
                    {
                        if (reset)
                        {
                            await _progressService.StopLoading();
                            HasLoaded = true;
                        }
                    }

                    if (reset && loadId == _loadId)
                    {
                        // Do not await: extra pages arrive in the background so the list stays interactive.
#pragma warning disable 4014
                        PrefetchAhead(loadId);
#pragma warning restore 4014
                        await _eventAggregator.PublishOnUIThreadAsync(new ResetScrollPositionMessage());
                    }
                }
                finally
                {
                    if (!reset)
                    {
                        _isLoadingMore = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load new packages.");
                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.RemoteSourceViewModel_FailedToLoad)),
                    L(nameof(Resources.RemoteSourceViewModel_FailedToLoadRemotePackages), ex.Message));
                throw;
            }
        }

        private async Task PrefetchAhead(int loadId)
        {
            try
            {
                for (var i = 0; i < PrefetchPageCount; i++)
                {
                    if (loadId != _loadId || !HasMore)
                    {
                        return;
                    }

                    await LoadPackages(false, false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to prefetch packages.");
            }
        }

        private async Task<IEnumerable<Package>> GetInstalledPackagesAsync()
        {
            if (Application.Current != null)
            {
                return await Task.Run(() => _chocolateyPackageService.GetInstalledPackages()).ConfigureAwait(false);
            }

            return await _chocolateyPackageService.GetInstalledPackages();
        }

        private async Task<IReadOnlyList<OutdatedPackage>> GetOutdatedPackagesAsync(bool forceCheckForOutdatedPackages)
        {
            if (Application.Current != null)
            {
                return await Task.Run(() => _chocolateyPackageService.GetOutdatedPackages(IncludePrerelease, forceCheckForOutdatedPackages, Source)).ConfigureAwait(false);
            }

            return await _chocolateyPackageService.GetOutdatedPackages(IncludePrerelease, forceCheckForOutdatedPackages, Source);
        }

        private async Task<PackageResults> SearchPackagesAsync(PackageSearchOptions options)
        {
            // Chocolatey search is expensive; keep it off the dispatcher so scrolling stays responsive.
            if (Application.Current != null)
            {
                return await Task.Run(() => _chocolateyPackageService.Search(SearchQuery, options)).ConfigureAwait(false);
            }

            return await _chocolateyPackageService.Search(SearchQuery, options);
        }

        private async Task<int> ApplySearchResultAsync(PackageResults result, bool reset, int page)
        {
            var fetchedCount = result.Packages == null ? 0 : result.Packages.Length;
            var viewModels = Application.Current != null
                ? await Task.Run(() => CreatePackageViewModels(result.Packages)).ConfigureAwait(false)
                : CreatePackageViewModels(result.Packages);

            var added = await AddViewModelsAsync(viewModels);

            System.Action commitPage = () =>
            {
                if (reset && page == 0)
                {
                    TotalCount = result.TotalCount;
                }

                _nextPage = page + 1;
                HasMore = fetchedCount >= PageSize;
                NotifyListCounts();
            };

            var dispatcher = Application.Current != null ? Application.Current.Dispatcher : null;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                await dispatcher.InvokeAsync(commitPage).Task.ConfigureAwait(false);
            }
            else
            {
                commitPage();
            }

            return added;
        }

        private IList<IPackageViewModel> CreatePackageViewModels(IEnumerable<Package> packages)
        {
            var viewModels = new List<IPackageViewModel>();
            if (packages == null)
            {
                return viewModels;
            }

            var installedPackages = _installedPackages ?? new List<Package>();

            // When showing all versions, the source returns them in the active sort order (e.g.
            // popularity / download count), which interleaves the versions of a package. Order each
            // package's versions newest-first, keeping the packages themselves in their original
            // (relevance) order.
            var packagesToDisplay = IncludeAllVersions
                ? packages
                    .GroupBy(package => package.Id, StringComparer.OrdinalIgnoreCase)
                    .SelectMany(group => group.OrderByDescending(package => package.Version))
                    .ToList()
                : packages.ToList();

            packagesToDisplay.ForEach(p =>
            {
                var remoteVersion = p.Version;

                if (IncludeAllVersions)
                {
                    // When showing all versions, every row is a distinct version of the same package.
                    // Only flag the row whose version is actually installed, and never overwrite the
                    // displayed version - otherwise every row collapses to the installed version (#1146).
                    var installedVersion = installedPackages.FirstOrDefault(package =>
                        string.Equals(package.Id, p.Id, StringComparison.OrdinalIgnoreCase)
                        && Equals(package.Version, p.Version));
                    if (installedVersion != null)
                    {
                        p.IsPinned = installedVersion.IsPinned;
                        p.IsInstalled = true;
                    }
                }
                else
                {
                    var installedPackage = installedPackages.FirstOrDefault(package => string.Equals(package.Id, p.Id, StringComparison.OrdinalIgnoreCase));
                    if (installedPackage != null)
                    {
                        p.Version = installedPackage.Version;
                        p.IsPinned = installedPackage.IsPinned;
                        p.IsInstalled = true;
                    }
                }

                var packageViewModel = Mapper.Map<IPackageViewModel>(p);
                packageViewModel.ChocolateySource = Source;
                packageViewModel.RemoteVersion = remoteVersion;
                viewModels.Add(packageViewModel);
            });

            if (_configService.GetEffectiveConfiguration().ExcludeInstalledPackages ?? false)
            {
                viewModels.RemoveAll(package => package.IsInstalled);
            }

            return viewModels;
        }

        private async Task<int> AddViewModelsAsync(IList<IPackageViewModel> viewModels)
        {
            if (viewModels == null || viewModels.Count == 0)
            {
                return 0;
            }

            var dispatcher = Application.Current != null ? Application.Current.Dispatcher : null;
            if (dispatcher == null)
            {
                foreach (var packageViewModel in viewModels)
                {
                    Packages.Add(packageViewModel);
                }

                NotifyListCounts();
                return viewModels.Count;
            }

            for (var index = 0; index < viewModels.Count; index += UiAddChunkSize)
            {
                var start = index;
                var count = Math.Min(UiAddChunkSize, viewModels.Count - index);
                await dispatcher.InvokeAsync(() =>
                {
                    for (var offset = 0; offset < count; offset++)
                    {
                        Packages.Add(viewModels[start + offset]);
                    }

                    NotifyListCounts();
                }).Task.ConfigureAwait(false);

                // Let scroll input and layout run before the next tiles are materialized.
                await dispatcher.InvokeAsync(() => { }, DispatcherPriority.Input).Task.ConfigureAwait(false);
            }

            return viewModels.Count;
        }

        private void NotifyListCounts()
        {
            NotifyOfPropertyChange(nameof(LoadedCount));
        }

        private void AddSortOptions()
        {
            var downloadCount = L(nameof(Resources.RemoteSourceViewModel_SortSelectionPopularity));
            var title = L(nameof(Resources.RemoteSourceViewModel_SortSelectionAtoZ));

            var index = SortOptions.IndexOf(downloadCount);

            if (index == -1)
            {
                SortOptions.Insert(0, downloadCount);
            }

            index = SortOptions.IndexOf(title);

            if (index == -1)
            {
                SortOptions.Insert(1, title);
            }
        }

        private void RemoveOldSortOptions()
        {
            var downloadCount = L(nameof(Resources.RemoteSourceViewModel_SortSelectionPopularity));
            var title = L(nameof(Resources.RemoteSourceViewModel_SortSelectionAtoZ));

            for (var index = SortOptions.Count - 1; index >= 0; index--)
            {
                if (SortOptions[index] != downloadCount && SortOptions[index] != title)
                {
                    SortOptions.RemoveAt(index);
                }
            }
        }

        private void SubscribeToLoadPackagesOnSearchQueryChange()
        {
            _searchQuerySubscription = Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(e => e.EventArgs.PropertyName == "SearchQuery")
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
#pragma warning disable 4014
                .Subscribe(e => LoadPackages(false));
#pragma warning restore 4014
        }
    }
}
