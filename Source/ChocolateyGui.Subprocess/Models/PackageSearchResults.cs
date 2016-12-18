using System.Collections.Generic;
using ChocolateyGui.Models;

namespace ChocolateyGui.Subprocess.Models
{
    public class PackageSearchResults
    {
        public IEnumerable<Package> Packages { get; set; }

        public int TotalCount { get; set; }
    }
}
