using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services.PackageVersionService;
using Chocolatey.Explorer.Services.PackagesService;
using Chocolatey.Explorer.Services.SourceService;

namespace Chocolatey.Explorer.Services.PackageService
{
    public class PackageService : IPackageService
    {
        private readonly IRun _powershellAsync;
        private readonly ISourceService _sourceService;
        private readonly ICommandExecuter _commandExecuter;

        public event Delegates.LineDelegate LineChanged;
        public event Delegates.FinishedPackageDelegate RunFinshed;
        public event Delegates.StartedDelegate RunStarted;

        public PackageService(IRunAsync powershell, ISourceService sourceService, ICommandExecuter commandExecuter)
        {
			_powershellAsync = powershell;
            _sourceService = sourceService;
            _commandExecuter = commandExecuter;
            _powershellAsync.OutputChanged += OutputChanged;
            _powershellAsync.RunFinished += RunFinished;
        }

        public void InstallPackage(string package)
        {
            this.Log().Info("Installing package: " + package);
            OnRunStarted("Installing package " + package);
            _powershellAsync.Run("cinst " + package + " -source " + _sourceService.Source);
        }

        public void UninstallPackage(string package)
        {
            this.Log().Info("Uninstalling package: " + package);
            OnRunStarted("Uninstalling package " + package);
            _powershellAsync.Run("cuninst " + package);
        }

        public void UpdatePackage(string package)
        {
            this.Log().Info("Updating package: " + package);
            OnRunStarted("Updating package " + package);
            _powershellAsync.Run("cup " + package + " -source " + _sourceService.Source);
        }

        private void OutputChanged(string line)
        {
            OnLineChanged(line);
        }

        private void RunFinished()
        {
            InvalidateCache();
            OnRunFinshed();
        }

        private void OnRunFinshed()
        {
            var handler = RunFinshed;
            if (handler != null) handler();
        }

        private void OnRunStarted(string message)
        {
            var handler = RunStarted;
            if (handler != null) handler(message);
        }

        private void OnLineChanged(string line)
        {
            var handler = LineChanged;
            if (handler != null) handler(line);
        }

        private void InvalidateCache()
        {
            _commandExecuter.Execute<ClearCacheAllCommand>();
        }

    }
}