using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
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
            var powershell = _mocks.Get<IRun>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return("test");
            _service.ListOfPackages();
            powershell.AssertWasCalled(mock => mock.Run("clist -source test"));
        }

    }
}