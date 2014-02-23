using System.Collections.ObjectModel;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Windows
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<SourceViewModel> Sources { get; set; }
    }
}
