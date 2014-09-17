using System;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace ChocolateyGui.Utilities.Extensions
{
    public static class PowershellExtensions
    {
        public static Task RunCommandsAsync(this Pipeline pipeline)
        {
            if (pipeline.PipelineStateInfo.State == PipelineState.Completed)
                return TaskEx.FromResult(0);
             
            var tcs = new TaskCompletionSource<object>();
            EventHandler<PipelineStateEventArgs> stateHandler = null;
            stateHandler = (sender, args) =>
            {
                switch (args.PipelineStateInfo.State)
                {
                    case PipelineState.Stopped:
                    case PipelineState.Completed:
                        pipeline.StateChanged -= stateHandler;
                        tcs.TrySetResult(null);
                        break;
                    case PipelineState.Failed:
                        pipeline.StateChanged -= stateHandler;
                        tcs.TrySetException(args.PipelineStateInfo.Reason);
                        break;
                }
            };
            pipeline.StateChanged += stateHandler;
            pipeline.InvokeAsync();

            return tcs.Task;
        }
    }
}
