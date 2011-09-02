using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Chocolatey.Explorer.Powershell
{
    public class RunAsync : IRun
    {
        private Runspace _RunSpace;
        private Pipeline _PipeLine;
        private PipelineReader<PSObject> _OutPut;

        public event ResultsHandler OutputChanged;

        public event EmptyHandler RunFinished;

        public RunAsync()
        {
            _RunSpace = RunspaceFactory.CreateRunspace();
            _RunSpace.Open();
        }

        public void Run(String command)
        {
            _PipeLine = _RunSpace.CreatePipeline(command);
            _PipeLine.Input.Close();
            _OutPut = _PipeLine.Output;
            _OutPut.DataReady += _Output_DataReady;
            _PipeLine.InvokeAsync();
        }

        public void OnOutputChanged(string version)
        {
            var handler = OutputChanged;
            if (handler != null) handler(version);
        }

        public void OnFinishedRun()
        {
            var handler = RunFinished;
            if (handler != null) handler();
        }

        private void _Output_DataReady(Object sender, System.EventArgs e)
        {
            var data = _PipeLine.Output.NonBlockingRead();
            if (data.Count > 0)
            {
                foreach (var d in data)
                {
                    OnOutputChanged(d + Environment.NewLine);
                }

            }
            if(_PipeLine.Output.EndOfPipeline )
            {
                OnFinishedRun();
            }
        }

    }
}