using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.PackageVersionService;
using Chocolatey.Explorer.Services.PackagesService;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Commands
{
    public class TextClearCacheAvailablePackagesCommand
    {
        [Test]
        public void IfIPackageVersionIsICacheableThenShouldInvalidateCache()
        {
            var sut = new ClearCacheAvailablePackagesCommand();
            sut.AvailablePackagesService = MockRepository.GenerateMock<IFakeCacheableAvailablePackagesService>();
            sut.Execute();
            sut.AvailablePackagesService.AssertWasCalled(x => ((ICacheable)x).InvalidateCache());
        }

        public interface IFakeCacheableAvailablePackagesService : IAvailablePackagesService, ICacheable { } 
    }
}