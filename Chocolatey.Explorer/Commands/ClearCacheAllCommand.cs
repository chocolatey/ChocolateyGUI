using Chocolatey.Explorer.CommandPattern;

namespace Chocolatey.Explorer.Commands
{
    /// <summary>
    /// Clears the cache by basically calling the following commands. 
    /// <see cref="ClearCacheAvailablePackagesCommand"/>
    /// <see cref="ClearCacheInstalledPackagesCommand"/>
    /// <see cref="ClearCachePackageVersionCommand"/>
    /// </summary>
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