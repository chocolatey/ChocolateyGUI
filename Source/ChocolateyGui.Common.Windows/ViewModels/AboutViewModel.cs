// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AboutViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Caliburn.Micro;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Services;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public sealed class AboutViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IVersionService _versionService;

        public AboutViewModel(IEventAggregator eventAggregator, IVersionService versionService)
        {
            _eventAggregator = eventAggregator;
            _versionService = versionService;
        }

        public string ChocolateyGuiVersion
        {
            get { return _versionService.Version; }
        }

        public string ChocolateyGuiInformationalVersion
        {
            get { return _versionService.InformationalVersion; }
        }

        public void Back()
        {
            _eventAggregator.PublishOnUIThread(new AboutGoBackMessage());
        }
    }
}