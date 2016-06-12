// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ShellViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Providers;
using ChocolateyGui.Services;
using ChocolateyGui.Utilities;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IHandle<ShowPackageDetailsMessage>, IHandle<ShowSourcesMessage>
    {
        private readonly IVersionNumberProvider _versionNumberProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly SourcesViewModel _sourcesViewModel;

        public ShellViewModel(ISourceService sourceService,
            IVersionNumberProvider versionNumberProvider,
            IEventAggregator eventAggregator,
            SourcesViewModel sourcesViewModel)
        {
            if (sourceService == null)
            {
                throw new ArgumentNullException(nameof(sourceService));
            }

            _versionNumberProvider = versionNumberProvider;
            _eventAggregator = eventAggregator;
            _sourcesViewModel = sourcesViewModel;
            Sources = new BindableCollection<SourceViewModel>(sourceService.GetSources());
            ActiveItem = _sourcesViewModel;
        }

        public void ShowSettings()
        {
        }

        public void ShowAbout()
        {
        }

        public void Handle(ShowPackageDetailsMessage message)
        {
            ActiveItem = new PackageViewModel(_eventAggregator) { Package = message.Package };
        }

        public void Handle(ShowSourcesMessage message)
        {
            ActiveItem = _sourcesViewModel;
        }

        public string AboutInformation
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.ABOUT.md");

        public string ReleaseNotes
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.CHANGELOG.md");

        public string Credits
            => ResourceReader.GetFromResources(GetType().Assembly, "ChocolateyGui.Resources.CREDITS.md");

        public string VersionNumber => _versionNumberProvider.Version;

        public BindableCollection<SourceViewModel> Sources { get; set; }

        public SourcesViewModel SourcesSelectorViewModel => _sourcesViewModel;

        protected override void OnInitialize()
        {
            _eventAggregator.Subscribe(this);
        }
    }
}