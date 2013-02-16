using Chocolatey.Explorer.Powershell;

namespace Chocolatey.Explorer.Services
{
    public class ChocolateyService : IChocolateyService
    {
        private readonly IRun _powershell;
        private readonly ISourceService _sourceService;

        public delegate void OutputDelegate(string output);
        public delegate void RunFinishedDelegate();

        public event OutputDelegate OutputChanged;
        public event RunFinishedDelegate RunFinished;

        public ChocolateyService()
            : this(new RunSync(), new SourceService())
        {
        }

        public ChocolateyService(IRunSync powerShell, ISourceService sourceService)
        {
            _powershell = powerShell;
            _sourceService = sourceService;
            _powershell.OutputChanged += OutPutChangedHandler;
            _powershell.RunFinished += RunFinishedHandler;
        }

        public void LatestVersion()
        {
            this.Log().Info("Getting latest version.");
            _powershell.Run("cver" + " -source " + _sourceService.Source);
        }

        public void Help()
        {
            this.Log().Info("Getting help");
            _powershell.Run("chocolatey /?");
        }

        private void OnRunFinished()
        {
            RunFinishedDelegate handler = RunFinished;
            if (handler != null) handler();
        }

        private void InvokeOutputChanged(string output)
        {
            OutputDelegate handler = OutputChanged;
            if (handler != null) handler(output);
        }

        private void OutPutChangedHandler(string output)
        {
            InvokeOutputChanged(output);
        }

        private void RunFinishedHandler()
        {
            OnRunFinished();
        }

    }
}