using Chocolatey.Explorer.Services.ChocolateyService;
using Chocolatey.Explorer.Services.SettingsService;
using NUnit.Framework;
using System;
using System.Linq;
using Chocolatey.Explorer.Services;
using Rhino.Mocks;
using Chocolatey.Explorer.Services.FileStorageService;
using System.IO;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Test.Services
{
	[TestFixture]
	public class TestChocolateyLibDirHelper
	{
	    private ISettingsService _settingsService;

        [SetUp]
        public void Setup()
        {
            _settingsService = MockRepository.GenerateMock<ISettingsService>();
            _settingsService.Stub(ssS => ssS.ChocolateyLibDirectory).Return("");
        }

		[Test]
		[ExpectedException()]
		public void IfReloadFromDirWithInvalidDirectoryThenThrowsException()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Throw(new DirectoryNotFoundException());
            var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), fileStorageService, _settingsService);

			var result = helper.ReloadFromDir();

			// expects some sort of exception to match current behavior - test should be latered if this behavior is not actually expected :)
		}

		[Test]
		[ExpectedException()]
		public void IfReloadFromDirWithInvalidEnvironmentVariableThenThrowsException()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Throw(new Exception("Posing as HREsult-based exception that environment would throw, per the way the logic is currently written"));
            var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), fileStorageService, _settingsService);

			var result = helper.ReloadFromDir();

			// expects some sort of exception to match current behavior - test should be latered if this behavior is not actually expected :)
		}

		[Test]
		public void IfReloadFromDirWithEmptyDirectoryThenReturnsListWithChocolatelyOnly()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
            var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), fileStorageService, _settingsService);

			var result = helper.ReloadFromDir();

			Assert.AreEqual("chocolatey", result.Single().Name);
		}

		[Test]
		[ExpectedException(typeof(ChocolateyVersionUnknownException))]
		public void IfReloadFromDirAndHelpTextIsUnrecognizedThenThrowsChocoVersionUnknownException()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
			var chocolatelyService = new FakeChocolateyService("not a valid chocolatey version string");
            var helper = new ChocolateyLibDirHelper(chocolatelyService, fileStorageService, _settingsService);

			var result = helper.ReloadFromDir();

			// expect the version exception
		}

		[Test]
		public void IfReloadFromDirAndHelpTextIsCorrectPatternThenChocolateyPackageContainsProperVersion()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

			var result = helper.ReloadFromDir();

			Assert.AreEqual("0.9.8.20", result.Single().InstalledVersion);
		}

        [Test]
        public void IfFindsVersionFromDirectory()
        {
            var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("0.1.5", result[0].InstalledVersion);
        }

        [Test]
        public void IfFindsNameFromDirectory()
        {
            var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("ChocolateyGUI", result[0].Name);
        }

        [Test]
        public void IfFindsVersionFromDirectoryWithPre()
        {
            var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5-pre"});
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("0.1.5-pre", result[0].InstalledVersion);
        }

        [Test]
        public void IfFindsNameFromDirectoryWithPre()
        {
            var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5-pre" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

            var result = helper.ReloadFromDir();

            Assert.AreEqual("ChocolateyGUI", result[0].Name);
        }

        [Test]
        public void IfGetHighestInstalledVersionFindsHighestVersionWhenAllNumbersAreSingleDigits()
        {
            var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.5", "ChocolateyGUI.0.1.6" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

            var result = helper.GetHighestInstalledVersion("ChocolateyGUI", true);

            Assert.AreEqual("0.1.6", result);
        }

        [Test]
        public void IfGetHighestInstalledVersionFindsHighestVersionWhenLastNumberIsDoubledigit()
        {
            var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.1.15", "ChocolateyGUI.0.1.6" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

            var result = helper.GetHighestInstalledVersion("ChocolateyGUI", true);

            Assert.AreEqual("0.1.15", result);
        }

        [Test]
        public void IfGetHighestInstalledVersionFindsHighestVersionWhenSecondNumberIsDoubleDigit()
        {
            var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
            fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { "ChocolateyGUI.0.11.5", "ChocolateyGUI.0.1.6" });
            var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), fileStorageService, _settingsService);

            var result = helper.GetHighestInstalledVersion("ChocolateyGUI", true);

            Assert.AreEqual("0.11.5", result);
        }

		[TestCase("BasicOnePart.1",							"BasicOnePart",				"1")]
		[TestCase("BasicTwoPart.1.1",						"BasicTwoPart",				"1.1")]
		[TestCase("BasicThreePart.1.1.1",					"BasicThreePart",			"1.1.1")]
		[TestCase("BasicFourPart.1.1.1.1",					"BasicFourPart",			"1.1.1.1")]
		[TestCase("MixedDigitLengths.1.10.981.1234",		"MixedDigitLengths",		"1.10.981.1234")]
		[TestCase("DatePackageFixNotation.1.1.1.20130215",	"DatePackageFixNotation",	"1.1.1.20130215")]
		[TestCase("SemVerPreRelease.1.1.1.1-pre",			"SemVerPreRelease",			"1.1.1.1-pre")]
		[TestCase("SemVerCustomPreRelease.1.1.1.1-stuff",	"SemVerCustomPreRelease",	"1.1.1.1-stuff")]
		[TestCase("OnePartNameWith.Decimal.1.1.1.1",		"OnePartNameWith.Decimal",	"1.1.1.1")]
		[TestCase("TwoPartNameWith.Decimal.1.1.1.1",		"TwoPartNameWith.Decimal",	"1.1.1.1")]
		[TestCase("ThreePartNameWith.Decimal.1.1.1.1",		"ThreePartNameWith.Decimal","1.1.1.1")]
		[TestCase("FourPartNameWith.Decimal.1.1.1.1",		"FourPartNameWith.Decimal",	"1.1.1.1")]
		[TestCase("FourPartPreNameWith.Decimal.1.1.1.1-pre", "FourPartPreNameWith.Decimal", "1.1.1.1-pre")]
		[TestCase("OnePartNumericEnd2.1",					"OnePartNumericEnd2",		"1")]
		[TestCase("TwoPartNumericEnd2.1.1",					"TwoPartNumericEnd2",		"1.1")]
		[TestCase("ThreePartNumericEnd2.1.1.1",				"ThreePartNumericEnd2",		"1.1.1")]
		[TestCase("FourPartNumericEnd2.1.1.1.1",			"FourPartNumericEnd2",		"1.1.1.1")]
		[TestCase("FourPartPreNumericEnd2.1.1.1.1-pre",		"FourPartPreNumericEnd2",	"1.1.1.1-pre")]
		public void IfGetPackageFromDirectoryNameReceivesNameThatIsValidPerRecomendations(string filename, string expectedPackageName, string expectedVersion)
		{
			var expectedPackage = new Package() { 
				Name = expectedPackageName, 
				InstalledVersion = expectedVersion 
			};
			var helper = new ChocolateyLibDirHelper(new FakeChocolateyService(), MockRepository.GenerateMock<IFileStorageService>(), _settingsService);

			var actualPackage = helper.GetPackageFromDirectoryName(filename);

			Assert.AreEqual(expectedPackage.Name, actualPackage.Name);
			Assert.AreEqual(expectedPackage.InstalledVersion, actualPackage.InstalledVersion);
		}

		private class FakeChocolateyService : IChocolateyService
		{
			public const string ValidVersionString = "Version: '0.9.8.20'\nInstall Directory: 'C:\\Chocolatey'";

			public event ChocolateyService.OutputDelegate OutputChanged;
			public event ChocolateyService.RunFinishedDelegate RunFinished;
			
			public string ExpectedOutputFromHelp { get; set; }

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
