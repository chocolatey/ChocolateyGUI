// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AdvancedInstallViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using chocolatey;
using ChocolateyGui.Common.Base;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Windows.Commands;
using ChocolateyGui.Common.Windows.Controls.Dialogs;
using ChocolateyGui.Common.Windows.Utilities;
using NuGet.Versioning;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public class AdvancedInstallViewModel : ObservableBase, IClosableChildWindow<AdvancedInstallViewModel>
    {
        private readonly IChocolateyService _chocolateyService;
        private readonly IPersistenceService _persistenceService;
        private CancellationTokenSource _cts;
        private string _selectedVersion;
        private bool _includePreRelease;
        private Utilities.NotifyTaskCompletion<ObservableCollection<string>> _availableVersions;
        private string _packageParamaters;
        private string _installArguments;
        private int _executionTimeoutInSeconds;
        private string _logFile;
        private string _cacheLocation;
        private bool _preRelease;
        private bool _forcex86;
        private bool _overrideArguments;
        private bool _notSilent;
        private bool _applyInstallArgumentsToDependencies;
        private bool _applyPackageParametersToDependencies;
        private bool _allowDowngrade;
        private bool _allowMultipleVersions;
        private bool _ignoreDependencies;
        private bool _forceDependencies;
        private bool _skipPowerShell;
        private bool _ignoreChecksums;
        private bool _allowEmptyChecksums;
        private bool _allowEmptyChecksumsSecure;
        private bool _requireChecksums;
        private string _downloadChecksum;
        private string _downloadChecksum64bit;
        private string _downloadChecksumType;
        private string _downloadChecksumType64bit;
        private List<string> _availableChecksumTypes;
        private string _packageVersion;

        public AdvancedInstallViewModel(
            IChocolateyService chocolateyService,
            IPersistenceService persistenceService,
            NuGetVersion packageVersion)
        {
            _chocolateyService = chocolateyService;
            _persistenceService = persistenceService;

            _cts = new CancellationTokenSource();

            _packageVersion = packageVersion.ToString();
            SelectedVersion = _packageVersion;

            FetchAvailableVersions();

            AvailableChecksumTypes = new List<string> { "md5", "sha1", "sha256", "sha512" };
            InstallCommand = new RelayCommand(
                o => { Close?.Invoke(this); },
                o => string.IsNullOrEmpty(SelectedVersion) || SelectedVersion == Resources.AdvancedChocolateyDialog_LatestVersion || NuGetVersion.TryParse(SelectedVersion, out _));
            CancelCommand = new RelayCommand(
                o =>
                {
                    _cts.Cancel();
                    Close?.Invoke(null);
                },
                o => true);

            BrowseLogFileCommand = new RelayCommand(BrowseLogFile);
            BrowseCacheLocationCommand = new RelayCommand(BrowseCacheLocation);

            SetDefaults();
        }

        public string SelectedVersion
        {
            get
            {
                return _selectedVersion;
            }

            set
            {
                SetPropertyValue(ref _selectedVersion, value);
                OnSelectedVersionChanged(value);
            }
        }

        public bool IncludePreRelease
        {
            get
            {
                return _includePreRelease;
            }

            set
            {
                if (SetPropertyValue(ref _includePreRelease, value))
                {
                    _cts.Cancel();
                    _cts.Dispose();
                    _cts = new CancellationTokenSource();

                    FetchAvailableVersions();
                }
            }
        }

        public Utilities.NotifyTaskCompletion<ObservableCollection<string>> AvailableVersions
        {
            get { return _availableVersions; }
            set { SetPropertyValue(ref _availableVersions, value); }
        }

        public string PackageParameters
        {
            get { return _packageParamaters; }
            set { SetPropertyValue(ref _packageParamaters, value); }
        }

        public string InstallArguments
        {
            get { return _installArguments; }
            set { SetPropertyValue(ref _installArguments, value); }
        }

        public int ExecutionTimeoutInSeconds
        {
            get { return _executionTimeoutInSeconds; }
            set { SetPropertyValue(ref _executionTimeoutInSeconds, value); }
        }

        public string LogFile
        {
            get { return _logFile; }
            set { SetPropertyValue(ref _logFile, value); }
        }

        public string CacheLocation
        {
            get { return _cacheLocation; }
            set { SetPropertyValue(ref _cacheLocation, value); }
        }

        public bool PreRelease
        {
            get { return _preRelease; }
            set { SetPropertyValue(ref _preRelease, value); }
        }

        public bool Forcex86
        {
            get { return _forcex86; }
            set { SetPropertyValue(ref _forcex86, value); }
        }

        public bool OverrideArguments
        {
            get
            {
                return _overrideArguments;
            }

            set
            {
                SetPropertyValue(ref _overrideArguments, value);

                if (value)
                {
                    NotSilent = false;
                }
            }
        }

        public bool NotSilent
        {
            get
            {
                return _notSilent;
            }

            set
            {
                SetPropertyValue(ref _notSilent, value);

                if (value)
                {
                    OverrideArguments = false;
                }
            }
        }

        public bool ApplyInstallArgumentsToDependencies
        {
            get { return _applyInstallArgumentsToDependencies; }
            set { SetPropertyValue(ref _applyInstallArgumentsToDependencies, value); }
        }

        public bool ApplyPackageParametersToDependencies
        {
            get { return _applyPackageParametersToDependencies; }
            set { SetPropertyValue(ref _applyPackageParametersToDependencies, value); }
        }

        public bool AllowDowngrade
        {
            get { return _allowDowngrade; }
            set { SetPropertyValue(ref _allowDowngrade, value); }
        }

        public bool AllowMultipleVersions
        {
            get { return _allowMultipleVersions; }
            set { SetPropertyValue(ref _allowMultipleVersions, value); }
        }

        public bool IgnoreDependencies
        {
            get
            {
                return _ignoreDependencies;
            }

            set
            {
                SetPropertyValue(ref _ignoreDependencies, value);

                if (value)
                {
                    ForceDependencies = false;
                }
            }
        }

        public bool ForceDependencies
        {
            get
            {
                return _forceDependencies;
            }

            set
            {
                SetPropertyValue(ref _forceDependencies, value);

                if (value)
                {
                    IgnoreDependencies = false;
                }
            }
        }

        public bool SkipPowerShell
        {
            get
            {
                return _skipPowerShell;
            }

            set
            {
                SetPropertyValue(ref _skipPowerShell, value);

                if (value)
                {
                    OverrideArguments = false;
                    NotSilent = false;
                }
            }
        }

        public bool IgnoreChecksums
        {
            get
            {
                return _ignoreChecksums;
            }

            set
            {
                SetPropertyValue(ref _ignoreChecksums, value);

                if (value)
                {
                    RequireChecksums = false;
                }
            }
        }

        public bool AllowEmptyChecksums
        {
            get
            {
                return _allowEmptyChecksums;
            }

            set
            {
                SetPropertyValue(ref _allowEmptyChecksums, value);

                if (value)
                {
                    RequireChecksums = false;
                }
            }
        }

        public bool AllowEmptyChecksumsSecure
        {
            get
            {
                return _allowEmptyChecksumsSecure;
            }

            set
            {
                SetPropertyValue(ref _allowEmptyChecksumsSecure, value);

                if (value)
                {
                    RequireChecksums = false;
                }
            }
        }

        public bool RequireChecksums
        {
            get
            {
                return _requireChecksums;
            }

            set
            {
                SetPropertyValue(ref _requireChecksums, value);

                if (value)
                {
                    IgnoreChecksums = false;
                    AllowEmptyChecksums = false;
                    AllowEmptyChecksumsSecure = false;
                }
            }
        }

        public string DownloadChecksum
        {
            get { return _downloadChecksum; }
            set { SetPropertyValue(ref _downloadChecksum, value); }
        }

        public string DownloadChecksum64bit
        {
            get { return _downloadChecksum64bit; }
            set { SetPropertyValue(ref _downloadChecksum64bit, value); }
        }

        public string DownloadChecksumType
        {
            get
            {
                return _downloadChecksumType;
            }

            set
            {
                SetPropertyValue(ref _downloadChecksumType, value);
                DownloadChecksumType64bit = value;
            }
        }

        public string DownloadChecksumType64bit
        {
            get { return _downloadChecksumType64bit; }
            set { SetPropertyValue(ref _downloadChecksumType64bit, value); }
        }

        public List<string> AvailableChecksumTypes
        {
            get { return _availableChecksumTypes; }
            set { SetPropertyValue(ref _availableChecksumTypes, value); }
        }

        public ICommand InstallCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand BrowseLogFileCommand { get; }

        public ICommand BrowseCacheLocationCommand { get; }

        /// <inheritdoc />
        public Action<AdvancedInstallViewModel> Close { get; set; }

        private void FetchAvailableVersions()
        {
            var availableVersions = new ObservableCollection<string>();
            availableVersions.Add(Resources.AdvancedChocolateyDialog_LatestVersion);

            if (!string.IsNullOrEmpty(_packageVersion))
            {
                availableVersions.Add(_packageVersion);
            }

            AvailableVersions =
                new NotifyTaskCompletion<ObservableCollection<string>>(Task.FromResult(availableVersions));
        }

        private void SetDefaults()
        {
            var choco = Lets.GetChocolatey();
            var config = choco.GetConfiguration();
            DownloadChecksumType = "md5";
            DownloadChecksumType64bit = "md5";
            ExecutionTimeoutInSeconds = config.CommandExecutionTimeoutSeconds;
            CacheLocation = config.CacheLocation;
            LogFile = config.AdditionalLogFileLocation;
        }

        private void OnSelectedVersionChanged(string stringVersion)
        {
            NuGetVersion version;

            if (NuGetVersion.TryParse(stringVersion, out version))
            {
                PreRelease = version.IsPrerelease;
            }
        }

        private void BrowseLogFile(object value)
        {
            var filter = "{0}|{1}|{2}".format_with(
                L(nameof(Resources.FilePicker_LogFiles)) + "|*.log;*.klg",
                L(nameof(Resources.FilePicker_TextFiles)) + "|*.txt;*.text;*.plain",
                L(nameof(Resources.FilePicker_AllFiles)) + "|*.*");

            var logFile = _persistenceService.GetFilePath("log", filter);

            if (!string.IsNullOrEmpty(logFile))
            {
                LogFile = logFile;
            }
        }

        private void BrowseCacheLocation(object value)
        {
            var description = L(nameof(Resources.AdvancedChocolateyDialog_CacheLocation_BrowseDescription));
            var cacheDirectory = _persistenceService.GetFolderPath(CacheLocation, description);

            if (!string.IsNullOrEmpty(cacheDirectory))
            {
                CacheLocation = cacheDirectory;
            }
        }
    }
}