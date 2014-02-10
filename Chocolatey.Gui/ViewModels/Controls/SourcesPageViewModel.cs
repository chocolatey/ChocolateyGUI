using System.Collections.ObjectModel;
using Autofac;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Services;
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

        public SourcesControlViewModel()
        {
            var service = App.Container.Resolve<IPackageService>();
            Sources = new ObservableCollection<SourceViewModel>
            {
                new SourceViewModel(service) { Name ="Local", PageType = typeof(LocalSourceControl) }
            };

            var config = (ChocoConfigurationSection)System.Configuration.ConfigurationManager.GetSection("chocolatey");
            foreach (SourceElement source in config.Sources)
            {
                Sources.Add(new SourceViewModel(service) { Name = source.Name, Url = source.Url, PageType = typeof(RemoteSourceControl) });
            }
        }
    }
}
