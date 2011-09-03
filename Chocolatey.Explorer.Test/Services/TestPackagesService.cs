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
        private RhinoAutoMocker<PackagesService> mocks;
        private PackagesService service;

        [SetUp]
        public void Setup()
        {
            mocks = new RhinoAutoMocker<PackagesService>();
            service = mocks.ClassUnderTest;
        }

        [Test]
        public void IfListOfPackagesCallsPowershellRun()
        {
            var powershell = mocks.Get<IRun>();
            service.ListOfPackages();
            powershell.AssertWasCalled(mock => mock.Run("clist -source " + Settings.Source));
        }

    }
}