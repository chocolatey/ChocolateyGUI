using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using System;
using System.Linq;
using System.Threading;

namespace ChocolateyGui.UITests.Screens
{
    public class MainScreen : ChocolateyGuiBaseScreen
    {
        public MainScreen(FrameworkAutomationElementBase frameworkAutomationElement)
            : base(frameworkAutomationElement)
        {
        }

        public AboutScreen OpenAndGetAboutScreen()
        {
            var aboutButton = FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.SHOW_ABOUT_BUTTON)).AsButton();
            aboutButton.Click();

            // Do a retry to wait for the window
            return Retry.Find(() => FindFirstChild(cf => cf.ByControlType(ControlType.Window)),
                new RetrySettings
                {
                    Timeout = TimeSpan.FromSeconds(5),
                    IgnoreException = true,
                    ThrowOnTimeout = true,
                    TimeoutMessage = "Failed to find about screen"
                })
            .As<AboutScreen>();
        }

        public SettingsScreen OpenAndGetSettingsScreen()
        {
            var settingsButton = FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.SHOW_SETTINGS_BUTTON)).AsButton();
            settingsButton.Click();

            // Do a retry to wait for the window
            return Retry.Find(() => FindFirstChild(cf => cf.ByControlType(ControlType.Window)),
                new RetrySettings
                {
                    Timeout = TimeSpan.FromSeconds(5),
                    IgnoreException = true,
                    ThrowOnTimeout = true,
                    TimeoutMessage = "Failed to find settings screen"
                })
            .As<SettingsScreen>();
        }

        public RemoteSourceScreen OpenAndGetRemoteSourceScreen(string sourceName = "chocolatey")
        {
            var sourcesListView = FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.SOURCES_LIST_VIEW));
            
            if (sourcesListView == null)
            {
                throw new ApplicationException("Sources list not found.");
            }

            var chocolateyRemoteSourceListItem = FindItemByTextBlockName(sourcesListView, sourceName);

            chocolateyRemoteSourceListItem.AsListBoxItem().Click();

            WaitForDialog();

            return Retry.Find(() => FindFirstChild(cf => cf.ByControlType(ControlType.Window)),
                new RetrySettings
                {
                    Timeout = TimeSpan.FromSeconds(5),
                    IgnoreException = true,
                    ThrowOnTimeout = true,
                    TimeoutMessage = "Failed to find remote source screen"
                }).As<RemoteSourceScreen>();
        }

        public void ReturnToSourcesList()
        {
            // Under OncePerFixture the application instance is shared across the fixture's tests, so a previous
            // test may have drilled into a package-details view that hides the sources list. Click Back until the
            // sources list is visible again; on a freshly launched application it is already there and nothing is
            // clicked.
            Retry.WhileNull(
                () =>
                {
                    var sourcesListView = FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.SOURCES_LIST_VIEW));
                    if (sourcesListView != null)
                    {
                        return sourcesListView;
                    }

                    var backButton = FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.BACK_BUTTON));
                    if (backButton != null)
                    {
                        backButton.AsButton().Click();
                    }

                    return null;
                },
                timeout: TimeSpan.FromSeconds(10),
                interval: TimeSpan.FromMilliseconds(500),
                throwOnTimeout: true,
                ignoreException: true,
                timeoutMessage: "Could not return to the sources list.");
        }

        public void WaitForDialog()
        {
            // The dialog has a fade in and out, sometimes this messes with the detection of the dialog.
            // As such, we sleep for half a second on each side of the detection.
            Thread.Sleep(500);

            Retry.WhileNotNull(() => FindFirstChild(cf => cf.ByAutomationId(AutomationIds.DIALOG)),
                timeout: TimeSpan.FromSeconds(120),
                interval: TimeSpan.FromMilliseconds(100),
                throwOnTimeout: true);

            Thread.Sleep(500);
        }
    }
}