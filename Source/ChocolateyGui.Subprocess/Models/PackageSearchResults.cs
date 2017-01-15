// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchResults.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace ChocolateyGui.Subprocess.Models
{
    public class PackageSearchResults
    {
        public IEnumerable<Package> Packages { get; set; }

        public int TotalCount { get; set; }
    }
}