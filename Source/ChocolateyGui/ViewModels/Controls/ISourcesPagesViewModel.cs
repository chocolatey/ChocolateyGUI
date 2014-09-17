using System.Collections.ObjectModel;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels.Controls
{
    public interface ISourcesControlViewModel
    {
        ObservableCollection<SourceTabViewModel> Sources { get; set; } 
    }
}
