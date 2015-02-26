// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IMainWindowViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Windows
{
    using System.Collections.ObjectModel;
    using ChocolateyGui.ViewModels.Items;

    public interface IMainWindowViewModel
    {
        ObservableCollection<SourceViewModel> Sources { get; set; }
    }
}