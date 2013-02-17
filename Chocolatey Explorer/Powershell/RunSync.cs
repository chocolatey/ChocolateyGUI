using System;
using System.Management.Automation;
using System.Text;

namespace Chocolatey.Explorer.Powershell
{
    public class RunSync:IRunSync
    {
        public void Run(String command)
        {
            this.Log().Info("Running command: " + command);
            var result = new StringBuilder();
            var results = PowerShell.Create()
                .AddScript(command)
                .AddCommand("out-String")
                .Invoke<String>();
            if (results != null && results.Count > 0 && (results.Count == 1 && !String.IsNullOrWhiteSpace(results[0])))
            {
                foreach (var line in results)
                {
                    result.AppendLine(line);
                }
                if(OutputChanged!= null) OutputChanged(result.ToString());
            }
            else
            {
                if (OutputChanged != null) OutputChanged("No output");
            }
            if (RunFinished != null) RunFinished();
       }
        
        public event ResultsHandler OutputChanged;

        public event EmptyHandler RunFinished;
    }
}