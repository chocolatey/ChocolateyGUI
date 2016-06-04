// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Items
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using System.Windows;
    using AutoMapper;
    using Base;
    using Models;
    using NuGet;
    using Services;
    using Utilities;
    using Views.Controls;
    using MemoryCache = System.Runtime.Caching.MemoryCache;

    [DebuggerDisplay("Id = {Id}, Version = {Version}")]
    public class PackageViewModel : ObservableBase, IPackageViewModel, IWeakEventListener
    {
        private readonly MemoryCache _cache = MemoryCache.Default;

        private readonly IChocolateyPackageService _chocolateyService;

        private readonly INavigationService _navigationService;

        private readonly IMapper _mapper;

        private readonly IRemotePackageService _remotePackageService;

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
            IRemotePackageService remotePackageService,
            IChocolateyPackageService chocolateyService,
            INavigationService navigationService,
            IMapper mapper)
        {
            _remotePackageService = remotePackageService;
            _chocolateyService = chocolateyService;
            _navigationService = navigationService;
            _mapper = mapper;
            PackagesChangedEventManager.AddListener(_chocolateyService, this);
        }

        public string[] Authors
        {
            get { return _authors; }
            set { SetPropertyValue(ref _authors, value); }
        }

        public bool CanUpdate
        {
            get { return IsInstalled && LatestVersion != null && LatestVersion > Version; }
        }

        public string Copyright
        {
            get { return _copyright; }
            set { SetPropertyValue(ref _copyright, value); }
        }

        public DateTime Created
        {
            get { return _created; }
            set { SetPropertyValue(ref _created, value); }
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

        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set { SetPropertyValue(ref _lastUpdated, value); }
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

        public bool CanGoBack()
        {
            return _navigationService.CanGoBack;
        }

        public void GoBack()
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }

        public async Task Install()
        {
            await _chocolateyService.InstallPackage(Id, Version, Source).ConfigureAwait(false);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (sender is IChocolateyPackageService && e is PackagesChangedEventArgs)
            {
                NotifyPropertyChanged("IsInstalled");
                NotifyPropertyChanged("CanUpdate");
            }

            return true;
        }

        public async Task RetrieveLatestVersion()
        {
            SemanticVersion version;
            if ((version = (SemanticVersion)_cache.Get(string.Format("LatestVersion_{0}", Id))) != null)
            {
                LatestVersion = version;
                return;
            }

            var latest = await _remotePackageService.GetLatest(Id, IsPrerelease);

            if (latest != null)
            {
                version = latest.Version;

                if (latest.Source != null)
                {
                    Source = latest.Source;
                }
            }
            else
            {
                version = Version;
            }

            _cache.Set(
                string.Format("LatestVersion_{0}", Id),
                version,
                new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(1)
                    });

            LatestVersion = version;
        }

        public async Task Reinstall()
        {
            await _chocolateyService.InstallPackage(Id, Version, Source, true).ConfigureAwait(false);

            if (CanGoBack())
            {
                _navigationService.GoBack();
            }
        }

        public async Task Uninstall()
        {
            await _chocolateyService.UninstallPackage(Id, Version, true).ConfigureAwait(false);

            if (CanGoBack())
            {
                _navigationService.GoBack();
            }
        }

        public async Task Update()
        {
            await _chocolateyService.UpdatePackage(Id, Source).ConfigureAwait(false);

            if (CanGoBack())
            {
                _navigationService.GoBack();
            }
        }

        public async void ViewDetails()
        {
            if (DownloadCount == -1)
            {
                await PopulateDetails();
            }

            _navigationService.Navigate(typeof(PackageControl), this);
        }

        private async Task PopulateDetails()
        {
            var package = await _remotePackageService.GetByVersionAndIdAsync(_id, _version, _isPrerelease);
            _mapper.Map<IPackageViewModel, IPackageViewModel>(package, this);
        }
    }
}