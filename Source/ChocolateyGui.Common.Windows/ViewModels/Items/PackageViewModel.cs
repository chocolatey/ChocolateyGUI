// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using ChocolateyGui.Common.Base;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.Views;
using MahApps.Metro.Controls.Dialogs;
using NuGet.Versioning;
using Action = System.Action;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace ChocolateyGui.Common.Windows.ViewModels.Items
{
    [DebuggerDisplay("Id = {Id}, Version = {Version}")]
    public class PackageViewModel :
        ObservableBase,
        IPackageViewModel,
        IHandle<PackageHasUpdateMessage>,
        IHandle<FeatureModifiedMessage>
    {
        private static readonly Serilog.ILogger Logger = Serilog.Log.ForContext<PackageViewModel>();

        private readonly MemoryCache _cache = MemoryCache.Default;

        private readonly IChocolateyService _chocolateyService;
        private readonly IEventAggregator _eventAggregator;

        private readonly IMapper _mapper;
        private readonly IDialogService _dialogService;
        private readonly IProgressService _progressService;

        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;
        private readonly IConfigService _configService;
        private readonly IAllowedCommandsService _allowedCommandsService;
        private readonly IPackageArgumentsService _packageArgumentsService;
        private readonly IPersistenceService _persistenceService;

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

        private bool _isSideBySide;

        private bool _isLatestVersion;

        private bool _isPrerelease;

        private string _language;

        private DateTime _lastUpdated;

        private NuGetVersion _latestVersion;

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

        private NuGetVersion _version;

        private int _versionDownloadCount;

        public PackageViewModel(
            IChocolateyService chocolateyService,
            IEventAggregator eventAggregator,
            IMapper mapper,
            IDialogService dialogService,
            IProgressService progressService,
            IChocolateyGuiCacheService chocolateyGuiCacheService,
            IConfigService configService,
            IAllowedCommandsService allowedCommandsService,
            IPackageArgumentsService packageArgumentsService,
            IPersistenceService persistenceService)
        {
            _chocolateyService = chocolateyService;
            _eventAggregator = eventAggregator;
            _mapper = mapper;
            _dialogService = dialogService;
            _progressService = progressService;
            eventAggregator?.Subscribe(this);
            _chocolateyGuiCacheService = chocolateyGuiCacheService;
            _configService = configService;
            _allowedCommandsService = allowedCommandsService;
            _packageArgumentsService = packageArgumentsService;
            _persistenceService = persistenceService;
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

        public bool CanInstall => !IsInstalled;

        public bool IsInstallAllowed => _allowedCommandsService.IsInstallCommandAllowed;

        public bool CanReinstall => IsInstalled;

        public bool IsReinstallAllowed => _allowedCommandsService.IsInstallCommandAllowed;

        public bool CanUninstall => IsInstalled;

        public bool IsUninstallAllowed => _allowedCommandsService.IsUninstallCommandAllowed;

        public bool CanUpdate => IsInstalled && !IsPinned && !IsSideBySide && !IsLatestVersion;

        public bool IsUpgradeAllowed => _allowedCommandsService.IsUpgradeCommandAllowed;

        public bool CanPin => !IsPinned && IsInstalled;

        public bool IsPinAllowed => _allowedCommandsService.IsPinCommandAllowed;

        public bool CanUnpin => IsPinned && IsInstalled;

        public bool IsUnpinAllowed => _allowedCommandsService.IsPinCommandAllowed;

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

        public string LowerCaseId
        {
            get { return Id.ToLowerInvariant(); }
        }

        public bool IsAbsoluteLatestVersion
        {
            get { return _isAbsoluteLatestVersion; }
            set { SetPropertyValue(ref _isAbsoluteLatestVersion, value); }
        }

        public bool IsInstalled
        {
            get
            {
                return _isInstalled;
            }

            set
            {
                if (SetPropertyValue(ref _isInstalled, value))
                {
                    NotifyPropertyChanged(nameof(CanUpdate));
                }
            }
        }

        public bool IsPinned
        {
            get
            {
                return _isPinned;
            }

            set
            {
                if (SetPropertyValue(ref _isPinned, value))
                {
                    NotifyPropertyChanged(nameof(CanUpdate));
                }
            }
        }

        public bool IsSideBySide
        {
            get
            {
                return _isSideBySide;
            }

            set
            {
                if (SetPropertyValue(ref _isSideBySide, value))
                {
                    NotifyPropertyChanged(nameof(CanUpdate));
                }
            }
        }

        public bool IsLatestVersion
        {
            get
            {
                return _isLatestVersion;
            }

            set
            {
                if (SetPropertyValue(ref _isLatestVersion, value))
                {
                    NotifyPropertyChanged(nameof(CanUpdate));
                }
            }
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

        public NuGetVersion LatestVersion
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

        public NuGetVersion Version
        {
            get { return _version; }
            set { SetPropertyValue(ref _version, value); }
        }

        public int VersionDownloadCount
        {
            get { return _versionDownloadCount; }
            set { SetPropertyValue(ref _versionDownloadCount, value); }
        }

        public bool IsDownloadCountAvailable
        {
            get
            {
                return DownloadCount != -1 && !(_configService.GetEffectiveConfiguration().HidePackageDownloadCount ?? false);
            }
        }

        public bool IsPackageSizeAvailable
        {
            get { return PackageSize != -1; }
        }

        public async Task ShowArguments()
        {
            var decryptedArguments = _packageArgumentsService.DecryptPackageArgumentsFile(Id, Version.ToString()).ToList();

            if (decryptedArguments.Count == 0)
            {
                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_ArgumentsForPackageFormat), Title),
                    L(nameof(Resources.PackageViewModel_NoArgumentsAvailableForPackage)));
            }
            else
            {
                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_ArgumentsForPackageFormat), Title),
                    string.Join(Environment.NewLine, decryptedArguments));
            }
        }

        public async Task Install()
        {
            await InstallPackage(Version.ToString());
        }

        public async Task InstallAdvanced()
        {
            var dataContext = new AdvancedInstallViewModel(_chocolateyService, _persistenceService, Version);

            var result = await _dialogService.ShowChildWindowAsync<AdvancedInstallViewModel, AdvancedInstallViewModel>(
                L(nameof(Resources.AdvancedChocolateyDialog_Title_Install)),
                new AdvancedInstallView { DataContext = dataContext },
                dataContext);

            // null means that the Cancel button was clicked
            if (result != null)
            {
                if (string.Equals(result.SelectedVersion, Resources.AdvancedChocolateyDialog_LatestVersion, StringComparison.OrdinalIgnoreCase))
                {
                    result.SelectedVersion = null;
                }

                var advancedOptions = _mapper.Map<AdvancedInstall>(result);

                await InstallPackage(result.SelectedVersion, advancedOptions);
            }
        }

        public async Task Reinstall()
        {
            try
            {
                var confirmationResult = MessageDialogResult.Affirmative;
                if (!_configService.GetEffectiveConfiguration().SkipModalDialogConfirmation.GetValueOrDefault(false))
                {
                    confirmationResult = await _dialogService.ShowConfirmationMessageAsync(
                        L(nameof(Resources.Dialog_AreYouSureTitle)),
                        L(nameof(Resources.Dialog_AreYouSureReinstallMessage), Id));
                }

                if (confirmationResult == MessageDialogResult.Affirmative)
                {
                    using (await StartProgressDialog(L(nameof(Resources.PackageViewModel_ReinstallingPackage)), L(nameof(Resources.PackageViewModel_ReinstallingPackage)), Id))
                    {
                        await _chocolateyService.InstallPackage(Id, Version.ToString(), Source, true);
                        _chocolateyGuiCacheService.PurgeOutdatedPackages();
                        await _eventAggregator.PublishOnUIThreadAsync(new PackageChangedMessage(Id, PackageChangeType.Installed, Version));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while reinstalling {Id}, version {Version}.", Id, Version);
                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_FailedToReinstall)),
                    L(nameof(Resources.PackageViewModel_RanIntoInstallError), Id, ex.Message));
            }
        }

        public async Task Uninstall()
        {
            try
            {
                var confirmationResult = MessageDialogResult.Affirmative;
                if (!_configService.GetEffectiveConfiguration().SkipModalDialogConfirmation.GetValueOrDefault(false))
                {
                    confirmationResult = await _dialogService.ShowConfirmationMessageAsync(
                    L(nameof(Resources.Dialog_AreYouSureTitle)),
                    L(nameof(Resources.Dialog_AreYouSureUninstallMessage), Id));
                }

                if (confirmationResult == MessageDialogResult.Affirmative)
                {
                    using (await StartProgressDialog(L(nameof(Resources.PackageViewModel_UninstallingPackage)), L(nameof(Resources.PackageViewModel_UninstallingPackage)), Id))
                    {
                        var result = await _chocolateyService.UninstallPackage(Id, Version.ToString(), true);

                        if (!result.Successful)
                        {
                            var exceptionMessage = result.Exception == null
                                ? string.Empty
                                : L(nameof(Resources.ChocolateyRemotePackageService_ExceptionFormat), result.Exception);

                            var message = L(
                                nameof(Resources.ChocolateyRemotePackageService_UninstallFailedMessage),
                                Id,
                                Version,
                                string.Join("\n", result.Messages),
                                exceptionMessage);

                            await _dialogService.ShowMessageAsync(
                                L(nameof(Resources.ChocolateyRemotePackageService_UninstallFailedTitle)),
                                message);

                            Logger.Warning(result.Exception, "Failed to uninstall {Package}, version {Version}. Errors: {Errors}", Id, Version, result.Messages);

                            return;
                        }

                        IsInstalled = false;
                        _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(Id, PackageChangeType.Uninstalled, Version));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while uninstalling {Id}, version {Version}.", Id, Version);

                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_FailedToUninstall)),
                    L(nameof(Resources.PackageViewModel_RanIntoUninstallError), Id, ex.Message));
            }
        }

        public async Task Update()
        {
            try
            {
                using (await StartProgressDialog(L(nameof(Resources.PackageViewModel_UpdatingPackage)), L(nameof(Resources.PackageViewModel_UpdatingPackage)), Id))
                {
                    var result = await _chocolateyService.UpdatePackage(Id, Source);

                    if (!result.Successful)
                    {
                        var exceptionMessage = result.Exception == null
                            ? string.Empty
                            : L(nameof(Resources.ChocolateyRemotePackageService_ExceptionFormat), result.Exception);

                        var message = L(
                            nameof(Resources.ChocolateyRemotePackageService_UpdateFailedMessage),
                            Id,
                            string.Join("\n", result.Messages),
                            exceptionMessage);

                        await _dialogService.ShowMessageAsync(
                            L(nameof(Resources.ChocolateyRemotePackageService_UpdateFailedTitle)),
                            message);

                        Logger.Warning(result.Exception, "Failed to update {Package}. Errors: {Errors}", Id, result.Messages);

                        return;
                    }

                    _chocolateyGuiCacheService.PurgeOutdatedPackages();
                    _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(Id, PackageChangeType.Updated));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while updating {Id}, version {Version}.", Id, Version);

                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_FailedToUpdate)),
                    L(nameof(Resources.PackageViewModel_RanIntoUpdateError), Id, ex.Message));
            }
        }

        public async Task Pin()
        {
            try
            {
                using (await StartProgressDialog(L(nameof(Resources.PackageViewModel_PinningPackage)), L(nameof(Resources.PackageViewModel_PinningPackage)), Id))
                {
                    var result = await _chocolateyService.PinPackage(Id, Version.ToString());

                    if (!result.Successful)
                    {
                        var exceptionMessage = result.Exception == null
                            ? string.Empty
                            : L(nameof(Resources.ChocolateyRemotePackageService_ExceptionFormat), result.Exception);

                        var message = L(
                            nameof(Resources.ChocolateyRemotePackageService_PinFailedMessage),
                            Id,
                            Version,
                            string.Join("\n", result.Messages),
                            exceptionMessage);

                        await _dialogService.ShowMessageAsync(
                            L(nameof(Resources.ChocolateyRemotePackageService_PinFailedTitle)),
                            message);

                        Logger.Warning(result.Exception, "Failed to pin {Package}, version {Version}. Errors: {Errors}", Id, Version, result.Messages);

                        return;
                    }

                    IsPinned = true;
                    _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(Id, PackageChangeType.Pinned, Version));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while pinning {Id}, version {Version}.", Id, Version);

                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_FailedToPin)),
                    L(nameof(Resources.PackageViewModel_RanIntoPinningError), Id, ex.Message));
            }
        }

        public async Task Unpin()
        {
            try
            {
                using (await StartProgressDialog(L(nameof(Resources.PackageViewModel_UnpinningPackage)), L(nameof(Resources.PackageViewModel_UnpinningPackage)), Id))
                {
                    var result = await _chocolateyService.UnpinPackage(Id, Version.ToString());

                    if (!result.Successful)
                    {
                        var exceptionMessage = result.Exception == null
                            ? string.Empty
                            : L(nameof(Resources.ChocolateyRemotePackageService_ExceptionFormat), result.Exception);

                        var message = L(
                            nameof(Resources.ChocolateyRemotePackageService_UnpinFailedMessage),
                            Id,
                            Version,
                            string.Join("\n", result.Messages),
                            exceptionMessage);

                        await _dialogService.ShowMessageAsync(
                            L(nameof(Resources.ChocolateyRemotePackageService_UninstallFailedTitle)),
                            message);

                        Logger.Warning(result.Exception, "Failed to unpin {Package}, version {Version}. Errors: {Errors}", Id, Version, result.Messages);

                        return;
                    }

                    IsPinned = false;
                    _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(Id, PackageChangeType.Unpinned, Version));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while unpinning {Id}, version {Version}.", Id, Version);

                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_FailedToUnpin)),
                    L(nameof(Resources.PackageViewModel_RanIntoUnpinError), Id, ex.Message));
            }
        }

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void

        public async void ViewDetails()
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            await _eventAggregator.PublishOnUIThreadAsync(new ShowPackageDetailsMessage(this)).ConfigureAwait(false);
        }

        public void Handle(FeatureModifiedMessage message)
        {
            NotifyPropertyChanged(nameof(IsDownloadCountAvailable));
        }

        public void Handle(PackageHasUpdateMessage message)
        {
            if (!string.Equals(message.Id, Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            LatestVersion = message.Version;
            IsLatestVersion = false;
        }

        private async Task InstallPackage(string version, AdvancedInstall advancedOptions = null)
        {
            try
            {
                using (await StartProgressDialog(L(nameof(Resources.PackageViewModel_InstallingPackage)), L(nameof(Resources.PackageViewModel_InstallingPackage)), Id))
                {
                    var packageInstallResult = await _chocolateyService.InstallPackage(
                        Id,
                        version,
                        Source,
                        false,
                        advancedOptions);

                    if (!packageInstallResult.Successful)
                    {
                        var exceptionMessage = packageInstallResult.Exception == null
                            ? string.Empty
                            : L(nameof(Resources.ChocolateyRemotePackageService_ExceptionFormat), packageInstallResult.Exception);

                        var message = L(
                            nameof(Resources.ChocolateyRemotePackageService_InstallFailedMessage),
                            Id,
                            Version,
                            string.Join("\n", packageInstallResult.Messages),
                            exceptionMessage);

                        await _dialogService.ShowMessageAsync(
                            L(nameof(Resources.ChocolateyRemotePackageService_InstallFailedTitle)),
                            message);

                        Logger.Warning(packageInstallResult.Exception, "Failed to install {Package}, version {Version}. Errors: {Errors}", Id, Version, packageInstallResult.Messages);

                        return;
                    }

                    IsInstalled = true;

                    _chocolateyGuiCacheService.PurgeOutdatedPackages();
                    _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(Id, PackageChangeType.Installed, Version));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ran into an error while installing {Id}, version {Version}.", Id, Version);

                await _dialogService.ShowMessageAsync(
                    L(nameof(Resources.PackageViewModel_FailedToInstall)),
                    L(nameof(Resources.PackageViewModel_RanIntoInstallError), Id, ex.Message));
            }
        }

        private async Task<IDisposable> StartProgressDialog(string commandString, string initialProgressText, string id = "")
        {
            await _progressService.StartLoading(L(nameof(Resources.PackageViewModel_StartLoadingFormat), commandString, id));
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
                _disposeAction?.Invoke();
            }
        }
    }
}