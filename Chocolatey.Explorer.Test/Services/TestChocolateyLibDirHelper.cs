using Chocolatey.Explorer.Services.ChocolateyService;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.Services.SettingsService;
using NUnit.Framework;
using System;
using System.Linq;
using Chocolatey.Explorer.Services;
using Rhino.Mocks;
using System.IO;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Test.Services
{
	[TestFixture]
	public class TestChocolateyLibDirHelper
	{
	    private ISettingsService _settingsService;
        private IFileStorageService _fileStorageService;
			
        [SetUp]
        public void Setup()
        {
            _fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            _settingsService = MockRepository.GenerateMock<ISettingsService>();
            _settingsService.Stub(ssS => ssS.ChocolateyLibDirectory).Return("");
        }

		[Test]
		public void IfReloadFromDirWithInvalidDirectoryThenThrowsException()
		{
			_fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Throw(new DirectoryNotFoundException());

            var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), _fileStorageService, _settingsService);

            Assert.Throws<DirectoryNotFoundException>(() => helper.ReloadFromDir());

		}

		[Test]
		public void IfReloadFromDirWithInvalidEnvironmentVariableThenThrowsException()
		{
			_fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Throw(new Exception("Posing as HREsult-based exception that environment would throw, per the way the logic is currently written"));
           
            var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), _fileStorageService, _settingsService);

            Assert.Throws<Exception>(() => helper.ReloadFromDir());
		}

		[Test]
		public void IfReloadFromDirWithEmptyDirectoryThenReturnsListWithChocolatelyOnly()
		{
			_fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
            
            var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), _fileStorageService, _settingsService);

			var result = helper.ReloadFromDir();

			Assert.AreEqual("chocolatey", result.Single().Name);
		}

		[Test]
		public void IfReloadFromDirAndHelpTextIsUnrecognizedThenThrowsChocoVersionUnknownException()
		{
			_fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
			
            var chocolatelyService = new FakeChocolateyService("not a valid chocolatey version string");
            var helper = new ChocolateyLibDirHelper(chocolatelyService, _fileStorageService, _settingsService);

            Assert.Throws<ChocolateyVersionUnknownException>(() => helper.ReloadFromDir());
		}

		[Test]
		public void IfReloadFromDirAndHelpTextIsCorrectPatternThenChocolateyPackageContainsProperVersion()
		{
			_fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
            
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

			var result = helper.ReloadFromDir();

			Assert.AreEqual("0.9.8.20", result.Single().InstalledVersion);
		}

        [Test]
        public void IfFindsVersionFromDirectory()
        {
            _fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("0.1.5", result[0].InstalledVersion);
        }

        [Test]
        public void IfFindsNameFromDirectory()
        {
            _fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("ChocolateyGUI", result[0].Name);
        }

        [Test]
        public void IfFindsVersionFromDirectoryWithPre()
        {
            _fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5-pre"});
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("0.1.5", result[0].InstalledVersion);
        }

        [Test]
        public void IfFindsNameFromDirectoryWithPre()
        {
            _fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5-pre" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("ChocolateyGUI", result[0].Name);
        }

        [Test]
        public void IfGetHighestInstalledVersionFindsHighestVersionWhenAllNumbersAreSingleDigits()
        {
            _fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5", "ChocolateyGUI.0.1.6" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

            var result = helper.GetHighestInstalledVersion("ChocolateyGUI", true);

            Assert.AreEqual("0.1.6", result.InstalledVersion);
        }

        [Test]
        public void IfGetHighestInstalledVersionFindsHighestVersionWhenLastNumberIsDoubledigit()
        {
            _fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.15", "ChocolateyGUI.0.1.6" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

            var result = helper.GetHighestInstalledVersion("ChocolateyGUI", true);

            Assert.AreEqual("0.1.15", result.InstalledVersion);
        }

        [Test]
        public void IfGetHighestInstalledVersionFindsHighestVersionWhenSecondNumberIsDoubleDigit()
        {
            _fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.11.5", "ChocolateyGUI.0.1.6" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

            var result = helper.GetHighestInstalledVersion("ChocolateyGUI", true);

            Assert.AreEqual("0.11.5", result.InstalledVersion);
        }

		/// <summary>
		/// Follows versioning section at https://github.com/chocolatey/chocolatey/wiki/CreatePackages
		/// and nuspec versioning: http://docs.nuget.org/docs/reference/versioning
		/// </summary>
		[TestCase("BasicOnePart.1",							"BasicOnePart",				"1",                false)]
		[TestCase("BasicTwoPart.1.1",						"BasicTwoPart",				"1.1",              false)]
		[TestCase("BasicThreePart.1.1.1",					"BasicThreePart",			"1.1.1",            false)]
		[TestCase("BasicFourPart.1.1.1.1",					"BasicFourPart",			"1.1.1.1",          false)]
		[TestCase("MixedDigitLengths.1.10.981.1234",		"MixedDigitLengths",		"1.10.981.1234",    false)]
		[TestCase("DatePackageFixNotation.1.1.1.20130215",	"DatePackageFixNotation",	"1.1.1.20130215",   false)]
		[TestCase("SemVerPreRelease.1.1.1.1-pre",			"SemVerPreRelease",			"1.1.1.1",          true)]
		[TestCase("SemVerCustomPreRelease.1.1.1.1-stuff",	"SemVerCustomPreRelease",	"1.1.1.1",			true)]
		[TestCase("OnePartNameWith.Decimal.1.1.1.1",		"OnePartNameWith.Decimal",	"1.1.1.1",          false)]
		[TestCase("TwoPartNameWith.Decimal.1.1.1.1",		"TwoPartNameWith.Decimal",	"1.1.1.1",          false)]
		[TestCase("ThreePartNameWith.Decimal.1.1.1.1",		"ThreePartNameWith.Decimal","1.1.1.1",          false)]
		[TestCase("FourPartNameWith.Decimal.1.1.1.1",		"FourPartNameWith.Decimal",	"1.1.1.1",          false)]
		[TestCase("FourPartPreNameWith.Decimal.1.1.1.1-pre", "FourPartPreNameWith.Decimal", "1.1.1.1",      true)]
		[TestCase("OnePartNumericEnd2.1",					"OnePartNumericEnd2",		"1",                false)]
		[TestCase("TwoPartNumericEnd2.1.1",					"TwoPartNumericEnd2",		"1.1",              false)]
		[TestCase("ThreePartNumericEnd2.1.1.1",				"ThreePartNumericEnd2",		"1.1.1",            false)]
		[TestCase("FourPartNumericEnd2.1.1.1.1",			"FourPartNumericEnd2",		"1.1.1.1",          false)]
		[TestCase("FourPartPreNumericEnd2.1.1.1.1-pre",		"FourPartPreNumericEnd2",	"1.1.1.1",          true)]
		public void IfGetPackageFromDirectoryNameReceivesNameThatIsValidPerRecomendations(string filename, string expectedPackageName, string expectedVersion, bool prerelease)
		{
			var expectedPackage = new Package() { 
				Name = expectedPackageName, 
				InstalledVersion = expectedVersion,
                IsPreRelease = prerelease
			};
			var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), _fileStorageService, _settingsService);

			var actualPackage = helper.GetPackageFromDirectoryName(filename);

			Assert.AreEqual(expectedPackage.Name, actualPackage.Name);
			Assert.AreEqual(expectedPackage.InstalledVersion, actualPackage.InstalledVersion);
            Assert.AreEqual(expectedPackage.IsPreRelease, actualPackage.IsPreRelease);
		}

		private class FakeChocolateyService : IChocolateyService
		{
		    private const string ValidVersionString = "Version: '0.9.8.20'\nInstall Directory: 'C:\\Chocolatey'";

			public event ChocolateyService.OutputDelegate OutputChanged;
			public event ChocolateyService.RunFinishedDelegate RunFinished;

		    private string ExpectedOutputFromHelp { get; set; }

			public FakeChocolateyService(string expectedOutputFromHelp = ValidVersionString)
			{
				ExpectedOutputFromHelp = expectedOutputFromHelp;
			}

			public void LatestVersion()
			{
				throw new NotImplementedException();
			}

			public void Help()
			{
				if (OutputChanged != null)
					OutputChanged(ExpectedOutputFromHelp);
			}
		}
	}
}
