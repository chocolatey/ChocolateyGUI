using System;
using System.Collections.Generic;
using System.Threading;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackagesService;
using Chocolatey.Explorer.Services.SourceService;
using Chocolatey.Explorer.Test.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestPackagesService
    {
        private RhinoAutoMocker<PackagesService> _mocks;
        private PackagesService _service;

        [SetUp]
        public void Setup()
        {
            _mocks = new RhinoAutoMocker<PackagesService>();
            _service = _mocks.ClassUnderTest;
        }

        [Test]
        public void IfListOfPackagesCallsPowershellRun()
        {
			var powershell = _mocks.Get<IRunAsync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return("test");
            _service.ListOfPackages();
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

        [Test]
        public void IfListOfInstalledPackagesCallsRunFinishedWhenHasPackages()
        {
            var libDirHelper = _mocks.Get<IChocolateyLibDirHelper>();
            var waitHandle = new AutoResetEvent(false);
            libDirHelper.Stub(x => x.ReloadFromDir()).Return(new List<Package>() { new Package() });
            IList<Package> result = new List<Package>();
            _service.RunFinshed += packages =>
                {
                    result = packages;
                    waitHandle.Set();
                };

            _service.ListOfInstalledPackages();

            waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(5));

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void IfListOfInstalledPackagesCallsRunFailedWhenReloadFromDirThrowsException()
        {
            var libDirHelper = _mocks.Get<IChocolateyLibDirHelper>();
            var waitHandle = new AutoResetEvent(false);
            libDirHelper.Stub(x => x.ReloadFromDir()).Throw(new Exception());
            var result = 0;
            _service.RunFailed += packages =>
            {
                result = 1;
                waitHandle.Set();
            };

            _service.ListOfInstalledPackages();

            waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(5));

            Assert.AreEqual(1, result);
        }

    }
}