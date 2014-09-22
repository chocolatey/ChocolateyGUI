// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using ChocolateyGui.Base;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.ViewModels.Items;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    public class LocalSourceControlViewModel : ObservableBase, ILocalSourceControlViewModel, IWeakEventListener
    {
        private readonly IChocolateyService _chocolateyService;
        private readonly ILogService _logService;
        private readonly List<IPackageViewModel> _packages;
        private readonly IProgressService _progressService;
        private bool _hasLoaded;

        private bool _matchWord;
        private ObservableCollection<IPackageViewModel> _packageViewModels;

        private string _searchQuery;

        private string _sortColumn;

        private bool _sortDescending;

        public LocalSourceControlViewModel(IChocolateyService chocolateyService, IProgressService progressService, Func<Type, ILogService> logFactory)
        {
            this._chocolateyService = chocolateyService;
            this._progressService = progressService;
            this._logService = logFactory(typeof(LocalSourceControlViewModel));
            PackagesChangedEventManager.AddListener(_chocolateyService, this);

            this.Packages = new ObservableCollection<IPackageViewModel>();
            this._packages = new List<IPackageViewModel>();
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

        public bool CanUpdateAll()
        {
            return this.Packages.Any(p => p.CanUpdate);
        }

        public async void Loaded(object sender, EventArgs args)
        {
            try
            {
                if (this._hasLoaded)
                {
                    return;
                }

                await this.LoadPackages();

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(e => e.EventArgs.PropertyName == "MatchWord" || e.EventArgs.PropertyName == "SearchQuery")
                    .ObserveOnDispatcher()
                    .Subscribe(e => this.FilterPackages());

                this._hasLoaded = true;

                var chocoPackage = this._packages.FirstOrDefault(p => p.Id.ToLower() == "chocolatey");
                if (chocoPackage != null && chocoPackage.CanUpdate)
                {
                    this._progressService.ShowMessageAsync("Chocolatey", "There's an update available for chocolatey.").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this._logService.Fatal("Local source control view model failed to load.", ex);
                throw;
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (sender is IChocolateyService && e is PackagesChangedEventArgs)
            {
                this.LoadPackages();
            }

            return true;
        }

        public async void UpdateAll()
        {
            try
            {
                await this._progressService.StartLoading("Packages", true);
                var token = this._progressService.GetCancellationToken();
                var packages = this.Packages.Where(p => p.CanUpdate).ToList();
                double current = 0.0f;
                foreach (var package in packages)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                }
                await _progressService.StopLoading();
            }
            catch (Exception ex)
            {
                this._logService.Fatal("Updated all has failed.", ex);
                throw;
            }
        }

        private void FilterPackages()
        {
            this.Packages.Clear();
            if (!string.IsNullOrWhiteSpace(this.SearchQuery))
            {
                var query = this.MatchWord
                     ? this._packages.Where(
                         package =>
                             string.Compare(package.Title ?? package.Id, SearchQuery,
                                 StringComparison.OrdinalIgnoreCase) == 0)
                     : this._packages.Where(
                         package =>
                             CultureInfo.CurrentCulture.CompareInfo.IndexOf((package.Title ?? package.Id), SearchQuery,
                                 CompareOptions.OrdinalIgnoreCase) >= 0);

                query.ToList().ForEach(Packages.Add);
            }
            else
            {
                this.Packages = new ObservableCollection<IPackageViewModel>(this._packages);
            }
        }

        private async Task LoadPackages()
        {
            try
            {
                this._packages.Clear();
                this.Packages.Clear();

                var packages = await this._chocolateyService.GetInstalledPackages();
                foreach (var packageViewModel in packages)
                {
                    this._packages.Add(packageViewModel);
                    this.Packages.Add(packageViewModel);

                    if (packageViewModel.LatestVersion == null)
                    {
                        packageViewModel.RetriveLatestVersion();
                    }
                }
            }
            catch (Exception ex)
            {
                this._logService.Fatal("Packages failed to load", ex);
                throw;
            }
        }
    }
}
