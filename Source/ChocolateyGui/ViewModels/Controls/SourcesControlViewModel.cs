// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesControlViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using ChocolateyGui.Base;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    using ChocolateyGui.Utilities;
    using ChocolateyGui.ViewModels.Items;
    using ChocolateyGui.Views.Controls;

    public class SourcesControlViewModel : ObservableBase, ISourcesControlViewModel, IWeakEventListener
    {
        private readonly Func<string, Uri, Type, SourceTabViewModel> sourceViewModelFactory;

        private SourceTabViewModel _selectedSource;

        public SourcesControlViewModel(ISourceService sourceService, Func<string, Uri, Type, SourceTabViewModel> sourceViewModelFactory)
        {
            if (sourceService == null)
            {
                throw new ArgumentNullException("sourceService");
            }

            if (sourceViewModelFactory == null)
            {
                throw new ArgumentNullException("sourceViewModelFactory");
            }

            this.sourceViewModelFactory = sourceViewModelFactory;
            this.Sources = new ObservableCollection<SourceTabViewModel>
            {
                sourceViewModelFactory("This PC", null, typeof(LocalSourceControl))
            };

            foreach (var source in sourceService.GetSources())
            {
                this.Sources.Add(this.sourceViewModelFactory(source.Name, new Uri(source.Url), typeof(RemoteSourceControl)));
            }

            this.SelectedSource = this.Sources[0];

            SourcesChangedEventManager.AddListener(sourceService, this);
        }

        public SourceTabViewModel SelectedSource
        {
            get
            {
                return this._selectedSource;
            }

            set
            {
                if (value != null && value.Content == null)
                {
                    value.LoadContent();
                }

                this.SetPropertyValue(ref this._selectedSource, value);
            }
        }

        public ObservableCollection<SourceTabViewModel> Sources { get; set; }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (sender is ISourceService)
            {
                var eventArgs = e as SourcesChangedEventArgs;
                if (eventArgs.AddedSources != null && eventArgs.AddedSources.Count > 0)
                {
                    foreach (var source in eventArgs.AddedSources)
                    {
                        this.Sources.Add(this.sourceViewModelFactory(source.Name, new Uri(source.Url), typeof(RemoteSourceControl)));
                    }
                }

                if (eventArgs.RemovedSources != null && eventArgs.RemovedSources.Count > 0)
                {
                    foreach (var targetPackage in eventArgs.RemovedSources
                        .Select(source => this.Sources.FirstOrDefault(p => string.Equals(p.Name, source.Name, StringComparison.CurrentCultureIgnoreCase)))
                        .Where(targetPackage => targetPackage != null))
                    {
                        this.Sources.Remove(targetPackage);
                    }
                }
            }

            return true;
        }
    }
}