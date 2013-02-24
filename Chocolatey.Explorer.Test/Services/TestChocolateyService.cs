using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services.ChocolateyService;
using Chocolatey.Explorer.Services.SourceService;
using NUnit.Framework;
using StructureMap.AutoMocking;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Services
{
    [TestFixture]
    public class TestChocolateyService
    {
        private RhinoAutoMocker<ChocolateyService> _mocks;
        private ChocolateyService _service;

        [SetUp]
        public void Setup()
        {
            _mocks = new RhinoAutoMocker<ChocolateyService>();
            _service = _mocks.ClassUnderTest;
        }

        [Test]
        public void IfLatestVersionCallsPowershellRun()
        {
			var powershell = _mocks.Get<IRunSync>();
            var sourceService = _mocks.Get<ISourceService>();
            sourceService.Expect(x => x.Source)
                .Return(new Source { Url = "test" });
            _service.LatestVersion();
            powershell.AssertWasCalled(mock => mock.Run("cver" + " -source test"));
        }

        [Test]
        public void IfHelpCallsPowershellRun()
        {
            var powershell = _mocks.Get<IRunSync>();
            _service.Help();
            powershell.AssertWasCalled(mock => mock.Run("chocolatey /?"));
        }

        [Test]
        public void IfRunFinishedGetsCalledOnPowershellRun()
        {
            var powershell = _mocks.Get<IRunSync>();
            var result = 0;
            _service.RunFinished += () => result = 1;
            powershell.GetEventRaiser(x => x.RunFinished += null).Raise();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void IfOutputChangedGetsCalledOnPowershellRun()
        {
            var powershell = _mocks.Get<IRunSync>();
            var result = "";
            _service.OutputChanged += (x) => result = x;
            powershell.GetEventRaiser(x => x.OutputChanged += null).Raise("test");
            Assert.AreEqual("test", result);
        }
    }
}