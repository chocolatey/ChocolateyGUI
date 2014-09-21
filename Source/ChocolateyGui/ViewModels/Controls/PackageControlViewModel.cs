// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using ChocolateyGui.Base;
    using ChocolateyGui.ViewModels.Items;

    public class PackageControlViewModel : ObservableBase, IPackageControlViewModel
    {
        public IPackageViewModel Package { get; set; }
    }
}