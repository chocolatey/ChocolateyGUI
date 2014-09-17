using System.Collections.ObjectModel;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels.Windows
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<SourceViewModel> Sources { get; set; }
    }
}
