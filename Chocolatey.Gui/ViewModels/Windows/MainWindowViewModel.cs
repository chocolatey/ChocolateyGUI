using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Chocolatey.Gui.Base;
using Chocolatey.Gui.Models;

namespace Chocolatey.Gui.ViewModels.Windows
{
    public class MainWindowViewModel : ObservableBase, IMainWindowViewModel
    {
        public ObservableCollection<SourceModel> Sources { get; set; }

        public MainWindowViewModel()
        {
            Sources = new ObservableCollection<SourceModel>();
            var config = (ChocoConfigurationSection) System.Configuration.ConfigurationManager.GetSection("packageSources");
            foreach (SourceElement source in config.Sources)
            {
                Sources.Add(new SourceModel {Name = source.Name, Url = source.Url});
            }
        }
    }
}
