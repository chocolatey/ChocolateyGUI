// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RemoteSourceControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using ChocolateyGui.Base;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Items;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;

    public class RemoteSourceControlViewModel : ObservableBase, IRemoteSourceControlViewModel
    {
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return _packageViewModels; }
            set { SetPropertyValue(ref _packageViewModels, value); }
        }

        private readonly IPackageService _packageService;
        private readonly Uri _source;

        public RemoteSourceControlViewModel(IPackageService packageService, Uri source)
        {
            _packageService = packageService;
            _source = source;
            Packages = new ObservableCollection<IPackageViewModel>();
            LoadPackages();

            var immediateProperties = new[]
            {
                "IncludeAllVersions", "IncludePrerelease", "MatchWord",
                "SortColumn", "SortDescending"
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

        private async void LoadPackages()
        {
            var result = await _packageService.Search(SearchQuery, new PackageSearchOptions(PageSize, CurrentPage - 1, SortColumn, SortDescending, IncludePrerelease, IncludeAllVersions, MatchWord), _source);
            PageCount = result.TotalCount / PageSize;
            Packages.Clear();
            result.Packages.ToList().ForEach(p =>
            {
                p.Source = _source;
                Packages.Add(p);
            });

            if (PageCount < CurrentPage)
                CurrentPage = PageCount == 0 ? 1 : PageCount;
        }

        private int _pageCount = 1;
        public int PageCount
        {
            get { return _pageCount; }
            set { SetPropertyValue(ref _pageCount, value); }
        }

        private int _pageSize = 50;
        public int PageSize
        {
            get { return _pageSize; }
            set { SetPropertyValue(ref _pageSize, value); }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { SetPropertyValue(ref _currentPage, value); }
        }

        public bool CanGoToFirst()
        {
            return CurrentPage > 1;
        }

        public void GoToFirst()
        {
            CurrentPage = 1;
        }

        public bool CanGoToPrevious()
        {
            return CurrentPage > 1;
        }

        public void GoToPrevious()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        public bool CanGoToNext()
        {
            return CurrentPage < PageCount;
        }

        public void GoToNext()
        {
            if (CurrentPage < PageCount)
                CurrentPage++;
        }

        public bool CanGoToLast()
        {
            return CurrentPage < PageCount;
        }

        public void GoToLast()
        {
            CurrentPage = PageCount;
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set { SetPropertyValue(ref _searchQuery, value); }
        }

        private bool _includePrerelease;
        public bool IncludePrerelease
        {
            get { return _includePrerelease; }
            set { SetPropertyValue(ref _includePrerelease, value); }
        }

        private bool _includeAllVersions;
        public bool IncludeAllVersions
        {
            get { return _includeAllVersions; }
            set { SetPropertyValue(ref _includeAllVersions, value); }
        }

        private bool _matchWord;
        public bool MatchWord
        {
            get { return _matchWord; }
            set { SetPropertyValue(ref _matchWord, value); }
        }

        private string _sortColumn;
        public string SortColumn
        {
            get { return _sortColumn; }
            set { SetPropertyValue(ref _sortColumn, value); }
        }

        private bool _sortDescending;
        public bool SortDescending
        {
            get { return _sortDescending; }
            set { SetPropertyValue(ref _sortDescending, value); }
        }
    }
}