using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Controls
{
    public class LocalSourceControlViewModel : ObservableBase, ILocalSourceControlViewModel, IWeakEventListener
    {
        private readonly List<IPackageViewModel> _packages;
        private bool _hasLoaded = false;

        private ObservableCollection<IPackageViewModel> _packageViewModels; 
        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return _packageViewModels; }
            set { SetPropertyValue(ref _packageViewModels, value); }
        }

        private readonly IChocolateyService _chocolateyService;
        public LocalSourceControlViewModel(IChocolateyService chocolateyService)
        {
            _chocolateyService = chocolateyService;
            PackagesChangedEventManager.AddListener(_chocolateyService, this);

            Packages = new ObservableCollection<IPackageViewModel>();
            _packages = new List<IPackageViewModel>();
        }

        public async void Loaded(object sender, EventArgs args)
        {
            if (_hasLoaded)
                return;

            await LoadPackages();
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(e => e.EventArgs.PropertyName == "MatchWord" || e.EventArgs.PropertyName == "SearchQuery")
                .ObserveOnDispatcher()
                .Subscribe(e => FilterPackages());

            _hasLoaded = true;
        }

        private async Task LoadPackages()
        {
            _packages.Clear();
            Packages.Clear();

            var packages = await _chocolateyService.GetPackages();
            foreach (var packageViewModel in packages)
            {
                _packages.Add(packageViewModel);
                Packages.Add(packageViewModel);
            }
        }

        private void FilterPackages()
        {
            Packages.Clear();
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
               var query = MatchWord
                    ? _packages.Where(
                        package =>
                            String.Compare((package.Title ?? package.Id), SearchQuery,
                                StringComparison.OrdinalIgnoreCase) == 0)
                    : _packages.Where(
                        package =>
                            CultureInfo.CurrentCulture.CompareInfo.IndexOf((package.Title ?? package.Id), SearchQuery,
                                CompareOptions.OrdinalIgnoreCase) >= 0);

                query.ToList().ForEach(Packages.Add);
            }
            else
            {
                Packages = new ObservableCollection<IPackageViewModel>(_packages);
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set { SetPropertyValue(ref _searchQuery, value); }
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

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (sender is IChocolateyService && e is PackagesChangedEventArgs)
            {
                LoadPackages();
            }
            return true;
        }
    }
}
