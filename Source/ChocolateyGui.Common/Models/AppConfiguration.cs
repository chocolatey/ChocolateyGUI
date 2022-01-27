// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AppConfiguration.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using chocolatey;
using ChocolateyGui.Common.Attributes;
using ChocolateyGui.Common.Properties;

namespace ChocolateyGui.Common.Models
{
    public class AppConfiguration
    {
        public string Id { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ConfigOutdatedPackagesCacheDurationInMinutesDescription))]
        [Config]
        public string OutdatedPackagesCacheDurationInMinutes { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ConfigDefaultSourceName))]
        [Config]
        public string DefaultSourceName { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_NumberOfPackageVersionsForSelectionDescription))]
        [Config]
        public string NumberOfPackageVersionsForSelection { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_LanguageDescription))]
        [Config]
        public string UseLanguage { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleShowConsoleOutputDescription))]
        [Feature]
        public bool? ShowConsoleOutput { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleDefaultTileViewLocalDescription))]
        [Feature]
        public bool? DefaultToTileViewForLocalSource { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleDefaultTileViewRemoteDescription))]
        [Feature]
        public bool? DefaultToTileViewForRemoteSource { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleUseDelayedSearchDescription))]
        [Feature]
        public bool? UseDelayedSearch { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_TogglePreventPreloadDescription))]
        [Feature]
        public bool? PreventPreload { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_TogglePreventAutomatedOutdatedPackagesCheckDescription))]
        [Feature]
        public bool? PreventAutomatedOutdatedPackagesCheck { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleExcludeInstalledPackagesDescription))]
        [Feature]
        public bool? ExcludeInstalledPackages { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleShowAggregatedSourceViewDescription))]
        [Feature]
        public bool? ShowAggregatedSourceView { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleShowAdditionalPackageInformationDescription))]
        [Feature]
        public bool? ShowAdditionalPackageInformation { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleAllowNonAdminAccessToSettingsDescription))]
        [Feature]
        public bool? AllowNonAdminAccessToSettings { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleUseKeyboardBindings))]
        [Feature]
        public bool? UseKeyboardBindings { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_TogglePackageDownloadCountDescription))]
        [Feature]
        public bool? HidePackageDownloadCount { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_TogglePreventAllPackageIconDownloads))]
        [Feature]
        public bool? PreventAllPackageIconDownloads { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleHideAllRemoteChocolateySources))]
        [Feature]
        public bool? HideAllRemoteChocolateySources { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleDefaultToDarkMode))]
        [Feature]

        public bool? DefaultToDarkMode { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_ToggleHideThisPCSourceDescription))]
        [Feature]
        public bool? HideThisPCSource { get; set; }

        [LocalizedDescription(nameof(Resources.SettingsView_TogglePreventUsageOfUpdateAllButtonDescription))]
        [Feature]
        public bool? PreventUsageOfUpdateAllButton { get; set; }

        public override string ToString()
        {
            return @"
OutdatedPackagesCacheDurationInMinutes: {0}
DefaultSourceName: {1}
UseLanguage: {2}
ShowConsoleOutput: {3}
DefaultToTileViewForLocalSource: {4}
DefaultToTileViewForRemoteSource: {5}
UseDelayedSearch: {6}
PreventPreload: {7}
PreventAutomatedOutdatedPackagesCheck: {8}
ExcludeInstalledPackages: {9}
ShowAggregatedSourceView: {10}
ShowAdditionalPackageInformation: {11}
AllowNonAdminAccessToSettings: {12}
UseKeyboardBindings: {13}
HidePackageDownloadCount: {14}
PreventAllPackageIconDownloads: {15}
HideAllRemoteChocolateySources: {16}
DefaultToDarkMode: {17}
HideThisPCSource: {18}
PreventUsageOfUpdateAllButton: {19}
".format_with(
                OutdatedPackagesCacheDurationInMinutes,
                DefaultSourceName,
                UseLanguage,
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
                DefaultToDarkMode,
                HideThisPCSource,
                PreventUsageOfUpdateAllButton);
        }
    }
}