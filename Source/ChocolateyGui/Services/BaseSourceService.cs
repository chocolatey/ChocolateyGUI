// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BaseSourceService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System.Collections.Generic;
    using ChocolateyGui.Models;
    using ChocolateyGui.ViewModels.Items;

    public abstract class BaseSourceService
    {
        public event SourcesChangedEventHandler SourcesChanged;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "We are raising an event")]
        protected void RaiseSourceAddedEvent(SourceViewModel sourceViewModel)
        {
            var sourcesChangedEvent = this.SourcesChanged;
            if (sourcesChangedEvent != null)
            {
                sourcesChangedEvent(
                    this,
                    new SourcesChangedEventArgs(new List<SourceViewModel> { sourceViewModel }, new List<SourceViewModel>()));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "We are raising an event")]
        protected void RaiseSourceRemovedEvent(SourceViewModel sourceViewModel)
        {
            var sourcesChangedEvent = this.SourcesChanged;
            if (sourcesChangedEvent != null)
            {
                sourcesChangedEvent(
                    this,
                    new SourcesChangedEventArgs(new List<SourceViewModel>(), new List<SourceViewModel> { sourceViewModel }));
            }
        }
    }
}