using System;
using System.Collections.Generic;
using System.Linq;
using ChocolateyGui.Models;
using ChocolateyGui.Properties;
using ChocolateyGui.ViewModels.Items;

namespace ChocolateyGui.Services
{
    internal class SettingsSourceService : ISourceService
    {
        public IEnumerable<SourceViewModel> GetSources()
        {
            var sources = Settings.Default.sources;
            return (from string source in sources select source.Split('|'))
                .Select(parts => new SourceViewModel {Name = parts[0], Url = parts[1]});
        }

        public SourceViewModel GetDefaultSource()
        {
            var defaultSourceName = Settings.Default.currentSource;
            var defaultSource =
                GetSources()
                    .FirstOrDefault(
                        s => string.Compare(defaultSourceName, s.Name, StringComparison.InvariantCultureIgnoreCase) == 0);

            return defaultSource ?? GetSources().First();
        }

        public void AddSource(SourceViewModel svm)
        {
            Settings.Default.sources.Add(string.Format("{0}|{1}", svm.Name, svm.Url));
            var sourcesChangedEvent = SourcesChanged;
            if (sourcesChangedEvent != null)
                sourcesChangedEvent(this, new SourcesChangedEventArgs(new List<SourceViewModel> { svm }, new List<SourceViewModel>()));

            Settings.Default.Save();
        }

        public void RemoveSource(SourceViewModel svm)
        {
            Settings.Default.sources.Remove(string.Format("{0}|{1}", svm.Name, svm.Url));
            var sourcesChangedEvent = SourcesChanged;
            if (sourcesChangedEvent != null)
                sourcesChangedEvent(this, new SourcesChangedEventArgs(new List<SourceViewModel>(), new List<SourceViewModel> { svm }));

            Settings.Default.Save();
        }


        public event SourcesChangedEventHandler SourcesChanged;
    }
}
