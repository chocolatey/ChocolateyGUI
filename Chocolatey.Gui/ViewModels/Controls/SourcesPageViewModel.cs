using System;
using System.Collections.ObjectModel;
using System.Linq;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Properties;
using Chocolatey.Gui.ViewModels.Items;
using Chocolatey.Gui.Views.Controls;

namespace Chocolatey.Gui.ViewModels.Controls
{
    public class SourcesControlViewModel : ObservableBase, ISourcesControlViewModel
    {
        public ObservableCollection<SourceViewModel> Sources { get; set; }

        private SourceViewModel _selectedSource;

        public SourceViewModel SelectedSource
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

        public SourcesControlViewModel(Func<string, Uri, Type, SourceViewModel> sourceVmFactory)
        {

            Sources = new ObservableCollection<SourceViewModel>
            {
                sourceVmFactory("This PC", null, typeof(LocalSourceControl))
            };

            var sources = Settings.Default.sources;
            foreach (var parts in from string source in sources select source.Split('|'))
            {
                Sources.Add(sourceVmFactory(parts[0], new Uri(parts[1]), typeof (RemoteSourceControl)));
            }

            SelectedSource = Sources[0];
        }
    }
}
