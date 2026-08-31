using ChocolateyGui.UITests.Screens;
using ChocolateyGui.UITests.Support.Feed;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.TestUtilities;
using NUnit.Framework;
using System.Collections.Generic;

namespace ChocolateyGui.UITests
{

    [TestFixture]
    public class ChocolateyGuiTests : WireMockRemoteSourceTestBase
    {
        protected override ApplicationStartMode ApplicationStartMode => ApplicationStartMode.OncePerFixture;

        // RemoteSourceScreenTest drills into the hermes source; the About/Settings tests ignore it. One WireMock
        // feed (the shared hermes dataset, which includes absolute-extracted-path) serves the whole fixture, so
        // the tests no longer depend on an externally registered hermes source.
        protected override IEnumerable<MockSourceDefinition> Sources => new[]
        {
            new MockSourceDefinition("hermes", MockFeeds.Hermes()),
        };

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