using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Controls
{
    public interface IPackageControlViewModel
    {
        IPackageViewModel Package { get; set; }
    }
}
