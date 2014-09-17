using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels.Controls
{
    public interface IPackageControlViewModel
    {
        IPackageViewModel Package { get; set; }
    }
}
