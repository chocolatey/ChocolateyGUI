// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalSourceViewModel.cs">
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
using System.Xml;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;
using ChocolateyGui.Utilities.Extensions;
using ChocolateyGui.ViewModels.Items;
using Serilog;

namespace ChocolateyGui.ViewModels
{
    public sealed class LocalSourceViewModel : Screen, ISourceViewModelBase, IHandleWithTask<PackageChangedMessage>
    {
        private static readonly ILogger Logger = Log.ForContext<LocalSourceViewModel>();
        private readonly IChocolateyPackageService _chocolateyService;
        private readonly List<IPackageViewModel> _packages;
        private readonly IPersistenceService _persistenceService;
        private readonly IProgressService _progressService;
        private readonly IEventAggregator _eventAggregator;
        private bool _exportAll = true;
        private bool _hasLoaded;
        private bool _matchWord;
        private ObservableCollection<IPackageViewModel> _packageViewModels;
        private string _searchQuery;
        private bool _showOnlyPackagesWithUpdate;
        private string _sortColumn;
        private bool _sortDescending;
        private bool _isLoading;
        private bool _firstLoadComplete = true;

        public LocalSourceViewModel(
            IChocolateyPackageService chocolateyService,
            IProgressService progressService,
            IPersistenceService persistenceService,
            IEventAggregator eventAggregator,
            string displayName)
        {
            _chocolateyService = chocolateyService;
            _progressService = progressService;
            _persistenceService = persistenceService;

            DisplayName = displayName;
            Packages = new ObservableCollection<IPackageViewModel>();
            _packages = new List<IPackageViewModel>();
            if (eventAggregator == null)
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public bool ShowOnlyPackagesWithUpdate
        {
            get { return _showOnlyPackagesWithUpdate; }
            set { this.SetPropertyValue(ref _showOnlyPackagesWithUpdate, value); }
        }

        public bool MatchWord
        {
            get { return _matchWord; }
            set { this.SetPropertyValue(ref _matchWord, value); }
        }

        public ObservableCollection<IPackageViewModel> Packages
        {
            get { return _packageViewModels; }
            set { this.SetPropertyValue(ref _packageViewModels, value); }
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set { this.SetPropertyValue(ref _searchQuery, value); }
        }

        public string SortColumn
        {
            get { return _sortColumn; }
            set { this.SetPropertyValue(ref _sortColumn, value); }
        }

        public bool SortDescending
        {
            get { return _sortDescending; }
            set { this.SetPropertyValue(ref _sortDescending, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.SetPropertyValue(ref _isLoading, value); }
        }

        public bool FirstLoadComplete
        {
            get { return _firstLoadComplete; }
            set { this.SetPropertyValue(ref _firstLoadComplete, value); }
        }

        public bool CanUpdateAll()
        {
            return Packages.Any(p => p.CanUpdate);
        }

        public async void UpdateAll()
        {
            try
            {
                await _progressService.StartLoading(Resources.LocalSourceViewModel_Packages, true);
                var token = _progressService.GetCancellationToken();
                var packages = Packages.Where(p => p.CanUpdate && !p.IsPinned).ToList();
                double current = 0.0f;
                foreach (var package in packages)
                {
                    if (token.IsCancellationRequested)
                    {
                        await _progressService.StopLoading();
                        return;
                    }

                    _progressService.Report(Math.Min(current++ / packages.Count, 100));
                    await package.Update();
                }

                await _progressService.StopLoading();
                ShowOnlyPackagesWithUpdate = false;
                RefreshPackages();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Updated all has failed.", ex);
                throw;
            }
        }

        public void ExportAll()
        {
            _exportAll = false;

            try
            {
                var fileStream = _persistenceService.SaveFile("*.config", Resources.LocalSourceViewModel_ConfigFiles);

                if (fileStream == null)
                {
                    return;
                }

                var settings = new XmlWriterSettings { Indent = true };

                using (var xw = XmlWriter.Create(fileStream, settings))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("packages");

                    foreach (var package in Packages)
                    {
                        xw.WriteStartElement("package");
                        xw.WriteAttributeString("id", package.Id);
                        xw.WriteAttributeString("version", package.Version.ToString());
                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Export all has failed.", ex);
                throw;
            }
            finally
            {
                _exportAll = true;
            }
        }

        public bool CanExportAll()
        {
            return _exportAll;
        }

        public bool CanRefreshPackages()
        {
            return _hasLoaded && !IsLoading;
        }

        public async void RefreshPackages()
        {
            await LoadPackages();
        }

        public async Task Handle(PackageChangedMessage message)
        {
            await LoadPackages();
        }

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        protected override async void OnInitialize()
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                if (_hasLoaded)
                {
                    return;
                }

                await LoadPackages();

                Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                    .Where(
                        eventPattern =>
                            eventPattern.EventArgs.PropertyName == "MatchWord" ||
                            eventPattern.EventArgs.PropertyName == "SearchQuery" ||
                            eventPattern.EventArgs.PropertyName == "ShowOnlyPackagesWithUpdate")
                    .ObserveOnDispatcher()
                    .Subscribe(eventPattern => FilterPackages());

                _hasLoaded = true;

                var chocoPackage = _packages.FirstOrDefault(p => p.Id.ToLower() == "chocolatey");
                if (chocoPackage != null && chocoPackage.CanUpdate)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    _progressService.ShowMessageAsync(Resources.LocalSourceViewModel_Chocolatey, Resources.LocalSourceViewModel_UpdateAvailableForChocolatey)
                        .ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Local source control view model failed to load.", ex);
                throw;
            }
        }

        private void FilterPackages()
        {
            Packages.Clear();
            var query = _packages.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                query = MatchWord
                    ? query.Where(
                        package =>
                            string.Compare(
                                package.Title ?? package.Id,
                                SearchQuery,
                                StringComparison.OrdinalIgnoreCase) == 0)
                    : query.Where(
                        package =>
                            CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                                package.Title ?? package.Id,
                                SearchQuery,
                                CompareOptions.OrdinalIgnoreCase) >= 0);
            }

            if (ShowOnlyPackagesWithUpdate)
            {
                query = query.Where(p => p.CanUpdate && !p.IsPinned);
            }

            Packages = new ObservableCollection<IPackageViewModel>(query);
        }

        private async Task LoadPackages()
        {
            IsLoading = true;
            try
            {
                _packages.Clear();
                Packages.Clear();

                var packages = await _chocolateyService.GetInstalledPackages();
                foreach (var packageViewModel in packages)
                {
                    _packages.Add(packageViewModel);
                }

                FilterPackages();

                FirstLoadComplete = false;

                var updates = await _chocolateyService.GetOutdatedPackages();
                foreach (var update in updates)
                {
                    await _eventAggregator.PublishOnUIThreadAsync(new PackageHasUpdateMessage(update.Item1, update.Item2));
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Packages failed to load", ex);
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}