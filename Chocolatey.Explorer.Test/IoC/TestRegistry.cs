using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.ChocolateyService;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.Services.LogsService;
using Chocolatey.Explorer.Services.PackageService;
using Chocolatey.Explorer.Services.PackageVersionService;
using Chocolatey.Explorer.Services.PackagesService;
using Chocolatey.Explorer.Services.SettingsService;
using Chocolatey.Explorer.Services.SourceService;
using Chocolatey.Explorer.View.Controls;
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
			ObjectFactory.Inject<IAvailablePackagesService>(MockRepository.GenerateMock<IAvailablePackagesService>());
            ObjectFactory.Inject<IInstalledPackagesService>(MockRepository.GenerateMock<IInstalledPackagesService>());
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
            Assert.AreEqual(ObjectFactory.GetInstance<IPackageService>(), ObjectFactory.GetInstance<IPackageService>());
        }

        [Test]
        public void IfIAvailablePackagesServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IAvailablePackagesService>());
        }

        [Test]
        public void IfIAvailablePackagesServiceIsNotSingleton()
        {
            Assert.AreEqual(ObjectFactory.GetInstance<IAvailablePackagesService>(), ObjectFactory.GetInstance<IAvailablePackagesService>());
        }

        [Test]
        public void IfIInstalledPackagesServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IInstalledPackagesService>());
        }

        [Test]
        public void IfIInstalledPackagesServiceIsNotSingleton()
        {
            Assert.AreEqual(ObjectFactory.GetInstance<IInstalledPackagesService>(), ObjectFactory.GetInstance<IInstalledPackagesService>());
        }

        [Test]
        public void IfIPackageVersionServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageVersionService>());
        }

        [Test]
        public void IfIPackageVersionServiceIsNotSingleton()
        {
            Assert.AreEqual(ObjectFactory.GetInstance<IPackageVersionService>(), ObjectFactory.GetInstance<IPackageVersionService>());
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
        public void IfIFileStorageServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IFileStorageService>(), ObjectFactory.GetInstance<IFileStorageService>());
        }

        [Test]
        public void IfILogsServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<ILogsService>());
        }

        [Test]
        public void IfILogsServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<ILogsService>(), ObjectFactory.GetInstance<ILogsService>());
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

        [Test]
        public void IfILogsCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<ILogs>());
        }

        [Test]
        public void IfILogsIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<ILogs>(), ObjectFactory.GetInstance<ILogs>());
        }

        [Test]
        public void IfISettingsServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<ISettingsService>());
        }

        [Test]
        public void IfISettingsServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<ISettingsService>(), ObjectFactory.GetInstance<SettingsService>());
        }

        [Test]
        public void IfIChocolateyLibDirHelperCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IChocolateyLibDirHelper>());
        }

        [Test]
        public void IfIChocolateyLibDirHelperIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IChocolateyLibDirHelper>(), ObjectFactory.GetInstance<IChocolateyLibDirHelper>());
        }

        [Test]
        public void IfIPackageVersionXMLParserCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageVersionXMLParser>());
        }

        [Test]
        public void IfIPackageVersionXMLParserIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageVersionXMLParser>(), ObjectFactory.GetInstance<IPackageVersionXMLParser>());
        }

        [Test]
        public void IfICommandExecuterCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<ICommandExecuter>());
        }

        [Test]
        public void IfICommandExecuterIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<ICommandExecuter>(), ObjectFactory.GetInstance<ICommandExecuter>());
        }

        [Test]
        public void IfPackageVersionPanelGetsIPackageVersionService()
        {
            var panel = new PackageVersionPanel();
            Assert.IsNotNull(panel.PackageVersionService);
        }

        [Test]
        public void IfAvailablePackagesGirdGetsIAvailablePackagesService()
        {
            var grid = new AvailablePackagesGrid();
            Assert.IsNotNull(grid.AvailablePackagesService);
        }

        [Test]
        public void IfInstalledPackagesGridGetsIInstalledPackagesService()
        {
            var grid = new InstalledPackagesGrid();
            Assert.IsNotNull(grid.InstalledPackagesService);
        }

        [Test]
        public void IfPackagesButtonsPanelGetsPackageService()
        {
            var panel = new PackageButtonsPanel();
            Assert.IsNotNull(panel.PackageService);
        }

        [Test]
        public void IfPackagesButtonsPanelGetsPackageVersionService()
        {
            var panel = new PackageButtonsPanel();
            Assert.IsNotNull(panel.PackageVersionService);
        }

        [Test]
        public void IfPackageExtraInformationPanelGetsPackageVersionService()
        {
            var panel = new PackageExtraInformationPanel();
            Assert.IsNotNull(panel.PackageVersionService);
        }

        [Test]
        public void IfPackageRunPanelGetsPackageVersionService()
        {
            var panel = new PackageRunPanel();
            Assert.IsNotNull(panel.PackageVersionService);
        }

        [Test]
        public void IfPackageRunPanelGetsPackageService()
        {
            var panel = new PackageRunPanel();
            Assert.IsNotNull(panel.PackageService);
        }

        [Test]
        public void IfPackagesBaseGridGetsPackageVersionService()
        {
            var panel = new PackagesBaseGrid();
            Assert.IsNotNull(panel.PackageVersionService);
        }

        [Test]
        public void IfStatusBarGetsPackageVersionService()
        {
            var panel = new Statusbar();
            Assert.IsNotNull(panel.PackageVersionService);
        }

        [Test]
        public void IfStatusBarGetsPackageService()
        {
            var panel = new Statusbar();
            Assert.IsNotNull(panel.PackageService);
        }

        [Test]
        public void IfStatusBarGetsInstalledPackagesService()
        {
            var panel = new Statusbar();
            Assert.IsNotNull(panel.InstalledPackagesService);
        }

        [Test]
        public void IfStatusBarGetsAvailablePackagesService()
        {
            var panel = new Statusbar();
            Assert.IsNotNull(panel.AvailablePackagesService);
        }
    }
}