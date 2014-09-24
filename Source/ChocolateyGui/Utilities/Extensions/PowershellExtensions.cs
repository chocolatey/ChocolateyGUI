// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PowershellExtensions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Extensions
{
    using System;
    using System.Management.Automation.Runspaces;
    using System.Threading.Tasks;

    public static class PowershellExtensions
    {
        public static Task RunCommandsAsync(this Pipeline pipeline)
        {
            if (pipeline.PipelineStateInfo.State == PipelineState.Completed)
            {
                return Task.FromResult<object>(null);
            }
             
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