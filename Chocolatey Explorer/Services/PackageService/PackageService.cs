using Chocolatey.Explorer.Powershell;
using log4net;

namespace Chocolatey.Explorer.Services
{
    public class PackageService : IPackageService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PackageService));

        private readonly IRun _powershellAsync;
        private readonly ISourceService _sourceService;

        public delegate void LineDelegate(string line);
        public event LineDelegate LineChanged;
        public delegate void FinishedDelegate();
        public event FinishedDelegate RunFinshed;

        public PackageService(): this(new RunAsync(),new SourceService())
        {
        }

        public PackageService(IRun powershell, ISourceService sourceService)
        {
			_powershellAsync = powershell;
            _sourceService = sourceService;
            _powershellAsync.OutputChanged += OutputChanged;
            _powershellAsync.RunFinished += RunFinished;
        }

        public void InstallPackage(string package)
        {
            log.Info("Installing package: " + package);
            _powershellAsync.Run("cinst " + package + " -source " + _sourceService.Source);
        }

        public void UninstallPackage(string package)
        {
            log.Info("Uninstalling package: " + package);
            _powershellAsync.Run("cuninst " + package);
        }

        public void UpdatePackage(string package)
        {
            log.Info("Updating package: " + package);
            _powershellAsync.Run("cup " + package + " -source " + _sourceService.Source);
        }

        private void OutputChanged(string line)
        {
            OnLineChanged(line);
        }

        private void RunFinished()
        {
            OnRunFinshed();
        }

        private void OnRunFinshed()
        {
            FinishedDelegate handler = RunFinshed;
            if (handler != null) handler();
        }

        private void OnLineChanged(string line)
        {
            LineDelegate handler = LineChanged;
            if (handler != null) handler(line);
        }

    }
}