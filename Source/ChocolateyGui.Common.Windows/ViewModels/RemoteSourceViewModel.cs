// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceViewModel.cs">
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
using System.Threading.Tasks;
using System.Windows;
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
using NuGet;
using NuGet.Packaging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public sealed class RemoteSourceViewModel : ViewModelScreen, ISourceViewModelBase
    {
        private static readonly ILogger Logger = Log.ForContext<RemoteSourceViewModel>();
        private readonly IChocolateyService _chocolateyPackageService;
        private readonly IDialogService _dialogService;
        private readonly IProgressService _progressService;
        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;
        private readonly IConfigService _configService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMapper _mapper;
        private int _currentPage = 1;
        private bool _hasLoaded;
        private bool _shouldShowPreventPreloadMessage;
        private bool _includeAllVersions;
        private bool _includePrerelease;
        private bool _matchWord;
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        private int _pageCount = 1;
        private int _pageSize = 50;
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

        public int CurrentPage
        {
            get { return _currentPage; }
            set { this.SetPropertyValue(ref _currentPage, value); }
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

        public int PageCount
        {
            get { return _pageCount; }
            set { this.SetPropertyValue(ref _pageCount, value); }
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

        public bool CanGoToFirst()
        {
            return CurrentPage > 1;
        }

        public bool CanGoToLast()
        {
            return CurrentPage < PageCount;
        }

        public bool CanGoToNext()
        {
            return CurrentPage < PageCount;
        }

        public bool CanGoToPrevious()
        {
            return CurrentPage > 1;
        }

        public void GoToFirst()
        {
            CurrentPage = 1;
        }

        public void GoToLast()
        {
            CurrentPage = PageCount;
        }

        public void GoToNext()
        {
            if (CurrentPage < PageCount)
            {
                CurrentPage++;
            }
        }

        public void GoToPrevious()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
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
            try
            {
                if (!IsActive || (!CanLoadRemotePackages() && Packages.Any()))
                {
                    return;
                }

                if (!HasLoaded && (_configService.GetEffectiveConfiguration().PreventPreload ?? false))
                {
                    ShowShouldPreventPreloadMessage = true;
                    HasLoaded = true;
                    return;
                }

                HasLoaded = false;
                ShowShouldPreventPreloadMessage = false;

                var sort = _sortSelectionName;

                await _progressService.StartLoading(L(nameof(Resources.RemoteSourceViewModel_LoadingPage), CurrentPage));

                _progressService.WriteMessage(L(nameof(Resources.RemoteSourceViewModel_FetchingPackages)));

                try
                {
                    var result =
                        await
                            _chocolateyPackageService.Search(
                                SearchQuery,
                                new PackageSearchOptions(
                                    PageSize,
                                    CurrentPage - 1,
                                    sort,
                                    IncludePrerelease,
                                    IncludeAllVersions,
                                    MatchWord,
                                    Source.Value));
                    var installed = await _chocolateyPackageService.GetInstalledPackages();
                    var outdated = await _chocolateyPackageService.GetOutdatedPackages(false, null, forceCheckForOutdatedPackages);

                    PageCount = (int)Math.Ceiling((double)result.TotalCount / (double)PageSize);
                    Packages.Clear();

                    result.Packages.ToList().ForEach(p =>
                    {
                        if (installed.Any(package => string.Equals(package.Id, p.Id, StringComparison.OrdinalIgnoreCase)))
                        {
                            p.IsInstalled = true;
                        }

                        Packages.Add(Mapper.Map<IPackageViewModel>(p));
                    });

                    if (_configService.GetEffectiveConfiguration().ExcludeInstalledPackages ?? false)
                    {
                        Packages.RemoveAll(x => x.IsInstalled);
                    }

                    if (PageCount < CurrentPage)
                    {
                        CurrentPage = PageCount == 0 ? 1 : PageCount;
                    }
                }
                finally
                {
                    await _progressService.StopLoading();
                    HasLoaded = true;
                }

                await _eventAggregator.PublishOnUIThreadAsync(new ResetScrollPositionMessage());
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

        public bool CanCheckForOutdatedPackages()
        {
            return HasLoaded;
        }

        public async void CheckForOutdatedPackages()
        {
            _chocolateyGuiCacheService.PurgeOutdatedPackages();
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

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == "CurrentPage")
                    .Throttle(TimeSpan.FromMilliseconds(300))
                    .DistinctUntilChanged()
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

            SortOptions.RemoveAll(so => so != downloadCount && so != title);
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