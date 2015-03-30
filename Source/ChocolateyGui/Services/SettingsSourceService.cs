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
    using ChocolateyGui.Providers;
    using ChocolateyGui.ViewModels.Items;

    internal class SettingsSourceService : ISourceService
    {
        private readonly IChocolateyConfigurationProvider _chocoConfig;

        public SettingsSourceService(IChocolateyConfigurationProvider chocoConfig)
        {
            this._chocoConfig = chocoConfig;
        }

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
            var sourcesAsTuples = (from string source in sources select source.Split('|'))
                .Select(s => Tuple.Create(s[0], s[1]));

            return sourcesAsTuples.Union(this._chocoConfig.Sources, new SourceTupleComparer())
                .Select(parts => new SourceViewModel { Name = parts.Item1, Url = parts.Item2 });
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

        internal class SourceTupleComparer : IEqualityComparer<Tuple<string, string>>
        {
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                return this.GetHashCode(x) == this.GetHashCode(y);
            }

            public int GetHashCode(Tuple<string, string> obj)
            {
                if (object.ReferenceEquals(obj, null))
                {
                    throw new ArgumentNullException("obj");
                }

                // URIs are functionally equivalent with or without trailing slashes.
                var url = obj.Item2.TrimEnd('/', '\\');

                return unchecked(obj.Item1.ToUpperInvariant().GetHashCode() // Name
                    + url.GetHashCode());
            }
        }
    }
}