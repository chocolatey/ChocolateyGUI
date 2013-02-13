using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestPackageService
    {
        private RhinoAutoMocker<PackageService> _mocks;
        private PackageService _service;

        [SetUp]
        public void Setup()
        {
            _mocks = new RhinoAutoMocker<PackageService>();
            _service = _mocks.ClassUnderTest;
        }

        [Test]
        public void IfInstallPackageCallsPowershellRun()
        {
            var powershell = _mocks.Get<IRunAsync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return("test");
            _service.InstallPackage("test");
            powershell.AssertWasCalled(mock => mock.Run("cinst test -source test"));
        }

        [Test]
        public void IfUpdatePackageCallsPowershellRun()
        {
			var powershell = _mocks.Get<IRunAsync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return("test");
            _service.UpdatePackage("test");
            powershell.AssertWasCalled(mock => mock.Run("cup test -source test"));
        }
    }
}