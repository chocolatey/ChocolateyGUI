using System;
using System.Collections.Generic;
using System.Threading;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackagesService;
using Chocolatey.Explorer.Test.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace Chocolatey.Explorer.Test.Services
{
    public class TestInstalledPackagesService
    {

        private RhinoAutoMocker<InstalledPackagesService> _mocks;
        private IInstalledPackagesService _service;

        [SetUp]
        public void Setup()
        {
            _mocks = new RhinoAutoMocker<InstalledPackagesService>();
            _service = _mocks.ClassUnderTest;
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

            _service.ListOfIntalledPackages();

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

            _service.ListOfIntalledPackages();

            waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(5));

            Assert.AreEqual(1, result);
        } 
    }
}