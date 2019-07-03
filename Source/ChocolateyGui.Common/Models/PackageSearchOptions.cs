// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchOptions.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models
{
    public struct PackageSearchOptions
    {
        public PackageSearchOptions(
            int pageSize,
            int currentPage,
            string sortColumn,
            bool includePrerelease,
            bool includeAllVersions,
            bool matchWord,
            string source)
            : this()
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            SortColumn = sortColumn;
            IncludeAllVersions = includeAllVersions;
            IncludePrerelease = includePrerelease;
            MatchQuery = matchWord;
            Source = source;
        }

        public int CurrentPage { get; set;  }

        public bool IncludeAllVersions { get; set; }

        public bool IncludePrerelease { get; set; }

        public bool MatchQuery { get; set; }

        public string Source { get; set; }

        public int PageSize { get; set; }

        public string SortColumn { get; set; }

        public string[] TagsQuery { get; set; }
    }
}