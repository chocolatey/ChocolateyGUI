// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageConfig.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    using System;
    using System.Collections.Generic;

    public class PackageConfig
    {
        public DateTime LastUpdated { get; set; }

        public IEnumerable<PackageConfigEntry> Packages { get; set; }
    }
}