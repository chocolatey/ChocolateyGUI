// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesChangedEventArgs.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    using System;
    using System.Collections.Generic;
    using ChocolateyGui.ViewModels.Items;

    public delegate void SourcesChangedEventHandler(object sender, SourcesChangedEventArgs e);

    public class SourcesChangedEventArgs : EventArgs
    {
        public SourcesChangedEventArgs(List<SourceViewModel> newSources, List<SourceViewModel> removedSources)
        {
            this.AddedSources = newSources;
            this.RemovedSources = removedSources;
        }

        public List<SourceViewModel> AddedSources { get; private set; }

        public List<SourceViewModel> RemovedSources { get; private set; }
    }
}