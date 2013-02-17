using System.Collections.Generic;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services.PackagesService;
using Chocolatey.Explorer.Services.SourceService;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestAvailablePackagesService
    {
        private RhinoAutoMocker<AvailablePackagesService> _mocks;
        private AvailablePackagesService _service;

        [SetUp]
        public void Setup()
        {
            _mocks = new RhinoAutoMocker<AvailablePackagesService>();
            _service = _mocks.ClassUnderTest;
        }

        [Test]
        public void IfListOfPackagesCallsPowershellRun()
        {
			var powershell = _mocks.Get<IRunAsync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return("test");
            _service.ListOfAvalablePackages();
            powershell.AssertWasCalled(mock => mock.Run("clist -source test"));
        }

        [Test]
        public void IfRaisesRunFinishedOnRunWithoutLines()
        {
            var powershell = _mocks.Get<IRunAsync>();
            var result = 0;
            _service.RunFinshed += (x) => result = 1;
            powershell.GetEventRaiser(x => x.RunFinished += null).Raise();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void IfRaisesRunFinishedOnRunWithLines()
        {
            var powershell = _mocks.Get<IRunAsync>();
            IList<Package> result = new List<Package>();
            _service.RunFinshed += (x) => result = x;
            powershell.GetEventRaiser(x => x.OutputChanged += null).Raise("test testversion");
            powershell.GetEventRaiser(x => x.RunFinished += null).Raise();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("test", result[0].Name);
            Assert.AreEqual("testversion", result[0].InstalledVersion);
        }
    }
}