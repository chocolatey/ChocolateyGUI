namespace ChocolateyGui.Models
{
    public struct PackageSearchOptions
    {
        private readonly int _currentPage;

        private readonly bool _includeAllVersions;

        private readonly bool _includedPrerelease;

        private readonly bool _matchQuery;

        private readonly int _pageSize;

        private readonly string _sortColumn;

        private readonly bool _sortDescending;

        private readonly string[] _tagsQuery;

        public PackageSearchOptions(int pageSize, int currentPage)
            : this()
        {
            this._pageSize = pageSize;
            this._currentPage = currentPage;
        }

        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn, bool sortDescending)
            : this()
        {
            this._pageSize = pageSize;
            this._currentPage = currentPage;
            this._sortColumn = sortColumn;
            this._sortDescending = sortDescending;
        }

        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn, bool sortDescending, bool includePrerelease, bool includeAllVersions, bool matchWord)
            : this()
        {
            this._pageSize = pageSize;
            this._currentPage = currentPage;
            this._sortColumn = sortColumn;
            this._sortDescending = sortDescending;
            this._includeAllVersions = includeAllVersions;
            this._includedPrerelease = includePrerelease;
            this._matchQuery = matchWord;
        }

        public int CurrentPage
        {
            get
            {
                return this._currentPage;
            }
        }

        public bool IncludeAllVersions
        {
            get
            {
                return this._includeAllVersions;
            }
        }

        public bool IncludePrerelease
        {
            get
            {
                return this._includedPrerelease;
            }
        }

        public bool MatchQuery
        {
            get
            {
                return this._matchQuery;
            }
        }

        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
        }

        public string SortColumn
        {
            get
            {
                return this._sortColumn;
            }
        }

        public bool SortDescending
        {
            get
            {
                return this._sortDescending;
            }
        }

        public string[] TagsQuery
        {
            get
            {
                return this._tagsQuery;
            }
        }
    }
}