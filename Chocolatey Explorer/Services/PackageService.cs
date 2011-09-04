using Chocolatey.Explorer.Powershell;

namespace Chocolatey.Explorer.Services
{
    public class PackageService : IPackageService
    {
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
            _powershellAsync.Run("cup " + package + " -source " + _sourceService.Source);
        }

        public void UpdatePackage(string package)
        {
            _powershellAsync.Run("cinst " + package + " -source " + _sourceService.Source);
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