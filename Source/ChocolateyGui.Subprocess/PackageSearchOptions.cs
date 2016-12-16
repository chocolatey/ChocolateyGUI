// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchOptions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace ChocolateyGui.Subprocess
{
    public struct PackageSearchOptions
    {
#pragma warning disable 649
#pragma warning restore 649

        public PackageSearchOptions(int pageSize, int currentPage)
            : this()
        {
            this.PageSize = pageSize;
            this.CurrentPage = currentPage;
        }

        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn)
            : this()
        {
            this.PageSize = pageSize;
            this.CurrentPage = currentPage;
            this.SortColumn = sortColumn;
        }

        [JsonConstructor]
        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn, bool includePrerelease,
            bool includeAllVersions, bool matchWord)
            : this()
        {
            this.PageSize = pageSize;
            this.CurrentPage = currentPage;
            this.SortColumn = sortColumn;
            this.IncludeAllVersions = includeAllVersions;
            this.IncludePrerelease = includePrerelease;
            this.MatchQuery = matchWord;
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