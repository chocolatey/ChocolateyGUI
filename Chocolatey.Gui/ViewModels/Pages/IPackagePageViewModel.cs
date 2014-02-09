using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chocolatey.Gui.ViewModels.Pages
{
    public interface IPackagePageViewModel
    {
        IPackagePageViewModel Package { get; set; }
    }
}
