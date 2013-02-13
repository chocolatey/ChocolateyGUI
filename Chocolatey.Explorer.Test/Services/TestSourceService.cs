using System.Collections.Generic;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services;
using NUnit.Framework;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestSourceService
    {
		private List<SampleSource> _sources;

		private SampleSource ChocolateySource { get { return _sources.Single(s => s.Name == "Chocolatey.org"); } }

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			LoadSourcesForDependentTests();
		}

		private void LoadSourcesForDependentTests()
		{
			if (!File.Exists("sources.xml"))
				Assert.Fail("The sources.xml file is necessary for several tests, but is not available in the local folder");

			var xdoc = XDocument.Load("sources.xml");
			var sources =
			_sources = xdoc.Descendants().Where(d => d.Name.LocalName == "source")
												 .Select(s => new SampleSource() { 
													Name = s.Descendants().Single(d => d.Name.LocalName == "name").Value,
													Url = s.Descendants().Single(d => d.Name.LocalName == "url").Value
												 })
												 .ToList();

			if (_sources.Count == 0)
				Assert.Fail("Could not load necessary source urls from the sources.xml file");
		}

        [Test]
        public void IfSourcesAreLoadedWhenInstantiated()
        {
            var sourceService = new SourceService();
            IList<Source> sources = null;
            sourceService.SourcesChanged += x => sources = x;
            sourceService.Initialize();
            Assert.IsNotNull(sources);
        }

        [Test]
        public void IfSourcesHave2ElementsWhenInstantiated()
        {
            var sourceService = new SourceService();
            IList<Source> sources = null;
            sourceService.SourcesChanged += x => sources = x;
            sourceService.Initialize();
            Assert.AreEqual(2,sources.Count);
        }

        [Test]
        public void IfCurrentSourceIsNotEmptyWhenInstantiated()
        {
            var sourceService = new SourceService();
            Source source = null;
            sourceService.CurrentSourceChanged += x => source = x;
            sourceService.Initialize();
            Assert.IsNotNull(source);
        }

        [Test]
        public void IfCurrentSourceIsTheFirstSourceWhenInstantiated()
        {
            var sourceService = new SourceService();
            Source source = null;
            sourceService.CurrentSourceChanged += x => source = x;
            sourceService.Initialize();
			Assert.AreEqual(ChocolateySource.Name, source.Name);
        }

        [Test]
        public void IfSourceReturnsTheUrlOfTheCurrentSource()
        {
            var sourceService = new SourceService();
            Source source = null;
            sourceService.CurrentSourceChanged += x => source = x;
            Assert.AreEqual(ChocolateySource.Url, sourceService.Source);
        }

        [Test]
        public void IfSetCurrentSourceSetsTheCurrentsource()
        {
            var sourceService = new SourceService();
            Source source = null;
            sourceService.CurrentSourceChanged += x => source = x;
			Assert.AreEqual(ChocolateySource.Url, sourceService.Source);
        }

		private class SampleSource
		{
			public string Name { get; set; }
			public string Url { get; set; }
		}
    }
}