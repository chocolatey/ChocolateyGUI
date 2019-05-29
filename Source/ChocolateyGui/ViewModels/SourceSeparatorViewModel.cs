// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourceSeparatorViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.ViewModels;

namespace ChocolateyGui.ViewModels
{
    public sealed class SourceSeparatorViewModel : ISourceViewModelBase
    {
        private const string DISPLAYNAME = "Separator";

        public SourceSeparatorViewModel()
        {
            DisplayName = DISPLAYNAME;
        }

        public string DisplayName { get; }
    }
}