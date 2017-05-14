// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchOptions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace ChocolateyGui.Models
{
    public struct PackageSearchOptions
    {
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

        [JsonConstructor]
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

        public int CurrentPage { get; }

        public bool IncludeAllVersions { get; }

        public bool IncludePrerelease { get; }

        public bool MatchQuery { get; }

        public string Source { get; }

        public int PageSize { get; }

        public string SortColumn { get; }

        public string[] TagsQuery { get; }
    }
}