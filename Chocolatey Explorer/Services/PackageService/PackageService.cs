using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Commands;
using Chocolatey.Explorer.Powershell;
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
            _powershellAsync.OutputChanged += OnLineChanged;
            _powershellAsync.RunFinished += OnRunFinished;
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

        private void OnRunFinished()
        {
            this.Log().Debug("Run finished");
            InvalidateCache();
            var handler = RunFinshed;
            if (handler != null) handler();
        }

        private void OnRunStarted(string message)
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler(message);
        }

        private void OnLineChanged(string line)
        {
            this.Log().Debug("Output changed: {0} ", line);
            var handler = LineChanged;
            if (handler != null) handler(line);
        }

        private void InvalidateCache()
        {
            this.Log().Debug("Invalidate cache");
            _commandExecuter.Execute<ClearCacheAllCommand>();
        }

    }
}