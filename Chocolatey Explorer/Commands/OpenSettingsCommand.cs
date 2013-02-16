using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.View;

namespace Chocolatey.Explorer.Commands
{
    public class OpenSettingsCommand:ICommand
    {
        public void Execute()
        {
            var settings = new Settings();
            settings.ShowDialog();
        }
    }
}