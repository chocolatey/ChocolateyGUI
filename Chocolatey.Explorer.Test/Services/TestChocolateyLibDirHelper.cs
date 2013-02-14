using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocolatey.Explorer.Services;
using Rhino.Mocks;
using Chocolatey.Explorer.Services.FileStorageService;
using System.IO;

namespace Chocolatey.Explorer.Test.Services
{
	[TestFixture]
	public class TestChocolateyLibDirHelper
	{
		[Test]
		[ExpectedException()]
		public void IfReloadFromDirWithInvalidDirectoryThenThrowsException()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Throw(new DirectoryNotFoundException());
			var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), fileStorageService);

			var result = helper.ReloadFromDir();

			// expects some sort of exception to match current behavior - test should be latered if this behavior is not actually expected :)
		}

		[Test]
		[ExpectedException()]
		public void IfReloadFromDirWithInvalidEnvironmentVariableThenThrowsException()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Throw(new Exception("Posing as HREsult-based exception that environment would throw, per the way the logic is currently written"));
			var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), fileStorageService);

			var result = helper.ReloadFromDir();

			// expects some sort of exception to match current behavior - test should be latered if this behavior is not actually expected :)
		}

		[Test]
		public void IfReloadFromDirWithEmptyDirectoryThenReturnsListWithChocolatelyOnly()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
			var helper = new ChocolateyLibDirHelper(MockRepository.GenerateMock<IChocolateyService>(), fileStorageService);

			var result = helper.ReloadFromDir();

			Assert.AreEqual("chocolatey", result.Single().Name);
		}

		[Test]
		[ExpectedException(typeof(ChocolateyVersionUnknownException))]
		public void IfReloadFromDirAndHelpTextIsUnrecognizedThenThrowsChocoVersionUnknownException()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
			var chocolatelyService = new FakeChocolateyService() {
				ExpectedOutputFromHelp = "not a valid chocolatey version string"
			};
			var helper = new ChocolateyLibDirHelper(chocolatelyService, fileStorageService);

			var result = helper.ReloadFromDir();

			// expect the version exception
		}

		[Test]
		public void IfReloadFromDirAndHelpTextIsCorrectPatternThenChocolateyPackageContainsProperVersion()
		{
			var fileStorageService = MockRepository.GenerateMock<IFileStorageService>();
			fileStorageService.Stub(fss => fss.GetDirectories(Arg<string>.Is.Anything)).Return(new string[] { });
			var chocolatelyService = new FakeChocolateyService() {
				ExpectedOutputFromHelp = "Version: '0.9.8.20'\nInstall Directory: 'C:\\Chocolatey'"
			};
			var helper = new ChocolateyLibDirHelper(chocolatelyService, fileStorageService);

			var result = helper.ReloadFromDir();

			Assert.AreEqual("0.9.8.20", result.Single().InstalledVersion);
		}

		private class FakeChocolateyService : IChocolateyService
		{

			public event ChocolateyService.OutputDelegate OutputChanged;
			public event ChocolateyService.RunFinishedDelegate RunFinished;

			public string ExpectedOutputFromHelp { get; set; }

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
