using ChocolateyGui.TestUtilities;
using ChocolateyGui.UITests.Screens;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using NUnit.Framework;

namespace ChocolateyGui.UITests
{

    [TestFixture]
    public class ChocolateyGuiTests : ChocolateyGuiTestBase
    {
        protected new ApplicationStartMode ApplicationStartMode => ApplicationStartMode.OncePerFixture;        

        [Test]
        public void AboutScreenTest()
        {
            var mainScreen = Application.GetMainWindow(Automation).As<MainScreen>();

            var aboutScreen = mainScreen.OpenAndGetAboutScreen();
            aboutScreen.BackButton.Invoke();
        }

        [Test]
        public void SettingsScreenTest()
        {
            var mainScreen = Application.GetMainWindow(Automation).As<MainScreen>();

            var settingsScreen = mainScreen.OpenAndGetSettingsScreen();
            settingsScreen.BackButton.Invoke();
        }

        [Test]
        public void RemoteSourceScreenTest()
        {
            var mainScreen = Application.GetMainWindow(Automation).As<MainScreen>();

            var remoteSourceScreen = mainScreen.OpenAndGetRemoteSourceScreen("hermes");

            var packageDetailsScreen = remoteSourceScreen.GetPackageDetailsScreen("absolute-extracted-path");

            packageDetailsScreen.BackButton.Invoke();
        }
    }
}