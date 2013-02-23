using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackagesService;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TestClearCacheInstalledPackagesCommand
    {
        [Test]
        public void IfIPackageVersionIsICacheableThenShouldInvalidateCache()
        {
            var sut = new ClearCacheInstalledPackagesCommand();
            sut.InstalledPackagesService = MockRepository.GenerateMock<IFakeCacheableInstalledPackagesService>();
            sut.Execute();
            sut.InstalledPackagesService.AssertWasCalled(x => ((ICacheable)x).InvalidateCache());
        }

        public interface IFakeCacheableInstalledPackagesService : IInstalledPackagesService, ICacheable { }  
    }
}