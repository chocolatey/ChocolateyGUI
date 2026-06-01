using ChocolateyGui.TestUtilities;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;
using System;

namespace ChocolateyGui.UITests
{
    public class ChocolateyGuiTestBase : FlaUITestBase
    {
        protected override AutomationBase GetAutomation()
        {
            var automation = new UIA3Automation();
            return automation;
        }

        protected override Application StartApplication()
        {
            var testDirectory = TestContext.CurrentContext.TestDirectory.Replace(".UITests", "");
            var application = Application.Launch(System.IO.Path.Combine(testDirectory, "ChocolateyGUI.exe"));

            var window = Retry.Find(
                () => application.GetMainWindow(Automation).FindFirstChild(cf => cf.ByAutomationId("PART_TitleBar")),
                new RetrySettings
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    Interval = TimeSpan.FromMilliseconds(500),
                    IgnoreException = true,
                    ThrowOnTimeout = true,
                    TimeoutMessage = "Could not start the application."
                });

            window.WaitUntilClickable();
            return application;
        }
    }
}
