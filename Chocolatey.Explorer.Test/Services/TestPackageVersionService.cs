using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestPackageVersionService
    {
        private RhinoAutoMocker<PackageVersionService> mocks;
        private PackageVersionService service;

        [SetUp]
        public void Setup()
        {
            mocks = new RhinoAutoMocker<PackageVersionService>();
            service = mocks.ClassUnderTest;
        }

        [Test]
        public void IfInstallPackageCallsPowershellRun()
        {
            var powershell = mocks.Get<IRun>();
            service.PackageVersion("test");
            powershell.AssertWasCalled(mock => mock.Run("cver test -source " + Settings.Source));
        }

    }
}