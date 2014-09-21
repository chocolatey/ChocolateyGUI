// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ISourcesControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using ChocolateyGui.ViewModels.Items;
    using System.Collections.ObjectModel;

    public interface ISourcesControlViewModel
    {
        ObservableCollection<SourceTabViewModel> Sources { get; set; } 
    }
}