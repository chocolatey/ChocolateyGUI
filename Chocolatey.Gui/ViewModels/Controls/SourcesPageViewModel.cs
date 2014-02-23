using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.Utilities;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.Views.Controls;

namespace Chocolatey.Gui.ViewModels.Controls
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
                if (_selectedSource != null && _selectedSource != value)
                    _selectedSource.IsSelected = false;
                value.IsSelected = true;
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
