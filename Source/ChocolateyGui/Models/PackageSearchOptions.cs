// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchOptions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    public struct PackageSearchOptions
    {
#pragma warning disable 649
#pragma warning restore 649

        public PackageSearchOptions(int pageSize, int currentPage)
            : this()
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
        }

        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn)
            : this()
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            SortColumn = sortColumn;
        }

        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn, bool includePrerelease,
            bool includeAllVersions, bool matchWord)
            : this()
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            SortColumn = sortColumn;
            IncludeAllVersions = includeAllVersions;
            IncludePrerelease = includePrerelease;
            MatchQuery = matchWord;
        }

        public int CurrentPage { get; }

        public bool IncludeAllVersions { get; }

        public bool IncludePrerelease { get; }

        public bool MatchQuery { get; }

        public int PageSize { get; }

        public string SortColumn { get; }

        public string[] TagsQuery { get; }
    }
}