using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.View.Forms;

namespace Chocolatey.Explorer.Commands
{
    /// <summary>
    /// Opens the <see cref="ILogs"/> form by calling the DoShow method on it.
    /// </summary>
    public class OpenLogsCommand : BaseCommand
    {
        public ILogs Logs { get; set; }

        public override void Execute()
        {
            this.Log().Info("Opened Logs form.");
            Logs.DoShow();
        }
    }
}