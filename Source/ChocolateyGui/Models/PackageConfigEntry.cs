// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageConfigEntry.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NuGet;

namespace ChocolateyGui.Models
{
    public class PackageConfigEntry
    {
        public PackageConfigEntry(string id, SemanticVersion version, Uri source)
        {
            Id = id;
            Version = version;
            Source = source;
        }

        public string Id { get; private set; }

        public Uri Source { get; private set; }

        public SemanticVersion Version { get; private set; }
    }
}