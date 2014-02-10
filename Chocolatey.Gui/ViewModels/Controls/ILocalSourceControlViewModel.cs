using System.Collections.ObjectModel;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Controls
{
    public interface ILocalSourceControlViewModel
    {
        ObservableCollection<PackageViewModel> Packages { get; set; }
    }
}
