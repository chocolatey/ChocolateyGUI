using System.Collections.ObjectModel;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Controls
{
    public interface ISourcesControlViewModel
    {
        ObservableCollection<SourceViewModel> Sources { get; set; } 
    }
}
