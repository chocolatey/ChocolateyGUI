// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ILocalSourceControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using ChocolateyGui.ViewModels.Items;
    using System;
    using System.Collections.ObjectModel;

    public interface ILocalSourceControlViewModel
    {
        ObservableCollection<IPackageViewModel> Packages { get; set; }

        string SearchQuery { get; set; }
        bool MatchWord { get; set; }

        string SortColumn { get; set; }
        bool SortDescending { get; set; }

        void Loaded(object sender, EventArgs e);

        bool CanUpdateAll();
        void UpdateAll();
    }
}