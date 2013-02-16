using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackageVersionService;
using Chocolatey.Explorer.Services.SourceService;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestPackageVersionService
    {
        private RhinoAutoMocker<PackageVersionService> _mocks;
        private PackageVersionService _service;

        [SetUp]
        public void Setup()
        {
            _mocks = new RhinoAutoMocker<PackageVersionService>();
            _service = _mocks.ClassUnderTest;
        }

        [Test]
        public void IfInstallPackageCallsPowershellRun()
        {
			var powershell = _mocks.Get<IRunAsync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return("test");
            _service.PackageVersion("test");
            powershell.AssertWasCalled(mock => mock.Run("cver test -source test"));
        }

    }
}