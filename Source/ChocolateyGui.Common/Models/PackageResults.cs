// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageResults.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models
{
    public class PackageResults
    {
        public Package[] Packages { get; set; }

        public int TotalCount { get; set; }
    }
}