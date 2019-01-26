// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AppConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Attributes;

namespace ChocolateyGui.Models
{
    public class AppConfiguration
    {
        public string Id { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowConsoleOutputDescription")]
        public bool ShowConsoleOutput { get; set; }

        [LocalizedDescription("SettingsView_ToggleDefaultTileViewLocalDescription")]
        public bool DefaultToTileViewForLocalSource { get; set; }

        [LocalizedDescription("SettingsView_ToggleDefaultTileViewRemoteDescription")]
        public bool DefaultToTileViewForRemoteSource { get; set; }

        [LocalizedDescription("SettingsView_ToggleUseDelayedSearchDescription")]
        public bool UseDelayedSearch { get; set; }

        [LocalizedDescription("SettingsView_ToggleExcludeInstalledPackagesDescription")]
        public bool ExcludeInstalledPackages { get; set; }

        [LocalizedDescription("SettingsView_ToggleShowAggregatedSourceViewDescription")]
        public bool ShowAggregatedSourceView { get; set; }
    }
}