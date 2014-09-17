using ChocolateyGui.Base;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels.Controls
{
    public class PackageControlViewModel : ObservableBase, IPackageControlViewModel
    {
        public IPackageViewModel Package { get; set; }
    }
}
