// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ISourceService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChocolateyGui.Models;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Services
{
    public interface ISourceService
    {
        event SourcesChangedEventHandler SourcesChanged;

        void AddSource(SourceViewModel sourceViewModel);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate")]
        SourceViewModel GetDefaultSource();

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate")]
        IEnumerable<SourceViewModel> GetSources();

        void RemoveSource(SourceViewModel sourceViewModel);
    }
}