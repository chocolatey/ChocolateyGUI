// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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
using Caliburn.Micro;
using ChocolateyGui.Models;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;
using ChocolateyGui.Utilities.Extensions;
using ChocolateyGui.ViewModels.Items;
using Serilog;

namespace ChocolateyGui.ViewModels
{
    using AutoMapper;

    public sealed class RemoteSourceViewModel : Screen, ISourceViewModelBase
    {
        private static readonly ILogger Logger = Log.ForContext<RemoteSourceViewModel>();
        private readonly IChocolateyService _chocolateyPackageService;
        private readonly IProgressService _progressService;
        private readonly IEventAggregator _eventAggregator;
        private int _currentPage = 1;
        private bool _hasLoaded;
        private bool _includeAllVersions;
        private bool _includePrerelease;
        private bool _matchWord;
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        private int _pageCount = 1;
        private int _pageSize = 50;
        private string _searchQuery;
        private string _sortSelection = Resources.RemoteSourceViewModel_SortSelectionPopularity;

        public RemoteSourceViewModel(
            IChocolateyService chocolateyPackageService,
            IProgressService progressService,
            IEventAggregator eventAggregator,
            ChocolateySource source)
        {
            Source = source;
            _chocolateyPackageService = chocolateyPackageService;
            _progressService = progressService;
            _eventAggregator = eventAggregator;

            Packages = new ObservableCollection<IPackageViewModel>();
            DisplayName = source.Id;

            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            _eventAggregator.Subscribe(this);
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

        public IReadOnlyList<string> SortOptions { get; } = new List<string>
        {
            Resources.RemoteSourceViewModel_SortSelectionPopularity,
            Resources.RemoteSourceViewModel_SortSelectionAtoZ
        };

        public string SortSelection
        {
            get { return _sortSelection; }
            set { this.SetPropertyValue(ref _sortSelection, value); }
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

        public bool CanRefreshRemotePackages()
        {
            return _hasLoaded;
        }

        public void RefreshRemotePackages()
        {
#pragma warning disable 4014
            LoadPackages();
#pragma warning restore 4014
        }

        protected override void OnViewAttached(object view, object context)
        {
            _eventAggregator.Subscribe(view);
        }

        protected override void OnInitialize()
        {
            try
            {
#pragma warning disable 4014
                LoadPackages();
#pragma warning restore 4014

                var immediateProperties = new[]
                {
                    "IncludeAllVersions", "IncludePrerelease", "MatchWord", "SortSelection"
                };

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == "SearchQuery")
                    .Throttle(TimeSpan.FromMilliseconds(500))
                    .DistinctUntilChanged()
                    .ObserveOnDispatcher()
#pragma warning disable 4014
                    .Subscribe(e => LoadPackages());
#pragma warning restore 4014

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => immediateProperties.Contains(e.EventArgs.PropertyName))
                    .ObserveOnDispatcher()
#pragma warning disable 4014
                    .Subscribe(e => LoadPackages());
#pragma warning restore 4014

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == "CurrentPage")
                    .Throttle(TimeSpan.FromMilliseconds(300))
                    .DistinctUntilChanged()
                    .ObserveOnDispatcher()
#pragma warning disable 4014
                    .Subscribe(e => LoadPackages());
#pragma warning restore 4014
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error(ex, "Failed to intialize remote source view model.");
                MessageBox.Show(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.RemoteSourceViewModel_UnableToConnectToFeed,
                        Source.Value),
                    Resources.RemoteSourceViewModel_FeedSearchError,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.ServiceNotification);
            }
        }

        private async Task LoadPackages()
        {
            try
            {
                _hasLoaded = false;

                var sort = SortSelection == Resources.RemoteSourceViewModel_SortSelectionPopularity ? "DownloadCount" : "Title";

                await _progressService.StartLoading(string.Format(Resources.RemoteSourceViewModel_LoadingPage, CurrentPage));
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

                    PageCount = (int)(((double)result.TotalCount / (double)PageSize) + 0.5);
                    Packages.Clear();
                    result.Packages.ToList().ForEach(p =>
                    {
                        if (installed.Any(package => package.Id == p.Id))
                        {
                            p.IsInstalled = true;
                        }

                        Packages.Add(Mapper.Map<Items.PackageViewModel>(p));
                    });

                    if (PageCount < CurrentPage)
                    {
                        CurrentPage = PageCount == 0 ? 1 : PageCount;
                    }
                }
                finally
                {
                    await _progressService.StopLoading();
                    _hasLoaded = true;
                }

                await _eventAggregator.PublishOnUIThreadAsync(new ResetScrollPositionMessage());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load new packages.");
                await _progressService.ShowMessageAsync(
                    Resources.RemoteSourceViewModel_FailedToLoad,
                    string.Format(Resources.RemoteSourceViewModel_FailedToLoadRemotePackages, ex.Message));
                throw;
            }
        }
    }
}