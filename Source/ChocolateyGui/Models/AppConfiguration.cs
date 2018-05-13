// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AppConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    public class AppConfiguration
    {
        public string Id { get; set; }

        public bool ShowConsoleOutput { get; set; }

        public bool DefaultToTileViewForLocalSource { get; set; }

        public bool DefaultToTileViewForRemoteSource { get; set; }

        public bool UseDelayedSearch { get; set; }

        public bool SearchInAllRepositories { get; set; }
    }
}