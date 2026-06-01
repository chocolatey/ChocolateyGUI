using System.Linq;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace ChocolateyGui.UITests.Screens
{
    public class ChocolateyGuiBaseScreen : Window
    {
        public ChocolateyGuiBaseScreen(FrameworkAutomationElementBase frameworkAutomationElement)
            : base(frameworkAutomationElement)
        {
        }

        protected AutomationElement FindItemByTextBlockName(AutomationElement list, string wantedName)
        {
            // scope this to whatever contains the items: the List/ItemsControl/ListBox element
            var items = list.FindAllDescendants(cf => cf.ByControlType(controlType: ControlType.ListItem));

            return items.FirstOrDefault(item =>
                item.FindFirstChild(cf =>
                    cf.ByControlType(ControlType.Text) // TextBlock maps to ControlType.Text in UIA
                      .And(cf.ByName(wantedName))
                ) != null);
        }
    }
}
