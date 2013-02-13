using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.View;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace Chocolatey.Explorer.Test.IoC
{
    [TestFixture]
    public class TestRegistry
    {

        [SetUp]
        public void Setup()
        {
            ObjectFactory.Configure(configure => configure.AddRegistry<Registry>());
        }

        [Test]
		[Ignore("Inject a file ssytem that returns true for the directory exists call")]
        public void IfIPackageManagerCanBeResolved()
        {
			Assert.IsNotNull(ObjectFactory.GetInstance<IPackageManager>());
        }

        [Test]
		[Ignore("Inject a file ssytem that returns true for the directory exists call")]
		public void IfIPackageManagerIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageManager>(), ObjectFactory.GetInstance<IPackageManager>());
        }

        [Test]
        public void IfIPackageServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageService>());
        }

        [Test]
        public void IfIPackageServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageService>(), ObjectFactory.GetInstance<IPackageService>());
        }

        [Test]
        public void IfIPackagesServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackagesService>());
        }

        [Test]
        public void IfIPackagesServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackagesService>(), ObjectFactory.GetInstance<IPackagesService>());
        }

        [Test]
        public void IfIPackageVersionServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageVersionService>());
        }

        [Test]
        public void IfIPackageVersionServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageVersionService>(), ObjectFactory.GetInstance<IPackageVersionService>());
        }

        [Test]
        public void IfIChocolateyServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IChocolateyService>());
        }

        [Test]
        public void IfIChocolateyServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IChocolateyService>(), ObjectFactory.GetInstance<IChocolateyService>());
        }

		[Test]
		public void IfIRunAsyncCanBeResolved()
		{
			Assert.IsNotNull(ObjectFactory.GetInstance<IRunAsync>());
		}

		[Test]
		public void IfIRunAsyncIsNotSingleton()
		{
			Assert.AreNotEqual(ObjectFactory.GetInstance<IRunSync>(), ObjectFactory.GetInstance<IRunAsync>());
		}

        [Test]
        public void IfIRunSyncCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IRunSync>());
        }

        [Test]
        public void IfIRunSyncIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IRunSync>(), ObjectFactory.GetInstance<IRunSync>());
        }

        [Test]
        public void IfISourceServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<ISourceService>());
        }

        [Test]
        public void IfISourceServiceIsSingleton()
        {
            Assert.AreEqual(ObjectFactory.GetInstance<ISourceService>(), ObjectFactory.GetInstance<ISourceService>());
        }

		[Test]
		public void IfIFileStorageServiceCanBeResolved()
		{
			Assert.IsNotNull(ObjectFactory.GetInstance<IFileStorageService>());
		}
    }
}