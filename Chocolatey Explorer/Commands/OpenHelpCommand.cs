using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.View.Forms;

namespace Chocolatey.Explorer.Commands
{
    public class OpenHelpCommand:BaseCommand
    {
        public IHelp Help { get; set; }

        public override void Execute()
        {
            this.Log().Info("Opened Help form.");
            Help.DoShow();
        }
    }
}