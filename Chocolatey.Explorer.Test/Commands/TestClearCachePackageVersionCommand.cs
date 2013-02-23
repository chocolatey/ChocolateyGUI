using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackageVersionService;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TestClearCachePackageVersionCommand
    {
        [Test]
        public void IfIPackageVersionIsICacheableThenShouldInvalidateCache()
        {
            var sut = new ClearCachePackageVersionCommand();
            sut.PackageVersionService = MockRepository.GenerateMock<IFakeCacheablePackageVersionService>();
            sut.Execute();
            sut.PackageVersionService.AssertWasCalled(x => ((ICacheable)x).InvalidateCache());
        }

        public interface IFakeCacheablePackageVersionService: IPackageVersionService, ICacheable {}
        
    }
}