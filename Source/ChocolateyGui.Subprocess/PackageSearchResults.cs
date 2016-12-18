using System.Collections.Generic;

namespace ChocolateyGui.Subprocess
{
    using ChocolateyGui.Models;

    public class PackageSearchResults
    {
        public IEnumerable<Package> Packages { get; set; }

        public int TotalCount { get; set; }
    }
}
