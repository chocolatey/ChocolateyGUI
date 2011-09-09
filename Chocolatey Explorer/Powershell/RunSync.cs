using System;
using System.Text;
using log4net;

namespace Chocolatey.Explorer.Powershell
{
    public class RunSync:IRun
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RunSync));

        public void Run(String command)
        {
            log.Info("Running command: " + command);
            var result = new StringBuilder();
            var results = System.Management.Automation.PowerShell.Create().AddScript(command).AddCommand("out-String").Invoke<String>();
            foreach(var line in results)
            {
                result.AppendLine(line);
            }
            OutputChanged(result.ToString());
            RunFinished();
       }
        
        public event ResultsHandler OutputChanged;

        public event EmptyHandler RunFinished;
    }
}