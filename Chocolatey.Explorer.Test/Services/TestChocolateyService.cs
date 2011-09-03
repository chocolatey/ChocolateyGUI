using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using NUnit.Framework;
using StructureMap.AutoMocking;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestChocolateyService
    {
        private RhinoAutoMocker<ChocolateyService> mocks;
        private ChocolateyService service;

        [SetUp]
        public void Setup()
        {
            mocks = new RhinoAutoMocker<ChocolateyService>();
            service = mocks.ClassUnderTest;
        }

        [Test]
        public void IfLatestVersionCallsPowershellRun()
        {
            var powershell = mocks.Get<IRun>();
            service.LatestVersion();
            powershell.AssertWasCalled(mock => mock.Run("cver" + " -source " + Settings.Source));
        }

        [Test]
        public void IfHelpCallsPowershellRun()
        {
            var powershell = mocks.Get<IRun>();
            service.Help();
            powershell.AssertWasCalled(mock => mock.Run("chocolatey /?"));
        }
    }
}