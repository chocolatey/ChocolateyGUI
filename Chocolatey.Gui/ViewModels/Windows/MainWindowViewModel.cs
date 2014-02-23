using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.Utilities;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui.ViewModels.Windows
{
    public class MainWindowViewModel : ObservableBase, IMainWindowViewModel, IWeakEventListener
    {
        public ObservableCollection<SourceViewModel> Sources { get; set; }

        private SourceViewModel _selectedSourceViewModel;

        public SourceViewModel SelectedSource
        {
            get { return _selectedSourceViewModel; }
            set { SetPropertyValue(ref _selectedSourceViewModel, value); NotifyPropertyChanged("IsSelectedSource"); }
        }

        public bool IsSelectedSource
        {
            get { return _selectedSourceViewModel != null; }
        }

        private readonly ISourceService _sourceService;
        private readonly IProgressService _progressService;
        private readonly Lazy<IPackageService> _packageService; 

        public MainWindowViewModel(ISourceService sourceService, IProgressService progressService, Lazy<IPackageService> packageServiceLazy)
        {
            _sourceService = sourceService;
            _progressService = progressService;
            _packageService = packageServiceLazy;
            Sources = new ObservableCollection<SourceViewModel>(_sourceService.GetSources());

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
                        Sources.Add(source);
                    }
                }
                if (eventArgs.RemovedSources != null && eventArgs.RemovedSources.Count > 0)
                {
                    foreach (var source in eventArgs.RemovedSources)
                    {
                        Sources.Remove(source);
                    }
                }
            }
            return true;
        }

        private string _newSourceName;

        public string NewSourceName
        {
            get { return _newSourceName; }
            set { SetPropertyValue(ref _newSourceName, value); }
        }

        private string _newSourceUrl;
        public string NewSourceUrl
        {
            get { return _newSourceUrl; }
            set { SetPropertyValue(ref _newSourceUrl, value); }
        }

        public bool CanAddSource()
        {
            if (string.IsNullOrWhiteSpace(NewSourceName))
                return false;

            if (Sources.Any(s => String.Compare(s.Name, NewSourceName, StringComparison.InvariantCultureIgnoreCase) == 0))
                return false;

            if (string.IsNullOrWhiteSpace(NewSourceUrl))
                return false;

            if (Sources.Any(s => String.Compare(s.Url, NewSourceUrl, StringComparison.InvariantCultureIgnoreCase) == 0))
                return false;

            Uri url;
            if (!Uri.TryCreate(NewSourceUrl, UriKind.Absolute, out url))
                return false;

            return true;
        }

        public async void AddSource()
        {
            if (string.IsNullOrWhiteSpace(NewSourceName))
            {
                _progressService.ShowMessage("New Source", "Source must have a name.");
                return;
            }

            if (Sources.Any(s => String.Compare(s.Name, NewSourceName, StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _progressService.ShowMessage("New Source", "There's already a source with that name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewSourceUrl))
            {
                _progressService.ShowMessage("New Source", "Source must have a Url.");
                return;
            }

            if (Sources.Any(s => String.Compare(s.Url, NewSourceUrl, StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _progressService.ShowMessage("New Source", "There's already a source with that url.");
                return;
            }

            Uri url;
            if (!Uri.TryCreate(NewSourceUrl, UriKind.Absolute, out url))
            {
                _progressService.ShowMessage("New Source", "Source url is malformed.");
                return;
            }

            if (!(await _packageService.Value.TestSourceUrl(url)))
            {
                _progressService.ShowMessage("New Source", "Failed to query source.");
                return;
            }

            _sourceService.AddSource(new SourceViewModel{Name = NewSourceName, Url = NewSourceUrl});
            NewSourceName = "";
            NewSourceUrl = "";
        }

        public bool CanRemoveSource()
        {
            return IsSelectedSource;
        }

        public void RemoveSource()
        {
            if (SelectedSource == null)
            {
                _progressService.ShowMessage("New Source", "There's no selected source.");
                return;
            }

            var oldSource = SelectedSource;
            SelectedSource = null;
            _sourceService.RemoveSource(oldSource);
        }
    }
}
