using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chocolatey_Explorer.Powershell
{
    public class RunSync:IRun
    {
        public void Run(String command)
        {
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