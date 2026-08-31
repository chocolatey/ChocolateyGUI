using ChocolateyGui.UITests.Screens;
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
    [TestFixture]
    public class BetaPackagesTests : WireMockRemoteSourceTestBase
    {
        private const string PACKAGE_UNDER_TEST = "mixedpackage";
        private const string LATEST_STABLE_VERSION = "2.0.0";
        private const string LATEST_BETA_VERSION = "2.1.0-beta-1";
        protected override ApplicationStartMode ApplicationStartMode => ApplicationStartMode.OncePerFixture;

        // The hermes source is now served by a local WireMock feed instead of an internal NuGet repository,
        // so these tests no longer depend on anything outside the repo. mixedpackage's version topology lives
        // in MockFeeds.Hermes().
        protected override IEnumerable<MockSourceDefinition> Sources => new[]
        {
            new MockSourceDefinition("hermes", MockFeeds.Hermes()),
        };

        private MainScreen MainScreen;
        private RemoteSourceScreen RemoteSourceScreen;

        [SetUp]
        public void Arrange()
        {
            MainScreen = Application.GetMainWindow(Automation).As<MainScreen>();

            // OncePerFixture shares one application across the fixture's tests, so reset the navigation and the
            // prerelease/all-versions toggles a previous test may have left set before arranging this one.
            MainScreen.ReturnToSourcesList();

            RemoteSourceScreen = MainScreen.OpenAndGetRemoteSourceScreen("hermes");

            EnsureUnchecked(AutomationIds.PRERELEASE_CHECK_BOX);
            EnsureUnchecked(AutomationIds.ALL_VERSIONS_CHECK_BOX);

            RemoteSourceScreen.FocusAndClearSearch();

            Keyboard.Type(PACKAGE_UNDER_TEST);
            Thread.Sleep(500); // Sometimes this fails without a minor sleep when debugging...
            Keyboard.Press(VirtualKeyShort.ENTER);
            MainScreen.WaitForDialog();
        }

        // A prior test in the shared fixture may have left a filter checkbox set; clear it (and let the resulting
        // reload settle) so every test starts from the default stable/latest view a fresh application would show.
        private void EnsureUnchecked(string automationId)
        {
            var checkBox = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(automationId)).AsCheckBox();
            if (checkBox.IsChecked == true)
            {
                checkBox.Click();
                MainScreen.WaitForDialog();
            }
        }

        [Test]
        public void RemoteScreenFindsStableVersionOfDesiredPackage()
        {
            var packageList = RemoteSourceScreen.GetPackageList();

            Assert.That(packageList, Has.Length.EqualTo(1));
            
            var targetPackage = packageList.FirstOrDefault();
            targetPackage.Click();
            Thread.Sleep(100); // Without waiting, sometimes this fails...
            targetPackage.DoubleClick();
            var versionItem = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.VERSION_TEXT));

            Assert.That(versionItem.Name, Is.EqualTo(LATEST_STABLE_VERSION));
        }

        [Test]
        public void RemoteScreenFindsPrereleaseVersionOfDesiredPackage()
        {
            MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.PRERELEASE_CHECK_BOX)).Click();
            MainScreen.WaitForDialog();

            var packageList = RemoteSourceScreen.GetPackageList();

            Assert.That(packageList, Has.Length.EqualTo(1));

            var targetPackage = packageList.FirstOrDefault();
            targetPackage.Click();
            Thread.Sleep(100);
            targetPackage.DoubleClick();
            var versionItem = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.VERSION_TEXT));

            Assert.That(versionItem.Name, Is.EqualTo(LATEST_BETA_VERSION));
        }

        [Test]
        public void RemoteScreenFindsAllStableVersionsOfDesiredPackage()
        {
            MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.ALL_VERSIONS_CHECK_BOX)).Click();
            MainScreen.WaitForDialog();

            var packageList = RemoteSourceScreen.GetPackageList();

            Assert.That(packageList, Has.Length.EqualTo(3));

            var targetPackage = packageList.FirstOrDefault();
            targetPackage.Click();
            Thread.Sleep(100);
            targetPackage.DoubleClick();
            var versionItem = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.VERSION_TEXT));

            Assert.That(versionItem.Name, Is.EqualTo(LATEST_STABLE_VERSION));
        }

        [Test]
        public void RemoteScreenFindsAllVersionsOfDesiredPackage()
        {
            MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.PRERELEASE_CHECK_BOX)).Click();
            MainScreen.WaitForDialog();

            MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.ALL_VERSIONS_CHECK_BOX)).Click();
            MainScreen.WaitForDialog();

            var packageList = RemoteSourceScreen.GetPackageList();

            Assert.That(packageList, Has.Length.EqualTo(10));

            var targetPackage = packageList.FirstOrDefault();
            targetPackage.Click();
            Thread.Sleep(100);
            targetPackage.DoubleClick();
            var versionItem = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.VERSION_TEXT));

            Assert.That(versionItem.Name, Is.EqualTo(LATEST_BETA_VERSION));
        }
    }
}