using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.Services.PackagesService;

namespace Chocolatey.Explorer.Services.SourceService
{
    public class SourceService : ISourceService
    {
        private IList<Source> _sources;
        private Source _currentsource;

		private readonly IFileStorageService _fileStorageService;
        private readonly IAvailablePackagesService _availablePackagesService;

        public delegate void SourcesDelegate(IList<Source> sources);

        public delegate void CurrentSourceDelegate(Source source);

        public event SourcesDelegate SourcesChanged;
        public event CurrentSourceDelegate CurrentSourceChanged;

		public SourceService(IFileStorageService fileStorageService)
        {
			_fileStorageService = fileStorageService;
		    Initialize();
        }

        public void Initialize()
        {
            this.Log().Debug("Initialize");
            LoadSources();
            SetCurrentSource(_sources[0]);
        }
        
        public void LoadSources()
        {
            this.Log().Debug("Loadsources");
            _sources = new List<Source>();
			var document = _fileStorageService.LoadXDocument("sources.xml");
            var sources = document.Elements("sources").Elements("source");
            foreach (var xElement in sources)
            {
                this.Log().Debug("Added source: {0} {1}", xElement.Element("name").Value, xElement.Element("url").Value);
                _sources.Add(new Source { Name = xElement.Element("name").Value, Url = xElement.Element("url").Value });
            }
            OnSourcesChanged(_sources);
        }

        public void SetCurrentSource(Source source)
        {
            this.Log().Debug("Current source: {0}", source);
            _currentsource = source;
            OnCurrentSourceChanged(_currentsource);
        }

        private void OnCurrentSourceChanged(Source currentsource)
        {
            this.Log().Debug("Current source changed: {0}", currentsource);
            var handler = CurrentSourceChanged;
            if (handler != null) handler(currentsource);
        }

        public Source Source
        {
            get { return _currentsource; }
        }

        public void AddSource(Source source)
        {
            this.Log().Debug("Add source: {0}", source);
            _sources.Add(source);
            SaveSources();
        }

        private void SaveSources()
        {
            this.Log().Debug("Savesources");
            LoadSources();
        }

        private void OnSourcesChanged(IList<Source> sources)
        {
            this.Log().Debug("Sources changed: {0}", sources);
            var handler = SourcesChanged;
            if (handler != null) handler(sources);
        }
    }
}
