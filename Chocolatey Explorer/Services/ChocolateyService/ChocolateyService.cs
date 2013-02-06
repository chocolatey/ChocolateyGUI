using Chocolatey.Explorer.Powershell;
using log4net;

namespace Chocolatey.Explorer.Services
{
    public class ChocolateyService : IChocolateyService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ChocolateyService));

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

        public ChocolateyService(IRun powerShell, ISourceService sourceService)
        {
            _powershell = new RunSync();
            _sourceService = sourceService;
            _powershell.OutputChanged += OutPutChangedHandler;
            _powershell.RunFinished += RunFinishedHandler;
        }

        public void LatestVersion()
        {
            log.Info("Getting latest version.");
            _powershell.Run("cver" + " -source " + _sourceService.Source);
        }

        public void Help()
        {
            log.Info("Getting help");
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