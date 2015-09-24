// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace ChocolateyGui.ViewModels.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;
    using Base;
    using Items;
    using Models;
    using Services;

    public class RemoteSourceControlViewModel : ObservableBase, IRemoteSourceControlViewModel
    {
        private readonly IRemotePackageService _remotePackageService;
        private readonly Uri _source;
        private int _currentPage = 1;
        private bool _includeAllVersions;
        private bool _includePrerelease;
        private bool _matchWord;
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        private int _pageCount = 1;
        private bool _hasLoaded;
        private int _pageSize = 50;
        private string _searchQuery;
        private string _sortColumn;
        private bool _sortDescending;

        public RemoteSourceControlViewModel(IRemotePackageService remotePackageService, Uri source)
        {
            try
            {
                _remotePackageService = remotePackageService;
                _source = source;
                Packages = new ObservableCollection<IPackageViewModel>();
                LoadPackages();

                var immediateProperties = new[]
                                              {
                                                  "IncludeAllVersions", "IncludePrerelease", "MatchWord", "SortColumn", 
                                                  "SortDescending"
                                              };

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == "SearchQuery")
                    .Throttle(TimeSpan.FromMilliseconds(500))
                    .DistinctUntilChanged()
                    .ObserveOnDispatcher()
                    .Subscribe(e => LoadPackages());

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => immediateProperties.Contains(e.EventArgs.PropertyName))
                    .ObserveOnDispatcher()
                    .Subscribe(e => LoadPackages());

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == "CurrentPage")
                    .Throttle(TimeSpan.FromMilliseconds(300))
                    .DistinctUntilChanged()
                    .ObserveOnDispatcher()
                    .Subscribe(e => LoadPackages());
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(
                    string.Format(CultureInfo.InvariantCulture, "Unable to connect to feed with Url: {0}.  Please check that this feed is accessible, and try again.", source), 
                    "Feed Search Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error, 
                    MessageBoxResult.OK, 
                    MessageBoxOptions.ServiceNotification);
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set { SetPropertyValue(ref _currentPage, value); }
        }

        public bool IncludeAllVersions
        {
            get { return _includeAllVersions; }
            set { SetPropertyValue(ref _includeAllVersions, value); }
        }

        public bool IncludePrerelease
        {
            get { return _includePrerelease; }
            set { SetPropertyValue(ref _includePrerelease, value); }
        }

        public bool MatchWord
        {
            get { return _matchWord; }
            set { SetPropertyValue(ref _matchWord, value); }
        }

        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return _packageViewModels; }
            set { SetPropertyValue(ref _packageViewModels, value); }
        }

        public int PageCount
        {
            get { return _pageCount; }
            set { SetPropertyValue(ref _pageCount, value); }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { SetPropertyValue(ref _pageSize, value); }
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set { SetPropertyValue(ref _searchQuery, value); }
        }

        public string SortColumn
        {
            get { return _sortColumn; }
            set { SetPropertyValue(ref _sortColumn, value); }
        }

        public bool SortDescending
        {
            get { return _sortDescending; }
            set { SetPropertyValue(ref _sortDescending, value); }
        }

        public bool CanRefreshRemotePackages()
        {
            return _hasLoaded;
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

        public void RefreshRemotePackages()
        {
            LoadPackages();
        }

        private async void LoadPackages()
        {
            _hasLoaded = false;

            var result = await _remotePackageService.Search(SearchQuery, new PackageSearchOptions(PageSize, CurrentPage - 1, SortColumn, SortDescending, IncludePrerelease, IncludeAllVersions, MatchWord), _source);
            PageCount = result.TotalCount / PageSize;
            Packages.Clear();
            result.Packages.ToList().ForEach(p =>
            {
                p.Source = _source;
                Packages.Add(p);
            });

            if (PageCount < CurrentPage)
            {
                CurrentPage = PageCount == 0 ? 1 : PageCount;
            }

            _hasLoaded = true;
        }
    }
}