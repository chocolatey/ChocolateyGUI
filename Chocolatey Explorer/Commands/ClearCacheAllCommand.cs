using Chocolatey.Explorer.CommandPattern;

namespace Chocolatey.Explorer.Commands
{
    public class ClearCacheAllCommand : BaseCommand
    {
        public ICommandExecuter CommandExecuter { get; set; }

        public override void Execute()
        {
            this.Log().Info("Clearing cache of all services.");
            CommandExecuter.Execute<ClearCacheAvailablePackagesCommand>();
            CommandExecuter.Execute<ClearCacheInstalledPackagesCommand>();
            CommandExecuter.Execute<ClearCachePackageVersionCommand>();
        }
    }
}