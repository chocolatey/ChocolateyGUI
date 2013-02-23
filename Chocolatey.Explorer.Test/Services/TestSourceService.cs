using System.Collections.Generic;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.SourceService;
using NUnit.Framework;
using System.Xml.Linq;
using System.Linq;
using Rhino.Mocks;
using Chocolatey.Explorer.Services.FileStorageService;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestSourceService
    {
		
		[TestFixtureSetUp]
		public void FixtureSetup()
		{		}

        [Test]
        public void IfSourcesAreLoadedWhenInstantiated()
        {
            var sourceService = new SourceService(GetMockFileStorageWithSources());
            IList<Source> sources = null;
            sourceService.SourcesChanged += x => sources = x;
            sourceService.Initialize();
            Assert.IsNotNull(sources);
        }

        [Test]
        public void IfSourcesHave2ElementsWhenInstantiated()
        {
            var sourceService = new SourceService(GetMockFileStorageWithSources());
            IList<Source> sources = null;
            sourceService.SourcesChanged += x => sources = x;
            sourceService.Initialize();
            Assert.AreEqual(2,sources.Count);
        }

        [Test]
        public void IfCurrentSourceIsNotEmptyWhenInstantiated()
        {
            var sourceService = new SourceService(GetMockFileStorageWithSources());
            Source source = null;
            sourceService.CurrentSourceChanged += x => source = x;
            sourceService.Initialize();
            Assert.IsNotNull(source);
        }


		private IFileStorageService GetMockFileStorageWithSources(string firstName = "First Name", string firstUrl = "http://first.url.com", 
																  string secondName = "Second Name", string secondUrl = "http://second.url.com")
		{
            var fakeData = string.Format("<sources><source><name>{0}</name><url>{1}</url><hasodatafeed>1</hasodatafeed></source><source><name>{2}</name><url>{3}</url><hasodatafeed>1</hasodatafeed></source></sources>",
									firstName, firstUrl, secondName, secondUrl);
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.LoadXDocument("sources.xml")).Return(XDocument.Parse(fakeData));
			return fileStorageService;
		}

        [Test]
        public void IfCurrentSourceIsTheFirstSourceWhenInstantiated()
        {
			const string expectedFirstName = "First Entry";
            var sourceService = new SourceService(GetMockFileStorageWithSources(expectedFirstName, secondName: "SECOND!1One!1"));
            Source source = null;
            sourceService.CurrentSourceChanged += x => source = x;
            
			sourceService.Initialize();

			Assert.AreEqual(expectedFirstName, source.Name);
        }

        [Test]
        public void IfSourceReturnsTheUrlOfTheCurrentSource()
        {
			const string expectedUrl = "http://first.url.com/notSureIunderstandThisTest?";
            var sourceService = new SourceService(GetMockFileStorageWithSources(firstUrl: expectedUrl, secondUrl: "http://totally.different.com"));
			
			sourceService.Initialize();

			Assert.AreEqual(expectedUrl, sourceService.Source.Url);
        }

        [Test]
        public void IfSetCurrentSourceSetsTheCurrentsource()
        {
			var newService = new Source { Name = "Third Service", Url = "http://third.entry.com"};
            var sourceService = new SourceService(GetMockFileStorageWithSources());
			
			sourceService.SetCurrentSource(newService);

			Assert.AreEqual(newService, sourceService.Source);
        }

		[Test]
		public void IfSetCurrentSourceWithRealFileSystemSetsTheSourceToFirstEntryInFile()
		{
            var sourceService = new SourceService(new LocalFileSystemStorageService());
			var firstEntry = XDocument.Load("sources.xml").Descendants().First(d => d.Name.LocalName == "name").Value;	// yeah, this totally won't be the first thing to break :P
			Source source = null;
			sourceService.CurrentSourceChanged += x => source = x;

			sourceService.Initialize();

			Assert.AreEqual(firstEntry, source.Name);
		}
    }
}