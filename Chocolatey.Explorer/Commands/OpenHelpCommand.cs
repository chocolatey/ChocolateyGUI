using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.View.Forms;

namespace Chocolatey.Explorer.Commands
{
    /// <summary>
    /// Opens the <see cref="IHelp"/> form by calling DoShow on it.
    /// </summary>
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