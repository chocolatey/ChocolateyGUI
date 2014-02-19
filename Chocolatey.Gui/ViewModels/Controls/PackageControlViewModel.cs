using Chocolatey.Gui.Base;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Controls
{
    public class PackageControlViewModel : ObservableBase, IPackageControlViewModel
    {
        public IPackageViewModel Package { get; set; }
    }
}
