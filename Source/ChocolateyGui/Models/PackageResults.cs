// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageResults.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    public class PackageResults
    {
        public Package[] Packages { get; set; }

        public int TotalCount { get; set; }
    }
}