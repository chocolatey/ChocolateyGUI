// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourceSeparatorViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels
{
    public sealed class SourceSeparatorViewModel : ISourceViewModelBase
    {
        public string DisplayName
        {
            get
            {
                return "Separator";
            }
        }
    }
}