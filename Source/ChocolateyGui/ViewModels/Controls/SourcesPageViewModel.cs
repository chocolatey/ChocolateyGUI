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

namespace ChocolateyGui.ViewModels.Controls
{
    public class SourcesControlViewModel : ObservableBase, ISourcesControlViewModel, IWeakEventListener
    {
        public ObservableCollection<SourceTabViewModel> Sources { get; set; }

        private SourceTabViewModel _selectedSource;

        public SourceTabViewModel SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                if (value != null && value.Content == null)
                    value.LoadContent();
                SetPropertyValue(ref _selectedSource, value);
            }
        }

        private readonly Func<string, Uri, Type, SourceTabViewModel> _sourceVmFactory;

        public SourcesControlViewModel(ISourceService sourceService, Func<string, Uri, Type, SourceTabViewModel> sourceVmFactory)
        {
            _sourceVmFactory = sourceVmFactory;
            Sources = new ObservableCollection<SourceTabViewModel>
            {
                sourceVmFactory("This PC", null, typeof(LocalSourceControl))
            };

            foreach (var source in sourceService.GetSources())
            {
                Sources.Add(_sourceVmFactory(source.Name, new Uri(source.Url), typeof(RemoteSourceControl)));
            }

            SelectedSource = Sources[0];

            SourcesChangedEventManager.AddListener(sourceService, this);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (sender is ISourceService)
            {
                var eventArgs = e as SourcesChangedEventArgs;
                if (eventArgs.AddedSources != null && eventArgs.AddedSources.Count > 0)
                {
                    foreach (var source in eventArgs.AddedSources)
                    {
                        Sources.Add(_sourceVmFactory(source.Name, new Uri(source.Url), typeof(RemoteSourceControl)));
                    }
                }
                if (eventArgs.RemovedSources != null && eventArgs.RemovedSources.Count > 0)
                {
                    foreach (var targetPackage in eventArgs.RemovedSources
                        .Select(source => Sources.FirstOrDefault(p => String.Equals(p.Name, source.Name, StringComparison.CurrentCultureIgnoreCase)))
                        .Where(targetPackage => targetPackage != null))
                    {
                        Sources.Remove(targetPackage);
                    }
                }
            }
            return true;
        }
    }
}
