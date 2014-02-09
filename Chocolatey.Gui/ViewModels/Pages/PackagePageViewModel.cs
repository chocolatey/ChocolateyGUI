using Autofac;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Pages
{
    public class PackagePageViewModel : ObservableBase, IPackagePageViewModel
    {
        public IPackageViewModel Package { get; set; }
    }
}
