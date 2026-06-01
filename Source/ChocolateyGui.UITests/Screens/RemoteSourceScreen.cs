using System;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.Core.WindowsAPI;

namespace ChocolateyGui.UITests.Screens
{
    public class RemoteSourceScreen : ChocolateyGuiBaseScreen
    {
        public RemoteSourceScreen(FrameworkAutomationElementBase frameworkAutomationElement)
            : base(frameworkAutomationElement)
        {
        }

        public PackageDetailsScreen GetPackageDetailsScreen(string packageTitle)
        {
            var packagesListView = this.Parent.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.PACKAGES_LIST));
            var targetPackageListItem = FindItemByTextBlockName(packagesListView, packageTitle);
            targetPackageListItem.AsListBoxItem().Click();
            targetPackageListItem.AsListBoxItem().DoubleClick();

            // Do a retry to wait for the window
            return Retry.Find(() => this.Parent.FindFirstDescendant(cf => cf.ByControlType(ControlType.Window)),
                new RetrySettings
                {
                    Timeout = TimeSpan.FromSeconds(5),
                    IgnoreException = true,
                    ThrowOnTimeout = true,
                    TimeoutMessage = "Failed to find remote source screen"
                })
            .As<PackageDetailsScreen>();
        }

        public void FocusAndClearSearch()
        {
            var search = Parent.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.SEARCH_TEXT_BOX));
            search.Focus();
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            Keyboard.Type(VirtualKeyShort.DELETE);
        }

        public AutomationElement[] GetPackageList()
        {
            return Parent
                .FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.PACKAGES_LIST))
                .FindAllChildren(cf => cf.ByControlType(ControlType.ListItem));
        }
    }
}