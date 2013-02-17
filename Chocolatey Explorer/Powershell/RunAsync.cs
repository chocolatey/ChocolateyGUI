using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Chocolatey.Explorer.Powershell
{
    public class RunAsync : IRunAsync
    {
        private readonly Runspace _runSpace;
        private Pipeline _pipeLine;
        private PipelineReader<PSObject> _outPut;
        private bool hasData;
        public event ResultsHandler OutputChanged;

        public event EmptyHandler RunFinished;

        public RunAsync()
        {
            _runSpace = RunspaceFactory.CreateRunspace();
            _runSpace.Open();
        }

        public void Run(String command)
        {
            this.Log().Info("Running command: " + command);
            _pipeLine = _runSpace.CreatePipeline(command);
            _pipeLine.Input.Close();
            _outPut = _pipeLine.Output;
            _outPut.DataReady += OutputDataReady;
            _pipeLine.InvokeAsync();
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

        private void OutputDataReady(Object sender, EventArgs e)
        {
            var data = _pipeLine.Output.NonBlockingRead();
            if (data.Count > 0)
            {
                hasData = true;
                foreach (var d in data)
                {
                    OnOutputChanged(d + Environment.NewLine);
                }

            }
            if(_pipeLine.Output.EndOfPipeline )
            {
                if (!hasData) OnOutputChanged("No output");
                OnFinishedRun();
            }
        }

    }
}