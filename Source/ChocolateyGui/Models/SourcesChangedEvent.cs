using System;
using System.Collections.Generic;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Models
{
    public delegate void SourcesChangedEventHandler(object sender, SourcesChangedEventArgs e);

    public class SourcesChangedEventArgs : EventArgs
    {
        public SourcesChangedEventArgs(List<SourceViewModel> newSources, List<SourceViewModel> removedSources)
        {
            AddedSources = newSources;
            RemovedSources = removedSources;
        }
        public List<SourceViewModel> AddedSources { get; private set; }
        public List<SourceViewModel> RemovedSources { get; private set; }
    }
}
