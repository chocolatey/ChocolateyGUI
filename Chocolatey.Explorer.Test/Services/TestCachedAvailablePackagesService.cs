using System;
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
    public class TestCachedAvailablePackagesService
    {
        private RhinoAutoMocker<CachedAvailablePackagesService> _mocks;
        private CachedAvailablePackagesService _service;

        [SetUp]
        public void Setup()
        {
            _mocks = new RhinoAutoMocker<CachedAvailablePackagesService>();
            _service = _mocks.ClassUnderTest;
        }

        [Test]
        public void IfListOfPackagesCallsPackagesServiceListOfPackagesWhenNoCache()
        {
            var packagesService = _mocks.Get<IODataAvailablePackagesService>();
            _service.ListOfAvailablePackages();
            packagesService.AssertWasCalled(mock => mock.ListOfAvailablePackages());
        }

        [Test]
        public void IfRaisesRunFinishedOnPackagesServiceRunFinishedWhenNoCache()
        {
            var packagesService = _mocks.Get<IODataAvailablePackagesService>();
            var result = 0;
            _service.RunFinshed += (x) => result = 1;
            packagesService.GetEventRaiser(x => x.RunFinshed += null).Raise(new List<Package>());

            Assert.AreEqual(1, result);
        }

        [Test]
        public void IfRaisesRunFailedOnPackagesServiceRunFailedWhenNoCache()
        {
            var packagesService = _mocks.Get<IODataAvailablePackagesService>();
            var result = 0;
            _service.RunFailed += (x) => result = 1;
            packagesService.GetEventRaiser(x => x.RunFailed += null).Raise(new Exception());

            Assert.AreEqual(1, result);
        }

        [Test]
        public void IfRaisesRunRunStartedOnListOfAvailablePackages()
        {
            var result = 0;
            _service.RunStarted += (x) => result = 1;
            _service.ListOfAvailablePackages();
            Assert.AreEqual(1, result);
        } 
    }
}