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
    using System.Linq;
    using System.Reactive.Linq;
    using ChocolateyGui.Base;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    using ChocolateyGui.ViewModels.Items;

    public class RemoteSourceControlViewModel : ObservableBase, IRemoteSourceControlViewModel
    {
        private readonly IPackageService _packageService;
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

        public RemoteSourceControlViewModel(IPackageService packageService, Uri source)
        {
            this._packageService = packageService;
            this._source = source;
            this.Packages = new ObservableCollection<IPackageViewModel>();
            this.LoadPackages();

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
                .Subscribe(e => this.LoadPackages());

            Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(e => immediateProperties.Contains(e.EventArgs.PropertyName))
                .ObserveOnDispatcher()
                .Subscribe(e => this.LoadPackages());
            
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(e => e.EventArgs.PropertyName == "CurrentPage")
                .Throttle(TimeSpan.FromMilliseconds(300))
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
                .Subscribe(e => this.LoadPackages());
        }

        public int CurrentPage
        {
            get { return this._currentPage; }
            set { this.SetPropertyValue(ref this._currentPage, value); }
        }

        public bool IncludeAllVersions
        {
            get { return this._includeAllVersions; }
            set { this.SetPropertyValue(ref this._includeAllVersions, value); }
        }

        public bool IncludePrerelease
        {
            get { return this._includePrerelease; }
            set { this.SetPropertyValue(ref this._includePrerelease, value); }
        }

        public bool MatchWord
        {
            get { return this._matchWord; }
            set { this.SetPropertyValue(ref this._matchWord, value); }
        }

        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return this._packageViewModels; }
            set { this.SetPropertyValue(ref this._packageViewModels, value); }
        }

        public int PageCount
        {
            get { return this._pageCount; }
            set { this.SetPropertyValue(ref this._pageCount, value); }
        }

        public int PageSize
        {
            get { return this._pageSize; }
            set { this.SetPropertyValue(ref this._pageSize, value); }
        }

        public string SearchQuery
        {
            get { return this._searchQuery; }
            set { this.SetPropertyValue(ref this._searchQuery, value); }
        }

        public string SortColumn
        {
            get { return this._sortColumn; }
            set { this.SetPropertyValue(ref this._sortColumn, value); }
        }

        public bool SortDescending
        {
            get { return this._sortDescending; }
            set { this.SetPropertyValue(ref this._sortDescending, value); }
        }

        public bool CanRefreshRemotePackages()
        {
            return this._hasLoaded;
        }

        public bool CanGoToFirst()
        {
            return this.CurrentPage > 1;
        }

        public bool CanGoToLast()
        {
            return this.CurrentPage < this.PageCount;
        }

        public bool CanGoToNext()
        {
            return this.CurrentPage < this.PageCount;
        }

        public bool CanGoToPrevious()
        {
            return this.CurrentPage > 1;
        }

        public void GoToFirst()
        {
            this.CurrentPage = 1;
        }

        public void GoToLast()
        {
            this.CurrentPage = this.PageCount;
        }

        public void GoToNext()
        {
            if (this.CurrentPage < this.PageCount)
            {
                this.CurrentPage++;
            }
        }

        public void GoToPrevious()
        {
            if (this.CurrentPage > 1)
            {
                this.CurrentPage--;
            }
        }

        public async void RefreshRemotePackages()
        {
            this.LoadPackages();
        }

        private async void LoadPackages()
        {
            this._hasLoaded = false;

            var result = await this._packageService.Search(this.SearchQuery, new PackageSearchOptions(this.PageSize, this.CurrentPage - 1, this.SortColumn, this.SortDescending, this.IncludePrerelease, this.IncludeAllVersions, this.MatchWord), this._source);
            this.PageCount = result.TotalCount / this.PageSize;
            this.Packages.Clear();
            result.Packages.ToList().ForEach(p =>
            {
                p.Source = _source;
                this.Packages.Add(p);
            });

            if (this.PageCount < this.CurrentPage)
            {
                this.CurrentPage = this.PageCount == 0 ? 1 : this.PageCount;
            }

            this._hasLoaded = true;
        }
    }
}