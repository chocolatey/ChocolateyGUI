using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.View.Forms;

namespace Chocolatey.Explorer.Commands
{
    public class OpenSettingsCommand:BaseCommand
    {
        public ISettings Settings { get; set; }

        public override void Execute()
        {
            Settings.DoShowDialog();
        }
    }
}