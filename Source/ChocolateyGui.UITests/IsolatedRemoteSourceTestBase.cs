// <copyright file="IsolatedRemoteSourceTestBase.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Threading;
using ChocolateyGui.UITests.Screens;
using ChocolateyGui.UITests.Support;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using NUnit.Framework;

namespace ChocolateyGui.UITests
{
    /// <summary>
    ///     Base class for UITests that run against a mocked community.chocolatey.org feed (served by Dev Proxy
    ///     from <c>Fixtures/community.chocolatey.org</c>) rather than a live or internal source. It manages the
    ///     Dev Proxy lifecycle and establishes the desired "installed" state for the package under test BEFORE
    ///     the application launches (so the state is deterministic and idempotent from run to run, and nothing
    ///     holds a file lock while we change it). Subclasses override <see cref="PrepareInstalledState"/> to
    ///     choose whether the package is installed.
    /// </summary>
    public abstract class IsolatedRemoteSourceTestBase : ChocolateyGuiTestBase
    {
        protected const string SourceName = "chocolatey";
        protected const string PackageId = "mock-chocolateygui";
        protected const string SearchTerm = "mock-chocolateygui";

        // Dev Proxy is intentionally fixture-scoped (started once, before the application launches, and
        // disposed in OneTimeTearDown) rather than per-test, so the per-test TearDown disposal the analyzer
        // expects does not apply here.
#pragma warning disable NUnit1032
        private DevProxyController _devProxy;
#pragma warning restore NUnit1032

        protected MainScreen MainScreen { get; private set; }

        protected RemoteSourceScreen RemoteSourceScreen { get; private set; }

        protected override Application StartApplication()
        {
            if (_devProxy == null)
            {
                _devProxy = new DevProxyController(IsolatedChocolateyEnvironment.MockResponsesDirectory);

                if (!_devProxy.IsAvailable)
                {
                    Assert.Ignore(
                        "Dev Proxy is not installed. Install it from https://aka.ms/devproxy (e.g. 'winget install DevProxy.DevProxy') or set DEVPROXY_PATH to run the isolated UITests.");
                }

                _devProxy.Start();
            }

            PrepareInstalledState();
            IsolatedChocolateyEnvironment.ClearHttpCache();

            try
            {
                return base.StartApplication();
            }
            catch
            {
                _devProxy?.Stop();
                _devProxy = null;
                throw;
            }
        }

        [OneTimeTearDown]
        public void StopDevProxy()
        {
            _devProxy?.Dispose();
            _devProxy = null;
        }

        /// <summary>
        ///     Establishes the installed state of the package under test before the application launches.
        ///     The default ensures it is NOT installed; the installed-package fixture overrides this to seed it.
        /// </summary>
        protected virtual void PrepareInstalledState()
        {
            IsolatedChocolateyEnvironment.RemoveInstalledPackage(PackageId);
        }

        /// <summary>
        ///     Opens the mocked remote source and returns to a clean baseline: no filters, empty search (which
        ///     loads the default popularity list).
        /// </summary>
        protected void OpenRemoteSourceAtBaseline()
        {
            MainScreen = Application.GetMainWindow(Automation).As<MainScreen>();
            MainScreen.ReturnToSourcesList();
            RemoteSourceScreen = MainScreen.OpenAndGetRemoteSourceScreen(SourceName);

            SetCheckBox(AutomationIds.ALL_VERSIONS_CHECK_BOX, false);
            SetCheckBox(AutomationIds.PRERELEASE_CHECK_BOX, false);
            SetCheckBox(AutomationIds.MATCH_CHECK_BOX, false);
            ClearSearchAndReload();
        }

        protected void SetCheckBox(string automationId, bool isChecked)
        {
            var checkBox = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(automationId)).AsCheckBox();

            if (checkBox.IsChecked != isChecked)
            {
                checkBox.Click();
                MainScreen.WaitForDialog();
            }
        }

        protected void Search(string term)
        {
            RemoteSourceScreen.FocusAndClearSearch();
            Keyboard.Type(term);
            Thread.Sleep(500); // Sometimes this fails without a minor sleep when debugging...
            Keyboard.Press(VirtualKeyShort.ENTER);
            MainScreen.WaitForDialog();
        }

        protected void ClearSearchAndReload()
        {
            RemoteSourceScreen.FocusAndClearSearch();
            Keyboard.Press(VirtualKeyShort.ENTER);
            MainScreen.WaitForDialog();
        }
    }
}
