// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocoSettingsSourceService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using ChocolateyGui.AsyncProcess;
    using ChocolateyGui.Properties;
    using ChocolateyGui.Providers;
    using ChocolateyGui.ViewModels.Items;

    public class ChocoSettingsSourceService : BaseSourceService, ISourceService
    {
        private readonly IChocolateyConfigurationProvider chocolateyConfigurationProvider;
        private string chocoExePath = string.Empty;
        private IEnumerable<SourceViewModel> _sources;

        public ChocoSettingsSourceService(IChocolateyConfigurationProvider chocolateyConfigurationProvider)
        {
            if (chocolateyConfigurationProvider == null)
            {
                throw new ArgumentNullException("chocolateyConfigurationProvider");
            }

            this.chocolateyConfigurationProvider = chocolateyConfigurationProvider;
            this.chocoExePath = Path.Combine(this.chocolateyConfigurationProvider.ChocolateyInstall, "choco.exe");

            // Transfer sources stored in old sources settings into new one
            if (Settings.Default.sources != null)
            {
                foreach (var source in Settings.Default.sources)
                {
                    var parts = source.Split('|');
                    var sourceViewModel = new SourceViewModel { Name = parts[0], Url = parts[1] };
                    this.AddSource(sourceViewModel);
                }
            }

            Settings.Default.sources = new StringCollection();
            Settings.Default.Save();
        }

        public async void AddSource(SourceViewModel sourceViewModel)
        {
            if (sourceViewModel == null)
            {
                throw new ArgumentNullException("sourceViewModel");
            }

            var arguments = new StringBuilder();
            arguments.AppendFormat("source add -n=\"{0}\" -s=\"{1}\" -y", sourceViewModel.Name, sourceViewModel.Url);

            await ProcessEx.RunAsync(this.chocoExePath, arguments.ToString());

            this._sources = null;
            this.GetSources();
            this.RaiseSourceAddedEvent(sourceViewModel);
        }

        public SourceViewModel GetDefaultSource()
        {
            var defaultSourceName = Settings.Default.currentSource;
            var defaultSource =
                this._sources
                    .FirstOrDefault(
                        s => string.Compare(defaultSourceName, s.Name, StringComparison.OrdinalIgnoreCase) == 0);

            return defaultSource ?? this._sources.First();
        }

        public IEnumerable<SourceViewModel> GetSources()
        {
            if (this._sources != null)
            {
                return this._sources;
            }

            var arguments = new StringBuilder();
            arguments.AppendFormat(CultureInfo.InvariantCulture, "source list");

            var results = ProcessEx.Run(this.chocoExePath, arguments.ToString());

            this._sources = (from source in results.StandardOutput 
                    select source.Split('-') 
                    into parts 
                    where parts.Count() == 2 
                    select new SourceViewModel { Name = parts[0].Trim(), Url = parts[1].Trim() }).ToList();

            return this._sources;
        }

        public async void RemoveSource(SourceViewModel sourceViewModel)
        {
            if (sourceViewModel == null)
            {
                throw new ArgumentNullException("sourceViewModel");
            }

            var arguments = new StringBuilder();
            arguments.AppendFormat("source remove -n=\"{0}\" -y", sourceViewModel.Name);

            await ProcessEx.RunAsync(this.chocoExePath, arguments.ToString());

            this._sources = null;
            this.GetSources();
            this.RaiseSourceRemovedEvent(sourceViewModel);
        }
    }
}