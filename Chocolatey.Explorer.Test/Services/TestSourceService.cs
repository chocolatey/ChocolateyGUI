using System.Collections.Generic;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestSourceService
    {
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
            Assert.AreEqual("Chocolatey.org",source.Name);
        }

    }
}