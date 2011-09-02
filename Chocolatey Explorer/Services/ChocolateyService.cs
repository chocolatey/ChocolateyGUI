using Chocolatey.Explorer.Powershell;

namespace Chocolatey.Explorer.Services
{
    public class ChocolateyService
    {
        private IRun _powershell;
        public delegate void OutputDelegate(string output);
        public delegate void RunFinishedDelegate();

        public event OutputDelegate OutputChanged;
        public event RunFinishedDelegate RunFinished;

        public void OnRunFinished()
        {
            RunFinishedDelegate handler = RunFinished;
            if (handler != null) handler();
        }

        private void InvokeOutputChanged(string output)
        {
            OutputDelegate handler = OutputChanged;
            if (handler != null) handler(output);
        }

        public ChocolateyService()
            : this(new RunSync())
        {
        }

        public ChocolateyService(IRun powerShell)
        {
            _powershell = powerShell;
            _powershell.OutputChanged += OutPutChangedHandler;
            _powershell.RunFinished += RunFinishedHandler;
        }

        private void OutPutChangedHandler(string output)
        {
            InvokeOutputChanged(output);
        }

        private void RunFinishedHandler()
        {
            OnRunFinished();
        }

        public void LatestVersion()
        {
            _powershell.Run("cver");
        }

        public void Help()
        {
            _powershell.Run("chocolatey /?");
        }
    }
}