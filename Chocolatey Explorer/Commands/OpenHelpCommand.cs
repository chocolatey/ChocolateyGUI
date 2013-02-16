using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.View;

namespace Chocolatey.Explorer.Commands
{
    public class OpenHelpCommand:ICommand
    {
        public void Execute()
        {
            var help = new Help();
            help.ShowDialog();
        }
    }
}