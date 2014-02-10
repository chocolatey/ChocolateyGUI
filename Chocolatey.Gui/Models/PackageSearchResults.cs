using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Models
{
    public class PackageSearchResults
    {
        public IEnumerable<IPackageViewModel> Packages { get; set; }
        public int TotalCount { get; set; }
    }
}
