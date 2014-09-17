using System.Collections.ObjectModel;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels.Controls
{
    public interface IRemoteSourceControlViewModel
    {
        ObservableCollection<IPackageViewModel> Packages { get; set; }

        string SearchQuery { get; set; }
        bool IncludePrerelease { get; set; }
        bool IncludeAllVersions { get; set; }
        bool MatchWord { get; set; }

        string SortColumn { get; set; }
        bool SortDescending { get; set; }

        int PageCount { get; set; }
        int PageSize { get; set; }
        int CurrentPage { get; set; }

        bool CanGoToFirst();
        void GoToFirst();

        bool CanGoToPrevious();
        void GoToPrevious();

        bool CanGoToNext();
        void GoToNext();

        bool CanGoToLast();
        void GoToLast();
    }
}
