using System;
using System.Management.Automation;
using System.Text;

namespace Chocolatey.Explorer.Powershell
{
    public class RunSync:IRunSync
    {
        public void Run(String command)
        {
            this.Log().Info("Running command: {0}", command);
            var result = new StringBuilder();
            var results = PowerShell.Create()
                .AddScript(command)
                .AddCommand("out-String")
                .Invoke<String>();
            if (results != null && results.Count > 0 && (results.Count == 1 && !String.IsNullOrWhiteSpace(results[0])))
            {
                foreach (var line in results)
                {
                    this.Log().Debug("Line: {0}", line);
                    result.AppendLine(line);
                }
                if (OutputChanged != null)
                {
                    this.Log().Debug("Result: {0}", result.ToString());
                    OutputChanged(result.ToString());
                }
            }
            else
            {
                if (OutputChanged != null)
                {
                    this.Log().Debug("No output");
                    OutputChanged("No output");
                }
            }
            if (RunFinished == null) return;
            this.Log().Debug("Run finished");
            RunFinished();
        }
        
        public event ResultsHandler OutputChanged;

        public event EmptyHandler RunFinished;
    }
}