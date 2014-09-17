using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using ChocolateyGui.Base;
using ChocolateyGui.Models;
using ChocolateyGui.Services;
using ChocolateyGui.Utilities;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels.Controls
{
    public class LocalSourceControlViewModel : ObservableBase, ILocalSourceControlViewModel, IWeakEventListener
    {
        private readonly List<IPackageViewModel> _packages;
        private bool _hasLoaded;

        private ObservableCollection<IPackageViewModel> _packageViewModels; 
        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return _packageViewModels; }
            set { SetPropertyValue(ref _packageViewModels, value); }
        }

        private readonly IChocolateyService _chocolateyService;
        private readonly IProgressService _progressService;
        private readonly ILogService _logService;
        public LocalSourceControlViewModel(IChocolateyService chocolateyService, IProgressService progressService, Func<Type, ILogService> logFactory)
        {
            _chocolateyService = chocolateyService;
            _progressService = progressService;
            _logService = logFactory(typeof(LocalSourceControlViewModel));
            PackagesChangedEventManager.AddListener(_chocolateyService, this);

            Packages = new ObservableCollection<IPackageViewModel>();
            _packages = new List<IPackageViewModel>();
        }

        public async void Loaded(object sender, EventArgs args)
        {
            try
            {
                if (_hasLoaded)
                    return;

                await LoadPackages();

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == "MatchWord" || e.EventArgs.PropertyName == "SearchQuery")
                    .ObserveOnDispatcher()
                    .Subscribe(e => FilterPackages());

                _hasLoaded = true;

                var chocoPackage = _packages.FirstOrDefault(p => p.Id.ToLower() == "chocolatey");
                if (chocoPackage != null && chocoPackage.CanUpdate)
                    _progressService.ShowMessageAsync("Chocolatey", "There's an update available for chocolatey.");
            }
            catch (Exception ex)
            {
                _logService.Fatal("Local source control view model failed to load.", ex);
                throw;
            }
        }

        private async Task LoadPackages()
        {
            try
            {
                _packages.Clear();
                Packages.Clear();

                var packages = await _chocolateyService.GetInstalledPackages();
                foreach (var packageViewModel in packages)
                {
                    _packages.Add(packageViewModel);
                    Packages.Add(packageViewModel);

                    if (packageViewModel.LatestVersion == null)
                        packageViewModel.RetriveLatestVersion();
                }
            }
            catch (Exception ex)
            {
                _logService.Fatal("Packages failed to load", ex);
                throw;
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

        public bool CanUpdateAll()
        {
            return Packages.Any(p => p.CanUpdate);
        }

        public async void UpdateAll()
        {
            try
            {
                await _progressService.StartLoading("Packages", true);
                var token = _progressService.GetCancellationToken();
                var packages = Packages.Where(p => p.CanUpdate).ToList();
                double current = 0.0f;
                foreach (var package in packages)
                {
                    if (token.IsCancellationRequested)
                        return;
                    _progressService.Report(Math.Min((current++) / packages.Count, 100));
                    await package.Update();
                }
                await _progressService.StopLoading();
            }
            catch (Exception ex)
            {
                _logService.Fatal("Updated all has failed.", ex);
                throw;
            }
        }
    }
}
