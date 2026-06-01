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