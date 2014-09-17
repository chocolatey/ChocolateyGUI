using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Models
{
    public class PackageSearchResults
    {
        public IEnumerable<IPackageViewModel> Packages { get; set; }
        public int TotalCount { get; set; }
    }
}
