// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ISourceService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System.Collections.Generic;
    using ChocolateyGui.Models;
    using ChocolateyGui.ViewModels.Items;

    public interface ISourceService
    {
        event SourcesChangedEventHandler SourcesChanged;

        void AddSource(SourceViewModel sourceViewModel);

        SourceViewModel GetDefaultSource();

        IEnumerable<SourceViewModel> GetSources();

        void RemoveSource(SourceViewModel viewModel);
    }
}