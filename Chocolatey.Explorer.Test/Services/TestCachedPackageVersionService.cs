using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.PackageVersionService;
using NUnit.Framework;
using Rhino.Mocks;

namespace Chocolatey.Explorer.Test.Services
{
    public class TestCachedPackageVersionService
    {
        [Test]
        public void IfDoesNotThrowAnExceptionWhenWeTryToAddPackageTwice()
        {
            var odata = MockRepository.GenerateMock<IODataPackageVersionService>();
            var service = new CachedPackageVersionService(odata);
            odata.GetEventRaiser(x => x.VersionChanged += null).Raise(new PackageVersion { Name = "test" });
            odata.GetEventRaiser(x => x.VersionChanged += null).Raise(new PackageVersion { Name = "test" });
       }

        [Test]
        public void IfDoesNotThrowAnExceptionWhenWeTryToAddTwoDifferentPackages()
        {
            var odata = MockRepository.GenerateMock<IODataPackageVersionService>();
            var service = new CachedPackageVersionService(odata);
            odata.GetEventRaiser(x => x.VersionChanged += null).Raise(new PackageVersion { Name = "test" });
            odata.GetEventRaiser(x => x.VersionChanged += null).Raise(new PackageVersion { Name = "test2" });
        }
    }
}