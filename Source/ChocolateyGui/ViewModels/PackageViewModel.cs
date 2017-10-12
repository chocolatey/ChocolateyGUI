// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Properties;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels
{
    public sealed class PackageViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;

        public PackageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public IPackageViewModel Package { get; set; }

        public new string DisplayName => string.Format(Resources.PackageViewModel_DisplayName, Package?.Title);

        public void Back()
        {
            _eventAggregator.PublishOnUIThread(new ShowSourcesMessage(null));
        }
    }
}