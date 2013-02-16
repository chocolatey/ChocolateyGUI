using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.View;
using Chocolatey.Explorer.View.Forms;
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
        public void IfIPackageManagerCanBeResolved()
        {
			var fileStorage = MockRepository.GenerateMock<IFileStorageService>();
			fileStorage.Stub(fs => fs.DirectoryExists(Arg<string>.Is.Anything)).Return(true);
			InjectSetupForPackageManager(fileStorage);

			Assert.IsNotNull(ObjectFactory.GetInstance<IPackageManager>());
        }

        [Test]
		public void IfIPackageManagerIsNotSingleton()
        {
			var fileStorage = MockRepository.GenerateMock<IFileStorageService>();
			fileStorage.Stub(fs => fs.DirectoryExists(Arg<string>.Is.Anything)).Return(true);
			InjectSetupForPackageManager(fileStorage);

			Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageManager>(), ObjectFactory.GetInstance<IPackageManager>());
        }

		private void InjectSetupForPackageManager(IFileStorageService fileStorageService)
		{
			ObjectFactory.Inject<IFileStorageService>(fileStorageService);
			ObjectFactory.Inject<IPackagesService>(MockRepository.GenerateMock<IPackagesService>());
			ObjectFactory.Inject<IPackageVersionService>(MockRepository.GenerateMock<IPackageVersionService>());
			ObjectFactory.Inject<IPackageService>(MockRepository.GenerateMock<IPackageService>());
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

        [Test]
        public void IfIHelpCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IHelp>());
        }

        [Test]
        public void IfIHelpIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IHelp>(), ObjectFactory.GetInstance<IHelp>());
        }

        [Test]
        public void IfIAboutCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IAbout>());
        }

        [Test]
        public void IfIAboutIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IAbout>(), ObjectFactory.GetInstance<IAbout>());
        }

        [Test]
        public void IfISettingsCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<ISettings>());
        }

        [Test]
        public void IfISettingsIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<ISettings>(), ObjectFactory.GetInstance<ISettings>());
        }
    }
}