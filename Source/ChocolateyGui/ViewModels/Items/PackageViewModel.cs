// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Items
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.Caching;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using ChocolateyGui.Base;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.Views.Controls;
    
    [DebuggerDisplay("Id = {Id}, Version = {Version}")]
    public class PackageViewModel : ObservableBase, IPackageViewModel, IWeakEventListener
    {
        private readonly MemoryCache _cache = MemoryCache.Default;

        private readonly IChocolateyService _chocolateyService;

        private readonly INavigationService _navigationService;

        private readonly IPackageService _packageService;

        private string _authors;

        private string _copyright;

        private DateTime _created;

        private string _dependencies;

        private string _description;

        private int _downloadCount;

        private string _galleryDetailsUrl;

        private string _iconUrl = string.Empty;

        private string _id;

        private bool _isAbsoluteLatestVersion;

        private Lazy<bool> _isInstalled;

        private bool _isLatestVersion;

        private bool _isPrerelease;

        private string _language;

        private DateTime _lastUpdated;

        private SemanticVersion _latestVersion;

        private string _licenseUrl = string.Empty;

        private string _owners;

        private string _packageHash;

        private string _packageHashAlgorithm;

        private long _packageSize;

        private string _projectUrl = string.Empty;

        private DateTime _published;

        private string _releaseNotes;

        private string _reportAbuseUrl = string.Empty;

        private string _requireLicenseAcceptance;

        private Uri _source;

        private string _summary;

        private string _tags;

        private string _title;

        private SemanticVersion _version;

        private int _versionDownloadCount;

        public PackageViewModel(IPackageService packageService, IChocolateyService chocolateyService, INavigationService navigationService)
        {
            this._packageService = packageService;
            this._chocolateyService = chocolateyService;
            this._navigationService = navigationService;
            PackagesChangedEventManager.AddListener(this._chocolateyService, this);

            this._isInstalled = new Lazy<bool>(() => this._chocolateyService.IsPackageInstalled(this.Id, this.Version));
        }

        public string Authors
        {
            get { return this._authors; }
            set { this.SetPropertyValue(ref this._authors, value); }
        }

        public bool CanUpdate
        {
            get { return this.IsInstalled && this.LatestVersion != null && this.LatestVersion > this.Version; }
        }

        public string Copyright
        {
            get { return this._copyright; }
            set { this.SetPropertyValue(ref this._copyright, value); }
        }

        public DateTime Created
        {
            get { return this._created; }
            set { this.SetPropertyValue(ref this._created, value); }
        }

        public string Dependencies
        {
            get { return this._dependencies; }
            set { this.SetPropertyValue(ref this._dependencies, value); }
        }

        public string Description
        {
            get { return this._description; }
            set { this.SetPropertyValue(ref this._description, value); }
        }

        public int DownloadCount
        {
            get { return this._downloadCount; }
            set { this.SetPropertyValue(ref this._downloadCount, value); }
        }

        public string GalleryDetailsUrl
        {
            get { return this._galleryDetailsUrl; }
            set { this.SetPropertyValue(ref this._galleryDetailsUrl, value); }
        }

        public string IconUrl
        {
            get { return this._iconUrl; }
            set { this.SetPropertyValue(ref this._iconUrl, value); }
        }

        public string Id
        {
            get { return this._id; }
            set { this.SetPropertyValue(ref this._id, value); }
        }

        public bool IsAbsoluteLatestVersion
        {
            get { return this._isAbsoluteLatestVersion; }
            set { this.SetPropertyValue(ref this._isAbsoluteLatestVersion, value); }
        }

        public bool IsInstalled
        {
            get { return this._isInstalled.Value; }
        }

        public bool IsLatestVersion
        {
            get { return this._isLatestVersion; }
            set { this.SetPropertyValue(ref this._isLatestVersion, value); }
        }

        public bool IsPrerelease
        {
            get { return this._isPrerelease; }
            set { this.SetPropertyValue(ref this._isPrerelease, value); }
        }

        public string Language
        {
            get { return this._language; }
            set { this.SetPropertyValue(ref this._language, value); }
        }

        public DateTime LastUpdated
        {
            get { return this._lastUpdated; }
            set { this.SetPropertyValue(ref this._lastUpdated, value); }
        }

        public SemanticVersion LatestVersion
        {
            get { return this._latestVersion; }
            set { this.SetPropertyValue(ref this._latestVersion, value); }
        }

        public string LicenseUrl
        {
            get { return this._licenseUrl; }
            set { this.SetPropertyValue(ref this._licenseUrl, value); }
        }

        public string Owners
        {
            get { return this._owners; }
            set { this.SetPropertyValue(ref this._owners, value); }
        }

        public string PackageHash
        {
            get { return this._packageHash; }
            set { this.SetPropertyValue(ref this._packageHash, value); }
        }

        public string PackageHashAlgorithm
        {
            get { return this._packageHashAlgorithm; }
            set { this.SetPropertyValue(ref this._packageHashAlgorithm, value); }
        }

        public long PackageSize
        {
            get { return this._packageSize; }
            set { this.SetPropertyValue(ref this._packageSize, value); }
        }

        public string ProjectUrl
        {
            get { return this._projectUrl; }
            set { this.SetPropertyValue(ref this._projectUrl, value); }
        }

        public DateTime Published
        {
            get { return this._published; }
            set { this.SetPropertyValue(ref this._published, value); }
        }

        public string ReleaseNotes
        {
            get { return this._releaseNotes; }
            set { this.SetPropertyValue(ref this._releaseNotes, value); }
        }

        public string ReportAbuseUrl
        {
            get { return this._reportAbuseUrl; }
            set { this.SetPropertyValue(ref this._reportAbuseUrl, value); }
        }

        public string RequireLicenseAcceptance
        {
            get { return this._requireLicenseAcceptance; }
            set { this.SetPropertyValue(ref this._requireLicenseAcceptance, value); }
        }

        public Uri Source
        {
            get { return this._source; }
            set { this.SetPropertyValue(ref this._source, value); }
        }

        public string Summary
        {
            get { return this._summary; }
            set { this.SetPropertyValue(ref this._summary, value); }
        }

        public string Tags
        {
            get { return this._tags; }
            set { this.SetPropertyValue(ref this._tags, value); }
        }

        public string Title
        {
            get { return string.IsNullOrWhiteSpace(this._title) ? this.Id : this._title; }
            set { this.SetPropertyValue(ref this._title, value); }
        }

        public SemanticVersion Version
        {
            get { return this._version; }
            set { this.SetPropertyValue(ref this._version, value); }
        }

        public int VersionDownloadCount
        {
            get { return this._versionDownloadCount; }
            set { this.SetPropertyValue(ref this._versionDownloadCount, value); }
        }

        private string MemoryCacheKey
        {
            get { return string.Format(CultureInfo.CurrentCulture, "PackageViewModel.{0}{1}", this.Id, this.Version); }
        }

        public bool CanGoBack()
        {
            return this._navigationService.CanGoBack;
        }

        public async Task EnsureIsLoaded()
        {
            if (this.Published == DateTime.MinValue)
            {
                await this._packageService.EnsureIsLoaded(this, this.Source);
            }
        }

        public void GoBack()
        {
            if (this._navigationService.CanGoBack)
            {
                this._navigationService.GoBack();
            }
        }

        public async Task Install()
        {
            await this._chocolateyService.InstallPackage(this.Id, this.Version, this.Source).ConfigureAwait(false);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (sender is IChocolateyService && e is PackagesChangedEventArgs)
            {
                this._isInstalled = new Lazy<bool>(() => this._chocolateyService.IsPackageInstalled(this.Id, this.Version));
                this.NotifyPropertyChanged("IsInstalled");
                this.NotifyPropertyChanged("CanUpdate");
            }

            return true;
        }

        public async Task RetrieveLatestVersion()
        {
            SemanticVersion version;
            if ((version = (SemanticVersion)this._cache.Get(string.Format("LatestVersion_{0}", this.Id))) != null)
            {
                this.LatestVersion = version;
                return;
            }

            var latest = await this._packageService.GetLatest(this.Id, this.IsPrerelease, this.Source);

            if (latest != null)
            {
                version = latest.Version;

                if (latest.Source != null)
                {
                    this.Source = latest.Source;
                }
            }
            else
            {
                version = this.Version;
            }

            this._cache.Set(
                string.Format("LatestVersion_{0}", this.Id),
                version,
                new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(1)
                    });

            this.LatestVersion = version;
        }

        public async Task Reinstall()
        {
            await this._chocolateyService.ReinstallPackage(this.Id, this.Version, this.Source).ConfigureAwait(false);

            if (this.CanGoBack())
            {
                this._navigationService.GoBack();
            }
        }

        public async Task Uninstall()
        {
            await this._chocolateyService.UninstallPackage(this.Id, this.Version, true).ConfigureAwait(false);

            if (this.CanGoBack())
            {
                this._navigationService.GoBack();
            }
        }

        public async Task Update()
        {
            await this._chocolateyService.UpdatePackage(this.Id, this.Source).ConfigureAwait(false);

            if (this.CanGoBack())
            {
                this._navigationService.GoBack();
            }
        }

        public void ViewDetails()
        {
            this._navigationService.Navigate(typeof(PackageControl), this);
        }

        private string MemoryCachePropertyKey([CallerMemberName] string propertyName = "")
        {
            return this.MemoryCacheKey + "." + propertyName;
        }
    }
}