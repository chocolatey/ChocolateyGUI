// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Caliburn.Micro;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.ViewModels.Items;

namespace ChocolateyGui.Common.Windows.ViewModels
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