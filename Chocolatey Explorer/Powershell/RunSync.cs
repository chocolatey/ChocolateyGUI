using System;
using System.Text;

namespace Chocolatey.Explorer.Powershell
{
    public class RunSync:IRunSync
    {
        public void Run(String command)
        {
            this.Log().Info("Running command: " + command);
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