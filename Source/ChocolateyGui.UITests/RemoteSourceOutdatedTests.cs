using ChocolateyGui.UITests.Screens;
using ChocolateyGui.UITests.Support;
using ChocolateyGui.UITests.Support.Feed;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.TestUtilities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ChocolateyGui.UITests
{
    // Regression coverage for issue #1109 ("Unable to upgrade beta packages through Chocolatey GUI"): with the
    // prerelease toggle on, an installed package that has a newer prerelease on a remote source must be recognised
    // as outdated, and the package details must surface the available (beta) version. Before the 3.1.0 fix the
    // RemoteSourceView/ChocolateyService knew nothing about prereleases, so this never happened. The negative test
    // guards the other direction: with prereleases excluded, a prerelease-only upgrade must NOT be offered.
    [TestFixture]
    public class RemoteSourceOutdatedTests : WireMockRemoteSourceTestBase
    {
        private const string PACKAGE_UNDER_TEST = "mixedpackage";
        private const string INSTALLED_VERSION = "2.0.0";
        private const string LATEST_BETA_VERSION = "2.1.0-beta-1";

        protected override ApplicationStartMode ApplicationStartMode => ApplicationStartMode.OncePerFixture;

        protected override IEnumerable<MockSourceDefinition> Sources => new[]
        {
            new MockSourceDefinition("hermes", MockFeeds.Hermes()),
        };

        private MainScreen MainScreen;
        private RemoteSourceScreen RemoteSourceScreen;

        // Seed mixedpackage 2.0.0 as installed BEFORE the application launches, so the remote source can mark it
        // outdated once the prerelease 2.1.0-beta-1 is discovered.
        protected override Application StartApplication()
        {
            IsolatedChocolateyEnvironment.RemoveInstalledPackage(PACKAGE_UNDER_TEST);
            IsolatedChocolateyEnvironment.SeedInstalledPackage(PACKAGE_UNDER_TEST, INSTALLED_VERSION);

            // The GUI caches outdated results per source/prerelease for 60 minutes; clear them so the check runs
            // fresh against the mock feed rather than reusing a previous run's result.
            IsolatedChocolateyEnvironment.ClearOutdatedPackagesCache();
            return base.StartApplication();
        }

        [OneTimeTearDown]
        public void RemoveSeededPackage()
        {
            IsolatedChocolateyEnvironment.RemoveInstalledPackage(PACKAGE_UNDER_TEST);
        }

        [SetUp]
        public void Arrange()
        {
            // OncePerFixture shares one application across the fixture's tests, so return to a known state
            // (sources list, prereleases excluded) before each test.
            MainScreen = Application.GetMainWindow(Automation).As<MainScreen>();
            MainScreen.ReturnToSourcesList();

            RemoteSourceScreen = MainScreen.OpenAndGetRemoteSourceScreen("hermes");
            EnsureUnchecked(AutomationIds.PRERELEASE_CHECK_BOX);
        }

        [Test]
        public void RemoteScreenShowsPrereleaseUpgradeAvailableForInstalledPackage()
        {
            // Enabling prereleases reloads the packages, which runs the outdated check against the seeded install
            // (LoadPackages -> GetOutdatedPackages), so the beta is recognised as an available upgrade.
            MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.PRERELEASE_CHECK_BOX)).Click();
            MainScreen.WaitForDialog();

            OpenPackageDetails();

            var availableVersion = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.AVAILABLE_VERSION_TEXT));

            Assert.That(
                availableVersion,
                Is.Not.Null,
                "The package details did not show an available (outdated) version - issue #1109 regression: the installed package was not recognised as having a prerelease upgrade.");
            Assert.That(availableVersion.Name, Is.EqualTo(LATEST_BETA_VERSION));
        }

        [Test]
        public void RemoteScreenDoesNotShowUpgradeWhenPrereleasesAreExcluded()
        {
            // Prereleases are excluded (Arrange), and the only newer version of the installed package is a
            // prerelease, so it must NOT be flagged outdated and no available version should be shown.
            OpenPackageDetails();

            var availableVersion = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.AVAILABLE_VERSION_TEXT));

            Assert.That(
                availableVersion,
                Is.Null,
                "The installed package was flagged as having an available upgrade even though the only newer version is a prerelease and prereleases are excluded.");
        }

        private void OpenPackageDetails()
        {
            RemoteSourceScreen.FocusAndClearSearch();
            Keyboard.Type(PACKAGE_UNDER_TEST);
            Thread.Sleep(500); // Sometimes this fails without a minor sleep when debugging...
            Keyboard.Press(VirtualKeyShort.ENTER);
            MainScreen.WaitForDialog();

            var packageList = RemoteSourceScreen.GetPackageList();
            Assert.That(packageList, Has.Length.EqualTo(1));

            var targetPackage = packageList.FirstOrDefault();
            targetPackage.Click();
            Thread.Sleep(100); // Without waiting, sometimes this fails...
            targetPackage.DoubleClick();
        }

        // A prior test in the shared fixture may have left a checkbox set; clear it (and let the resulting reload
        // settle) so every test starts from the same state a fresh application would give.
        private void EnsureUnchecked(string automationId)
        {
            var checkBox = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(automationId)).AsCheckBox();
            if (checkBox.IsChecked == true)
            {
                checkBox.Click();
                MainScreen.WaitForDialog();
            }
        }
    }
}
