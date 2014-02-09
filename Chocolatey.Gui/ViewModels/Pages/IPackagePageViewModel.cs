using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Pages
{
    public interface IPackagePageViewModel
    {
        IPackageViewModel Package { get; set; }
    }
}
