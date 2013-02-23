using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TestClearCacheAll
    {
        private ClearCacheAllCommand _sut;
        [SetUp]
        public void Setup()
        {
            _sut = new ClearCacheAllCommand();
            _sut.CommandExecuter = MockRepository.GenerateMock<ICommandExecuter>();
            _sut.Execute();
        }

        [Test]
        public void IfExecutesCommandClearCachePackageVersion()
        {
           _sut.CommandExecuter.AssertWasCalled(x => x.Execute<ClearCachePackageVersionCommand>());
        }

        [Test]
        public void IfExecutesCommandClearCacheInstalledPackages()
        {
            _sut.CommandExecuter.AssertWasCalled(x => x.Execute<ClearCacheInstalledPackagesCommand>());
        }

        [Test]
        public void IfExecutesCommandClearCacheAvailablePackages()
        {
            _sut.CommandExecuter.AssertWasCalled(x => x.Execute<ClearCacheAvailablePackagesCommand>());
        }
    }
}