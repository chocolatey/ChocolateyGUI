using Chocolatey_Explorer.Powershell;

namespace Chocolatey_Explorer.Services
{
    public class Chocolatey
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
        
        public Chocolatey()
        {
            _powershell = new RunSync();
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

        public Chocolatey(IRun powershell)
        {
            _powershell = powershell;
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