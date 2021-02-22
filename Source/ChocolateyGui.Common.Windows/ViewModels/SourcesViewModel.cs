// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.ViewModels;
using ChocolateyGui.Common.Windows.Services;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public class SourcesViewModel : Conductor<ISourceViewModelBase>.Collection.OneActive, IHandleWithTask<SourcesUpdatedMessage>
    {
        private readonly IChocolateyService _packageService;
        private readonly CreateRemove _remoteSourceVmFactory;
        private readonly IConfigService _configService;
        private readonly IImageService _imageService;
        private readonly IVersionService _versionService;
        private readonly Func<string, LocalSourceViewModel> _localSourceVmFactory;
        private bool _firstLoad = true;

        public SourcesViewModel(
            IChocolateyService packageService,
            IConfigService configService,
            IImageService imageService,
            IEventAggregator eventAggregator,
            IVersionService versionService,
            Func<string, LocalSourceViewModel> localSourceVmFactory,
            CreateRemove remoteSourceVmFactory)
        {
            _packageService = packageService;
            _configService = configService;
            _imageService = imageService;
            _versionService = versionService;
            _remoteSourceVmFactory = remoteSourceVmFactory;
            _localSourceVmFactory = localSourceVmFactory;

            if (localSourceVmFactory == null)
            {
                throw new ArgumentNullException(nameof(localSourceVmFactory));
            }

            if (remoteSourceVmFactory == null)
            {
                throw new ArgumentNullException(nameof(remoteSourceVmFactory));
            }

            eventAggregator.Subscribe(this);
        }

        public delegate RemoteSourceViewModel CreateRemove(ChocolateySource source);

        public ImageSource PrimaryApplicationImageSource
        {
            get { return _imageService.PrimaryApplicationImage; }
        }

        public ImageSource SecondaryApplicationImageSource
        {
            get { return _imageService.SecondaryApplicationImage; }
        }

        public string DisplayVersion
        {
            get { return _versionService.DisplayVersion; }
        }

        public virtual async Task LoadSources()
        {
            var oldItems = Items.Skip(1).Cast<ISourceViewModelBase>().ToList();

            var sources = await _packageService.GetSources();
            var vms = new List<ISourceViewModelBase>();

            if (_configService.GetEffectiveConfiguration().ShowAggregatedSourceView ?? false)
            {
                vms.Add(_remoteSourceVmFactory(new ChocolateyAggregatedSources()));
                vms.Add(new SourceSeparatorViewModel());
            }

            foreach (var source in sources.Where(s => !s.Disabled).OrderBy(s => s.Priority))
            {
                vms.Add(_remoteSourceVmFactory(source));
            }

            await Execute.OnUIThreadAsync(
                () =>
                    {
                        Items.RemoveRange(oldItems);
                        Items.AddRange(vms);

                        ActivateItem(Items[0]);
                    });
        }

        public async Task Handle(SourcesUpdatedMessage message)
        {
            await LoadSources();
        }

        protected override void OnViewReady(object view)
        {
            Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                .Where(p => p.EventArgs.PropertyName == nameof(ActiveItem))
                .Subscribe(p => DisplayName = $"Source - {ActiveItem?.DisplayName}");

            if (_firstLoad)
            {
                Items.Add(_localSourceVmFactory(Resources.Resources_ThisPC));

                _ = LoadSources();
                _firstLoad = false;
            }
        }

        private class SourcesComparer : IEqualityComparer<RemoteSourceViewModel>
        {
            public bool Equals(RemoteSourceViewModel x, RemoteSourceViewModel y)
            {
                return x.Source.Equals(y.Source);
            }

            public int GetHashCode(RemoteSourceViewModel obj)
            {
                return obj.Source.GetHashCode();
            }
        }
    }
}