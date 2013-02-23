using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services.PackageService;
using Chocolatey.Explorer.Services.SourceService;
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
                .Return(new Source {Url="test"});
            _service.InstallPackage("test");
            powershell.AssertWasCalled(mock => mock.Run("cinst test -source test"));
        }

        [Test]
        public void IfUnInstallPackageCallsPowershellRun()
        {
            var powershell = _mocks.Get<IRunAsync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return(new Source { Name = "test" });
            _service.UninstallPackage("test");
            powershell.AssertWasCalled(mock => mock.Run("cuninst test"));
        }

        [Test]
        public void IfUpdatePackageCallsPowershellRun()
        {
			var powershell = _mocks.Get<IRunAsync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return(new Source { Url = "test" });
            _service.UpdatePackage("test");
            powershell.AssertWasCalled(mock => mock.Run("cup test -source test"));
        }

        [Test]
        public void IfRaisesOutputChangedOnRun()
        {
            var powershell = _mocks.Get<IRunAsync>();
            var result = "";
            _service.LineChanged += line => result = line;
            powershell.GetEventRaiser(x => x.OutputChanged += null).Raise("test");

            Assert.AreEqual("test", result);
        }

        [Test]
        public void IfRaisesRunFinishedOnRun()
        {
            var powershell = _mocks.Get<IRunAsync>();
            var result = 0;
            _service.RunFinshed += () => result = 1;
            powershell.GetEventRaiser(x => x.RunFinished += null).Raise();

            Assert.AreEqual(1, result);
        }
    }
}