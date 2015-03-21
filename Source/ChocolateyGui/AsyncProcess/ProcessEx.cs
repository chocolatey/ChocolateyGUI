// --------------------------------------------------------------------------------------------------------------------
// <copyright company="James Manning" file="ProcessEx.cs">
//   Copyright (c) 2013 James Manning
//   This file was taken from here:
//   https://github.com/jamesmanning/RunProcessAsTask
//   and adapted under the MIT licensing rules.  Original copyright is in tact.
//   Modifications:
//     - prevent the creation of a new window when executing task
//     - correcting FxCop errors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.AsyncProcess
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The ProcessEx Class
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Not our code.")]
    public static partial class ProcessEx
    {
        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo)
        {
            return RunAsync(processStartInfo, CancellationToken.None);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not our code, and works as is.")]
        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
        {
            if (processStartInfo == null)
            {
                throw new ArgumentNullException("processStartInfo");
            }

            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;

            var tcs = new TaskCompletionSource<ProcessResults>();

            var standardOutput = new List<string>();
            var standardError = new List<string>();

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardOutput.Add(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardError.Add(args.Data);
                }
            };

            process.Exited += (sender, args) => tcs.TrySetResult(new ProcessResults(process, standardOutput, standardError));

            cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
                process.CloseMainWindow();
            });

            cancellationToken.ThrowIfCancellationRequested();

            if (process.Start() == false)
            {
                tcs.TrySetException(new InvalidOperationException("Failed to start process"));
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}