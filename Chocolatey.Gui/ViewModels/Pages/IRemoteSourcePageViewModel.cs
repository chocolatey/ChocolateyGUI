using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Pages
{
    public interface IRemoteSourcePageViewModel
    {
        ObservableCollection<PackageViewModel> Packages { get; set; }
    }
}
