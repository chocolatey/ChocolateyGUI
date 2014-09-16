using System.Collections.Generic;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.Services
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
