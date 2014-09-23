// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageConfigEntry.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    using System;

    public class PackageConfigEntry
    {
        public PackageConfigEntry(string id, SemanticVersion version, Uri source)
        {
            this.Id = id;
            this.Version = version;
            this.Source = source;
        }

        public string Id { get; private set; }

        public Uri Source { get; private set; }

        public SemanticVersion Version { get; private set; }
    }
}