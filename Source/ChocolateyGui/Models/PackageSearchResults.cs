// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchResults.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    using System.Collections.Generic;
    using ChocolateyGui.ViewModels.Items;
    
    public class PackageSearchResults
    {
        public IEnumerable<IPackageViewModel> Packages { get; set; }

        public int TotalCount { get; set; }
    }
}