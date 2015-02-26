// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IPackageControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using ChocolateyGui.ViewModels.Items;
    
    public interface IPackageControlViewModel
    {
        IPackageViewModel Package { get; set; }
    }
}