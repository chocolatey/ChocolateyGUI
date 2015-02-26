// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SettingsSourceService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using ChocolateyGui.Models;
    using ChocolateyGui.Properties;
    using ChocolateyGui.ViewModels.Items;

    internal class SettingsSourceService : ISourceService
    {
        public event SourcesChangedEventHandler SourcesChanged;

        public void AddSource(SourceViewModel sourceViewModel)
        {
            if (sourceViewModel == null)
            {
                throw new ArgumentNullException("sourceViewModel");
            }

            Settings.Default.sources.Add(string.Format(CultureInfo.CurrentCulture, "{0}|{1}", sourceViewModel.Name, sourceViewModel.Url));
            var sourcesChangedEvent = this.SourcesChanged;
            if (sourcesChangedEvent != null)
            {
                sourcesChangedEvent(
                    this,
                    new SourcesChangedEventArgs(new List<SourceViewModel> { sourceViewModel }, new List<SourceViewModel>()));
            }

            Settings.Default.Save();
        }

        public SourceViewModel GetDefaultSource()
        {
            var defaultSourceName = Settings.Default.currentSource;
            var defaultSource =
                this.GetSources()
                    .FirstOrDefault(
                        s => string.Compare(defaultSourceName, s.Name, StringComparison.OrdinalIgnoreCase) == 0);

            return defaultSource ?? this.GetSources().First();
        }

        public IEnumerable<SourceViewModel> GetSources()
        {
            var sources = Settings.Default.sources;
            return (from string source in sources select source.Split('|'))
                .Select(parts => new SourceViewModel { Name = parts[0], Url = parts[1] });
        }

        public void RemoveSource(SourceViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            Settings.Default.sources.Remove(string.Format(CultureInfo.CurrentCulture, "{0}|{1}", viewModel.Name, viewModel.Url));
            var sourcesChangedEvent = this.SourcesChanged;
            if (sourcesChangedEvent != null)
            {
                sourcesChangedEvent(
                    this,
                    new SourcesChangedEventArgs(new List<SourceViewModel>(), new List<SourceViewModel> { viewModel }));
            }

            Settings.Default.Save();
        }
    }
}