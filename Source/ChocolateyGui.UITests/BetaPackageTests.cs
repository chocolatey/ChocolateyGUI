using ChocolateyGui.TestUtilities;
using ChocolateyGui.UITests.Screens;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace ChocolateyGui.UITests
{
    [TestFixture]
    public class BetaPackagesTests : ChocolateyGuiTestBase
    {
        private const string PACKAGE_UNDER_TEST = "mixedpackage";
        private const string LATEST_STABLE_VERSION = "2.0.0";
        private const string LATEST_BETA_VERSION = "2.1.0-beta-1";
        protected new ApplicationStartMode ApplicationStartMode => ApplicationStartMode.OncePerFixture;

        private MainScreen MainScreen;
        private RemoteSourceScreen RemoteSourceScreen;

        [SetUp]
        public void Arrange()
        {
            MainScreen = Application.GetMainWindow(Automation).As<MainScreen>();

            RemoteSourceScreen = MainScreen.OpenAndGetRemoteSourceScreen("hermes");
            RemoteSourceScreen.FocusAndClearSearch();

            Keyboard.Type(PACKAGE_UNDER_TEST);
            Thread.Sleep(500); // Sometimes this fails without a minor sleep when debugging...
            Keyboard.Press(VirtualKeyShort.ENTER);
            MainScreen.WaitForDialog();
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