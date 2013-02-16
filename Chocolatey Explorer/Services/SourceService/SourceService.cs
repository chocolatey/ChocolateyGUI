using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.FileStorageService;

namespace Chocolatey.Explorer.Services
{
    public class SourceService : ISourceService
    {
        private IList<Source> _sources;
        private Source _currentsource;

		private readonly IFileStorageService _fileStorageService;

        public delegate void SourcesDelegate(IList<Source> sources);

        public delegate void CurrentSourceDelegate(Source source);

        public event SourcesDelegate SourcesChanged;
        public event CurrentSourceDelegate CurrentSourceChanged;

		public SourceService() : this(new LocalFileSystemStorageService()) { }

        public SourceService(IFileStorageService fileStorageService)
        {
			_fileStorageService = fileStorageService;
            Initialize();
        }

        public void Initialize()
        {
            LoadSources();
            SetCurrentSource(_sources[0]);
        }
        
        private void LoadSources()
        {
            _sources = new List<Source>();
			var document = _fileStorageService.LoadXDocument("sources.xml");
            var sources = document.Elements("sources").Elements("source");
            foreach (var xElement in sources)
            {
                _sources.Add(new Source() {Name = xElement.Element("name").Value,Url = xElement.Element("url").Value});
            }
            OnSourcesChanged(_sources);
        }

        public void SetCurrentSource(Source source)
        {
            _currentsource = source;
            OnCurrentSourceChanged(_currentsource);
        }

        private void OnCurrentSourceChanged(Source currentsource)
        {
            var handler = CurrentSourceChanged;
            if (handler != null) handler(currentsource);
        }

        public String Source
        {
            get { return _currentsource.Url; }
        }

        public void AddSource(Source source)
        {
            _sources.Add(source);
            SaveSources();
        }

        private void SaveSources()
        {
            LoadSources();
        }

        private void OnSourcesChanged(IList<Source> sources)
        {
            var handler = SourcesChanged;
            if (handler != null) handler(sources);
        }
    }
}
