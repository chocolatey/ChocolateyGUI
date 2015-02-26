// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IRemoteSourceControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using System.Collections.ObjectModel;
    using ChocolateyGui.ViewModels.Items;

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