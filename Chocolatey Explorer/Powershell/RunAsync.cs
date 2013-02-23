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
            this.Log().Info("Running command: {0}", command);
            _pipeLine = _runSpace.CreatePipeline(command);
            _pipeLine.Input.Close();
            _outPut = _pipeLine.Output;
            _outPut.DataReady += OutputDataReady;
            _pipeLine.InvokeAsync();
        }

        public void OnOutputChanged(string output)
        {
            this.Log().Debug("Output changed: {0}", output);
            var handler = OutputChanged;
            if (handler != null) handler(output);
        }

        public void OnFinishedRun()
        {
            this.Log().Debug("Run finished");
            var handler = RunFinished;
            if (handler != null) handler();
        }

        private void OutputDataReady(Object sender, EventArgs e)
        {
            this.Log().Debug("Output ready");
            var data = _pipeLine.Output.NonBlockingRead();
            if (data.Count > 0)
            {
                this.Log().Debug("Has data");
                hasData = true;
                foreach (var d in data)
                {
                    this.Log().Debug("data: {0}", d);
                    OnOutputChanged(d + Environment.NewLine);
                }

            }
            if (!_pipeLine.Output.EndOfPipeline) return;
            if (!hasData)
            {
                this.Log().Debug("No output");
                OnOutputChanged("No output");
            }
            OnFinishedRun();
        }

    }
}