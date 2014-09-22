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
        IEnumerable<SourceViewModel> GetSources();

        SourceViewModel GetDefaultSource();

        void AddSource(SourceViewModel svm);

        void RemoveSource(SourceViewModel svm);

        event SourcesChangedEventHandler SourcesChanged;
    }
}