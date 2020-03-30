// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AppConfiguration.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Attributes;

namespace ChocolateyGui.Common.Models
{
    public class AppConfiguration
    {
        public string Id { get; set; }

        [LocalizedDescription("SettingsView_ConfigOutdatedPackagesCacheDurationInMinutesDescription")]
        [Config]
        public string OutdatedPackagesCacheDurationInMinutes { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowConsoleOutputDescription")]
        [Feature]
        public bool ShowConsoleOutput { get; set; }

        [LocalizedDescription("SettingsView_ToggleDefaultTileViewLocalDescription")]
        [Feature]
        public bool DefaultToTileViewForLocalSource { get; set; }

        [LocalizedDescription("SettingsView_ToggleDefaultTileViewRemoteDescription")]
        [Feature]
        public bool DefaultToTileViewForRemoteSource { get; set; }

        [LocalizedDescription("SettingsView_ToggleUseDelayedSearchDescription")]
        [Feature]
        public bool UseDelayedSearch { get; set; }

        [LocalizedDescription("SettingsView_TogglePreventPreloadDescription")]
        [Feature]
        public bool PreventPreload { get; set; }

        [LocalizedDescription("SettingsView_ToggleExcludeInstalledPackagesDescription")]
        [Feature]
        public bool ExcludeInstalledPackages { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowAggregatedSourceViewDescription")]
        [Feature]
        public bool ShowAggregatedSourceView { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowAdditionalPackageInformationDescription")]
        [Feature]
        public bool ShowAdditionalPackageInformation { get; set; }

        [LocalizedDescription("SettingsView_ToggleAllowNonAdminAccessToSettingsDescription")]
        [Feature]
        public bool AllowNonAdminAccessToSettings { get; set; }

        [LocalizedDescription("SettingsView_ToggleUseKeyboardBindings")]
        [Feature]
        public bool UseKeyboardBindings { get; set; }

        [LocalizedDescription("SettingsView_TogglePackageDownloadCountDescription")]
        [Feature]
        public bool HidePackageDownloadCount { get; set; }
    }
}