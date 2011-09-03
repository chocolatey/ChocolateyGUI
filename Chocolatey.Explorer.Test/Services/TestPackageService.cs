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
        private RhinoAutoMocker<PackageService> mocks;
        private PackageService service;

        [SetUp]
        public void Setup()
        {
            mocks = new RhinoAutoMocker<PackageService>();
            service = mocks.ClassUnderTest;
        }

        [Test]
        public void IfInstallPackageCallsPowershellRun()
        {
            var powershell = mocks.Get<IRun>();
            service.InstallPackage("test");
            powershell.AssertWasCalled(mock => mock.Run("cup test -source " + Settings.Source));
        }

        [Test]
        public void IfUpdatePackageCallsPowershellRun()
        {
            var powershell = mocks.Get<IRun>();
            service.UpdatePackage("test");
            powershell.AssertWasCalled(mock => mock.Run("cinst test -source " + Settings.Source));
        }
    }
}