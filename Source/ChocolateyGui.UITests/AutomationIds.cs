using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System.Linq;

namespace ChocolateyGui.UITests
{
    internal static class AutomationIds
    {
        internal const string ALL_VERSIONS_CHECK_BOX = "AllVersionsCheckBox";
        internal const string MATCH_CHECK_BOX = "MatchCheckBox";
        internal const string AVAILABLE_VERSION_TEXT = "AvailableVersion";
        internal const string BACK_BUTTON = "Back";
        internal const string DIALOG = "PART_Dialog";
        internal const string PACKAGES_LIST = "Packages";
        internal const string PRERELEASE_CHECK_BOX = "PrereleaseCheckBox";
        internal const string SEARCH_TEXT_BOX = "SearchTextBox";
        internal const string SHOW_ABOUT_BUTTON = "ShowAbout";
        internal const string SHOW_SETTINGS_BUTTON = "ShowSettings";
        internal const string SOURCES_LIST_VIEW = "SourcesListView";
        internal const string VERSION_TEXT = "Version";

        internal static AutomationElement[] LocateAllByText(this AutomationElement automationElement, string text)
        {
            return automationElement.FindAllChildren(cf => cf.ByControlType(ControlType.Text).And(cf.ByName(text)));
        }

        internal static AutomationElement LocateFirstByText(this AutomationElement automationElement, string text)
        {
            return automationElement.LocateAllByText(text).FirstOrDefault();
        }
    }
}
