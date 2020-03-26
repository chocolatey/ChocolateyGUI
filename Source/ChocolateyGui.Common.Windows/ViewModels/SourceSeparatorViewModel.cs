// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourceSeparatorViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.ViewModels;

namespace ChocolateyGui.Common.Windows.ViewModels
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