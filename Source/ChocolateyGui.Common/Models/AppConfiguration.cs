// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AppConfiguration.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using chocolatey;
using ChocolateyGui.Common.Attributes;

namespace ChocolateyGui.Common.Models
{
    public class AppConfiguration
    {
        public string Id { get; set; }

        [LocalizedDescription("SettingsView_ConfigOutdatedPackagesCacheDurationInMinutesDescription")]
        [Config]
        public string OutdatedPackagesCacheDurationInMinutes { get; set; }

        [LocalizedDescription("SettingsView_ConfigDefaultSourceName")]
        [Config]
        public string DefaultSourceName { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowConsoleOutputDescription")]
        [Feature]
        public bool? ShowConsoleOutput { get; set; }

        [LocalizedDescription("SettingsView_ToggleDefaultTileViewLocalDescription")]
        [Feature]
        public bool? DefaultToTileViewForLocalSource { get; set; }

        [LocalizedDescription("SettingsView_ToggleDefaultTileViewRemoteDescription")]
        [Feature]
        public bool? DefaultToTileViewForRemoteSource { get; set; }

        [LocalizedDescription("SettingsView_ToggleUseDelayedSearchDescription")]
        [Feature]
        public bool? UseDelayedSearch { get; set; }

        [LocalizedDescription("SettingsView_TogglePreventPreloadDescription")]
        [Feature]
        public bool? PreventPreload { get; set; }

        [LocalizedDescription("SettingsView_TogglePreventAutomatedOutdatedPackagesCheckDescription")]
        [Feature]
        public bool? PreventAutomatedOutdatedPackagesCheck { get; set; }

        [LocalizedDescription("SettingsView_ToggleExcludeInstalledPackagesDescription")]
        [Feature]
        public bool? ExcludeInstalledPackages { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowAggregatedSourceViewDescription")]
        [Feature]
        public bool? ShowAggregatedSourceView { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowAdditionalPackageInformationDescription")]
        [Feature]
        public bool? ShowAdditionalPackageInformation { get; set; }

        [LocalizedDescription("SettingsView_ToggleAllowNonAdminAccessToSettingsDescription")]
        [Feature]
        public bool? AllowNonAdminAccessToSettings { get; set; }

        [LocalizedDescription("SettingsView_ToggleUseKeyboardBindings")]
        [Feature]
        public bool? UseKeyboardBindings { get; set; }

        [LocalizedDescription("SettingsView_TogglePackageDownloadCountDescription")]
        [Feature]
        public bool? HidePackageDownloadCount { get; set; }

        [LocalizedDescription("SettingsView_TogglePreventAllPackageIconDownloads")]
        [Feature]
        public bool? PreventAllPackageIconDownloads { get; set; }

        [LocalizedDescription("SettingsView_ToggleHideAllRemoteChocolateySources")]
        [Feature]
        public bool? HideAllRemoteChocolateySources { get; set; }

        [LocalizedDescription("SettingsView_ToggleDefaultToDarkMode")]
        [Feature]
        public bool? DefaultToDarkMode { get; set; }

        public override string ToString()
        {
            return @"
OutdatedPackagesCacheDurationInMinutes: {0}
DefaultSourceName: {1}
ShowConsoleOutput: {2}
DefaultToTileViewForLocalSource: {3}
DefaultToTileViewForRemoteSource: {4}
UseDelayedSearch: {5}
PreventPreload: {6}
PreventAutomatedOutdatedPackagesCheck: {7}
ExcludeInstalledPackages: {8}
ShowAggregatedSourceView: {9}
ShowAdditionalPackageInformation: {10}
AllowNonAdminAccessToSettings: {11}
UseKeyboardBindings: {12}
HidePackageDownloadCount: {13}
PreventAllPackageIconDownloads: {14}
HideAllRemoteChocolateySources: {15}
DefaultToDarkMode: {16}
".format_with(
                OutdatedPackagesCacheDurationInMinutes,
                DefaultSourceName,
                ShowConsoleOutput,
                DefaultToTileViewForLocalSource,
                DefaultToTileViewForRemoteSource,
                UseDelayedSearch,
                PreventPreload,
                PreventAutomatedOutdatedPackagesCheck,
                ExcludeInstalledPackages,
                ShowAggregatedSourceView,
                ShowAdditionalPackageInformation,
                AllowNonAdminAccessToSettings,
                UseKeyboardBindings,
                HidePackageDownloadCount,
                PreventAllPackageIconDownloads,
                HideAllRemoteChocolateySources,
                DefaultToDarkMode);
        }
    }
}