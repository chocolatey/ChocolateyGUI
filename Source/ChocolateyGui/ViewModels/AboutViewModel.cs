// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AboutViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Caliburn.Micro;
using ChocolateyGui.Models.Messages;

namespace ChocolateyGui.ViewModels
{
    public sealed class AboutViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;

        public AboutViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Back()
        {
            _eventAggregator.PublishOnUIThread(new AboutGoBackMessage());
        }
    }
}