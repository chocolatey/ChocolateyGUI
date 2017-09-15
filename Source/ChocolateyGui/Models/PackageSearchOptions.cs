// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageSearchOptions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace ChocolateyGui.Models
{
    [DataContract]
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

        [DataMember]
        public int CurrentPage { get; set;  }

        [DataMember]
        public bool IncludeAllVersions { get; set; }

        [DataMember]
        public bool IncludePrerelease { get; set; }

        [DataMember]
        public bool MatchQuery { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public string SortColumn { get; set; }

        [DataMember]
        public string[] TagsQuery { get; set; }
    }
}