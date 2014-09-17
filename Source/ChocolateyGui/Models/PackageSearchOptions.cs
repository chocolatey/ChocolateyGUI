namespace ChocolateyGui.Models
{
    public struct PackageSearchOptions
    {
        public PackageSearchOptions(int pageSize, int currentPage)
            : this()
        {
            _pageSize = pageSize;
            _currentPage = currentPage;
        }
        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn, bool sortDescending)
            : this()
        {
            _pageSize = pageSize;
            _currentPage = currentPage;
            _sortColumn = sortColumn;
            _sortDescending = sortDescending;
        }
        public PackageSearchOptions(int pageSize, int currentPage, string sortColumn, bool sortDescending, bool includePrerelease, bool includeAllVersions, bool matchWord)
            : this()
        {
            _pageSize = pageSize;
            _currentPage = currentPage;
            _sortColumn = sortColumn;
            _sortDescending = sortDescending;
            _includeAllVersions = includeAllVersions;
            _includedPrerelease = includePrerelease;
            _matchQuery = matchWord;
        }

        private readonly int _pageSize;
        public int PageSize { get { return _pageSize; } }

        private readonly int _currentPage;
        public int CurrentPage { get { return _currentPage; } }

        private readonly string _sortColumn;
        public string SortColumn { get { return _sortColumn; } }

        private readonly bool _sortDescending;
        public bool SortDescending { get { return _sortDescending; } }

        private readonly bool _includedPrerelease;
        public bool IncludePrerelease { get { return _includedPrerelease; } }

        private readonly bool _includeAllVersions;
        public bool IncludeAllVersions { get { return _includeAllVersions; } }

        private readonly bool _matchQuery;
        public bool MatchQuery { get { return _matchQuery; } }

        private readonly string[] _tagsQuery;

        public string[] TagsQuery { get { return _tagsQuery; } }
    }
}
