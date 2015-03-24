// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
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

    public class LocalSourceControlViewModel : ObservableBase, ILocalSourceControlViewModel, IWeakEventListener
    {
        private readonly IChocolateyPackageService _chocolateyService;
        private readonly ILogService _logService;
        private readonly List<IPackageViewModel> _packages;
        private readonly IProgressService _progressService;
        private bool _hasLoaded;
        private bool _matchWord;
        private bool _showOnlyPackagesWithUpdate;
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        private string _searchQuery;
        private string _sortColumn;
        private bool _sortDescending;

        public LocalSourceControlViewModel(IChocolateyPackageService chocolateyService, IProgressService progressService, Func<Type, ILogService> logFactory)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException("logFactory");
            }

            this._chocolateyService = chocolateyService;
            this._progressService = progressService;
            this._logService = logFactory(typeof(LocalSourceControlViewModel));
            PackagesChangedEventManager.AddListener(this._chocolateyService, this);

            this.Packages = new ObservableCollection<IPackageViewModel>();
            this._packages = new List<IPackageViewModel>();
        }

        public bool MatchWord
        {
            get
            {
                return this._matchWord;
            }

            set
            {
                this.SetPropertyValue(ref this._matchWord, value);
                this.ShowOnlyPackagesWithUpdate = false;
            }
        }

        public bool ShowOnlyPackagesWithUpdate
        {
            get { return this._showOnlyPackagesWithUpdate; }
            set { this.SetPropertyValue(ref this._showOnlyPackagesWithUpdate, value); }
        }

        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return this._packageViewModels; }
            set { this.SetPropertyValue(ref this._packageViewModels, value); }
        }

        public string SearchQuery
        {
            get
            {
                return this._searchQuery;
            }

            set
            {
                this.SetPropertyValue(ref this._searchQuery, value);
                this.ShowOnlyPackagesWithUpdate = false;
            }
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

        public bool CanRefreshPackages()
        {
            return this._hasLoaded;
        }

        public async void Loaded(object sender, EventArgs e)
        {
            try
            {
                if (this._hasLoaded)
                {
                    return;
                }

                await this.LoadPackages();

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(eventPattern => eventPattern.EventArgs.PropertyName == "MatchWord" || eventPattern.EventArgs.PropertyName == "SearchQuery" || eventPattern.EventArgs.PropertyName == "ShowOnlyPackagesWithUpdate")
                    .ObserveOnDispatcher()
                    .Subscribe(eventPattern => this.FilterPackages());

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
            if (sender is IChocolateyPackageService && e is PackagesChangedEventArgs)
            {
                this.LoadPackages().ConfigureAwait(false);
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
                        await this._progressService.StopLoading();
                        return;
                    }

                    this._progressService.Report(Math.Min((current++) / packages.Count, 100));
                    await package.Update();
                }

                await this._progressService.StopLoading();
            }
            catch (Exception ex)
            {
                this._logService.Fatal("Updated all has failed.", ex);
                throw;
            }
        }

        public async void RefreshPackages()
        {
            this._chocolateyService.ClearPackageCache();
            await this.LoadPackages();
        }

        private void FilterPackages()
        {
            this.Packages.Clear();
            if (!string.IsNullOrWhiteSpace(this.SearchQuery))
            {
                var query = this.MatchWord
                     ? this._packages.Where(
                         package =>
                         string.Compare(
                             package.Title ?? package.Id,
                             this.SearchQuery,
                             StringComparison.OrdinalIgnoreCase) == 0)
                                : this._packages.Where(
                         package =>
                         CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                             package.Title ?? package.Id,
                             this.SearchQuery,
                             CompareOptions.OrdinalIgnoreCase) >= 0);

                query.ToList().ForEach(this.Packages.Add);
            }
            else if (this.ShowOnlyPackagesWithUpdate)
            {
                var query = this._packages.Where(p => p.CanUpdate == true);
                query.ToList().ForEach(this.Packages.Add);
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
                        packageViewModel.RetrieveLatestVersion().ConfigureAwait(false);
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
