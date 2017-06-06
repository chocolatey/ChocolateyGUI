﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using ChocolateyGui.Base;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;
using NuGet;
using Action = System.Action;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace ChocolateyGui.ViewModels.Items
{
    [DebuggerDisplay("Id = {Id}, Version = {Version}")]
    public class PackageViewModel :
        ObservableBase,
        IPackageViewModel,
        IHandleWithTask<PackageChangedMessage>,
        IHandle<PackageHasUpdateMessage>
    {
        private static readonly Serilog.ILogger Logger = Serilog.Log.ForContext<PackageViewModel>();

        private readonly MemoryCache _cache = MemoryCache.Default;

        private readonly IChocolateyPackageService _chocolateyService;
        private readonly IEventAggregator _eventAggregator;

        private readonly IMapper _mapper;
        private readonly IProgressService _progressService;

        private string[] _authors;

        private string _copyright;

        private DateTime _created;

        private string _dependencies;

        private string _description;

        private int _downloadCount;

        private string _galleryDetailsUrl;

        private string _iconUrl = string.Empty;

        private string _id;

        private bool _isAbsoluteLatestVersion;

        private bool _isInstalled;

        private bool _isPinned;

        private bool _isLatestVersion;

        private bool _isPrerelease;

        private string _language;

        private DateTime _lastUpdated;

        private SemanticVersion _latestVersion;

        private string _licenseUrl = string.Empty;

        private string[] _owners;

        private string _packageHash;

        private string _packageHashAlgorithm;

        private long _packageSize;

        private string _projectUrl = string.Empty;

        private DateTimeOffset _published;

        private string _releaseNotes;

        private string _reportAbuseUrl = string.Empty;

        private string _requireLicenseAcceptance;

        private Uri _source;

        private string _summary;

        private string _tags;

        private string _title;

        private SemanticVersion _version;

        private int _versionDownloadCount;

        public PackageViewModel(
            IChocolateyPackageService chocolateyService,
            IEventAggregator eventAggregator,
            IMapper mapper,
            IProgressService progressService)
        {
            _chocolateyService = chocolateyService;
            _eventAggregator = eventAggregator;
            _mapper = mapper;
            _progressService = progressService;
            eventAggregator?.Subscribe(this);
        }

        public DateTime Created
        {
            get { return _created; }
            set { SetPropertyValue(ref _created, value); }
        }

        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set { SetPropertyValue(ref _lastUpdated, value); }
        }

        public string[] Authors
        {
            get { return _authors; }
            set { SetPropertyValue(ref _authors, value); }
        }

        public bool CanUpdate => IsInstalled && !IsPinned && LatestVersion != null && LatestVersion > Version;

        public string Copyright
        {
            get { return _copyright; }
            set { SetPropertyValue(ref _copyright, value); }
        }

        public string Dependencies
        {
            get { return _dependencies; }
            set { SetPropertyValue(ref _dependencies, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetPropertyValue(ref _description, value); }
        }

        public int DownloadCount
        {
            get { return _downloadCount; }
            set { SetPropertyValue(ref _downloadCount, value); }
        }

        public string GalleryDetailsUrl
        {
            get { return _galleryDetailsUrl; }
            set { SetPropertyValue(ref _galleryDetailsUrl, value); }
        }

        public string IconUrl
        {
            get { return _iconUrl; }
            set { SetPropertyValue(ref _iconUrl, value); }
        }

        public string Id
        {
            get { return _id; }
            set { SetPropertyValue(ref _id, value); }
        }

        public bool IsAbsoluteLatestVersion
        {
            get { return _isAbsoluteLatestVersion; }
            set { SetPropertyValue(ref _isAbsoluteLatestVersion, value); }
        }

        public bool IsInstalled
        {
            get { return _isInstalled; }
            set { SetPropertyValue(ref _isInstalled, value); }
        }

        public bool IsPinned
        {
            get
            {
                return _isPinned;
            }

            set
            {
                SetPropertyValue(ref _isPinned, value);
                NotifyPropertyChanged(nameof(CanUpdate));
            }
        }

        public bool IsLatestVersion
        {
            get { return _isLatestVersion; }
            set { SetPropertyValue(ref _isLatestVersion, value); }
        }

        public bool IsPrerelease
        {
            get { return _isPrerelease; }
            set { SetPropertyValue(ref _isPrerelease, value); }
        }

        public string Language
        {
            get { return _language; }
            set { SetPropertyValue(ref _language, value); }
        }

        public SemanticVersion LatestVersion
        {
            get { return _latestVersion; }
            set { SetPropertyValue(ref _latestVersion, value); }
        }

        public string LicenseUrl
        {
            get { return _licenseUrl; }
            set { SetPropertyValue(ref _licenseUrl, value); }
        }

        public string[] Owners
        {
            get { return _owners; }
            set { SetPropertyValue(ref _owners, value); }
        }

        public string PackageHash
        {
            get { return _packageHash; }
            set { SetPropertyValue(ref _packageHash, value); }
        }

        public string PackageHashAlgorithm
        {
            get { return _packageHashAlgorithm; }
            set { SetPropertyValue(ref _packageHashAlgorithm, value); }
        }

        public long PackageSize
        {
            get { return _packageSize; }
            set { SetPropertyValue(ref _packageSize, value); }
        }

        public string ProjectUrl
        {
            get { return _projectUrl; }
            set { SetPropertyValue(ref _projectUrl, value); }
        }

        public DateTimeOffset Published
        {
            get { return _published; }
            set { SetPropertyValue(ref _published, value); }
        }

        public string ReleaseNotes
        {
            get { return _releaseNotes; }
            set { SetPropertyValue(ref _releaseNotes, value); }
        }

        public string ReportAbuseUrl
        {
            get { return _reportAbuseUrl; }
            set { SetPropertyValue(ref _reportAbuseUrl, value); }
        }

        public string RequireLicenseAcceptance
        {
            get { return _requireLicenseAcceptance; }
            set { SetPropertyValue(ref _requireLicenseAcceptance, value); }
        }

        public Uri Source
        {
            get { return _source; }
            set { SetPropertyValue(ref _source, value); }
        }

        public string Summary
        {
            get { return _summary; }
            set { SetPropertyValue(ref _summary, value); }
        }

        public string Tags
        {
            get { return _tags; }
            set { SetPropertyValue(ref _tags, value); }
        }

        public string Title
        {
            get { return string.IsNullOrWhiteSpace(_title) ? Id : _title; }
            set { SetPropertyValue(ref _title, value); }
        }

        public SemanticVersion Version
        {
            get { return _version; }
            set { SetPropertyValue(ref _version, value); }
        }

        public int VersionDownloadCount
        {
            get { return _versionDownloadCount; }
            set { SetPropertyValue(ref _versionDownloadCount, value); }
        }

        public async Task Install()
        {
            try
            {
                using (await StartProgressDialog(Resources.PackageViewModel_InstallingPackage, Resources.PackageViewModel_InstallingPackage, Id))
                {
                    await _chocolateyService.InstallPackage(Id, Version, Source).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while installing {Id}, version {Version}.", Id, Version);
                await _progressService.ShowMessageAsync(
                    Resources.PackageViewModel_FailedToInstall,
                    string.Format(Resources.PackageViewModel_RanIntoInstallError, Id, ex.Message));
            }
        }

        public async Task Reinstall()
        {
            try
            {
                using (await StartProgressDialog(Resources.PackageViewModel_ReinstallingPackage, Resources.PackageViewModel_ReinstallingPackage, Id))
                {
                    await _chocolateyService.InstallPackage(Id, Version, Source, true).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while reinstalling {Id}, version {Version}.", Id, Version);
                await _progressService.ShowMessageAsync(
                    Resources.PackageViewModel_FailedToReinstall,
                    string.Format(Resources.PackageViewModel_RanIntoReinstallError, Id, ex.Message));
            }

            await _eventAggregator.PublishOnUIThreadAsync(new PackageChangedMessage(Id, PackageChangeType.Installed, Version));
        }

        public async Task Uninstall()
        {
            try
            {
                using (await StartProgressDialog(Resources.PackageViewModel_UninstallingPackage, Resources.PackageViewModel_UninstallingPackage, Id))
                {
                    await _chocolateyService.UninstallPackage(Id, Version, true).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while uninstalling {Id}, version {Version}.", Id, Version);
                await _progressService.ShowMessageAsync(
                    Resources.PackageViewModel_FailedToUninstall,
                    string.Format(Resources.PackageViewModel_RanIntoUninstallError, Id, ex.Message));
            }
        }

        public async Task Update()
        {
            try
            {
                using (await StartProgressDialog(Resources.PackageViewModel_UpdatingPackage, Resources.PackageViewModel_UpdatingPackage, Id))
                {
                    await _chocolateyService.UpdatePackage(Id, Source).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while updating {Id}, version {Version}.", Id, Version);
                await _progressService.ShowMessageAsync(
                    Resources.PackageViewModel_FailedToUpdate,
                    string.Format(Resources.PackageViewModel_RanIntoUpdateError, Id, ex.Message));
            }
        }

        public async Task Pin()
        {
            try
            {
                using (await StartProgressDialog(Resources.PackageViewModel_PinningPackage, Resources.PackageViewModel_PinningPackage, Id))
                {
                    await _chocolateyService.PinPackage(Id, Version).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while pinning {Id}, version {Version}.", Id, Version);
                await _progressService.ShowMessageAsync(
                    Resources.PackageViewModel_FailedToPin,
                    string.Format(Resources.PackageViewModel_RanIntoPinningError, Id, ex.Message));
            }
        }

        public async Task Unpin()
        {
            try
            {
                using (await StartProgressDialog(Resources.PackageViewModel_UnpinningPackage, Resources.PackageViewModel_UnpinningPackage, Id))
                {
                    await _chocolateyService.UnpinPackage(Id, Version).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while unpinning {Id}, version {Version}.", Id, Version);
                await _progressService.ShowMessageAsync(
                    Resources.PackageViewModel_FailedToUnpin,
                    string.Format(Resources.PackageViewModel_RanIntoUnpinError, Id, ex.Message));
            }
        }

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public async void ViewDetails()
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            if (DownloadCount == -1)
            {
                await PopulateDetails().ConfigureAwait(false);
            }

            await _eventAggregator.PublishOnUIThreadAsync(new ShowPackageDetailsMessage(this)).ConfigureAwait(false);
        }

        public Task Handle(PackageChangedMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Id != Id)
            {
                return Task.FromResult(true);
            }

            switch (message.ChangeType)
            {
                case PackageChangeType.Installed:
                    IsInstalled = true;
                    break;
                case PackageChangeType.Uninstalled:
                    IsInstalled = false;
                    break;
                case PackageChangeType.Pinned:
                    IsPinned = true;
                    break;
                case PackageChangeType.Unpinned:
                    IsPinned = false;
                    break;
            }

            return Task.FromResult(true);
        }

        public void Handle(PackageHasUpdateMessage message)
        {
            if (message.Id != Id)
            {
                return;
            }

            LatestVersion = message.Version;
            IsLatestVersion = false;
        }

        private async Task PopulateDetails()
        {
            await _progressService.StartLoading(Resources.PackageViewModel_LoadingPackageInfo);
            try
            {
                var package =
                    await
                        _chocolateyService.GetByVersionAndIdAsync(_id, _version, _isPrerelease).ConfigureAwait(false);
                _mapper.Map<IPackageViewModel, IPackageViewModel>(package, this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while populating details for package {Id}, version {Version}.", Id, Version);
            }
            finally
            {
                await _progressService.StopLoading();
            }
        }

        private async Task<IDisposable> StartProgressDialog(string commandString, string initialProgressText, string id = "")
        {
            await _progressService.StartLoading(string.Format(Resources.PackageViewModel_StartLoadingFormat, commandString, id));
            _progressService.WriteMessage(initialProgressText);
            return new DisposableAction(() => _progressService.StopLoading());
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action _disposeAction;

            public DisposableAction(System.Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                _disposeAction();
            }
        }
    }
}