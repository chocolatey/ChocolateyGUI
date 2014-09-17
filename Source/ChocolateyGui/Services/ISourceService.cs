using System.Collections.Generic;
using ChocolateyGui.Models;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Services
{
    public interface ISourceService
    {
        IEnumerable<SourceViewModel> GetSources();
        SourceViewModel GetDefaultSource();
        void AddSource(SourceViewModel svm);
        void RemoveSource(SourceViewModel svm);

        event SourcesChangedEventHandler SourcesChanged;
    }
}
