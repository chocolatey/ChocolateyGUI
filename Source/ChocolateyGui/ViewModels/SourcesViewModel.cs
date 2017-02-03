// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;
using ChocolateyGui.Subprocess.Models;

namespace ChocolateyGui.ViewModels
{
    public sealed class SourcesViewModel : Conductor<ISourceViewModelBase>.Collection.OneActive, IHandleWithTask<SourcesUpdatedMessage>
    {
        private readonly IChocolateyPackageService _packageService;
        private readonly CreateRemove _remoteSourceVmFactory;

        private bool _firstLoad = true;

        public SourcesViewModel(
            ISourceService sourceService,
            IChocolateyPackageService packageService,
            IEventAggregator eventAggregator,
            Func<string, LocalSourceViewModel> localSourceVmFactory,
            CreateRemove remoteSourceVmFactory)
        {
            _packageService = packageService;
            _remoteSourceVmFactory = remoteSourceVmFactory;
            if (sourceService == null)
            {
                throw new ArgumentNullException(nameof(sourceService));
            }

            if (localSourceVmFactory == null)
            {
                throw new ArgumentNullException(nameof(localSourceVmFactory));
            }

            if (remoteSourceVmFactory == null)
            {
                throw new ArgumentNullException(nameof(remoteSourceVmFactory));
            }

            Items.Add(localSourceVmFactory(Resources.Resources_ThisPC));

#pragma warning disable 4014
            LoadSources();
#pragma warning restore 4014

            eventAggregator.Subscribe(this);
        }

        public delegate RemoteSourceViewModel CreateRemove(ChocolateySource source);

        public async Task LoadSources()
        {
            var oldItems = Items.Skip(1).Cast<RemoteSourceViewModel>().ToList();

            var sources = await _packageService.GetSources();
            var vms = new List<RemoteSourceViewModel>();
            foreach (var source in sources.Where(s => !s.Disabled).OrderBy(s => s.Priority))
            {
                vms.Add(_remoteSourceVmFactory(source));
            }

            await Execute.OnUIThreadAsync(
                () =>
                    {
                        ActivateItem(Items[0]);

                        Items.RemoveRange(oldItems);
                        Items.AddRange(vms);
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
                .Subscribe(p => DisplayName = $"Source - {ActiveItem.DisplayName}");

            if (_firstLoad)
            {
                ActivateItem(Items[0]);
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