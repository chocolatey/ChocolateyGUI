// --------------------------------------------------------------------------------------------------------------------
// <copyright company="James Manning" file="ProcessEx.cs">
//   Copyright (c) 2013 James Manning
//   This file was taken from here:
//   https://github.com/jamesmanning/RunProcessAsTask
//   and adapted under the MIT licensing rules.  Original copyright is in tact.
//   Modifications:
//     - prevent the creation of a new window when executing task
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.AsyncProcess
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class ProcessResults
    {
        private readonly Process _process;
        private readonly IEnumerable<string> _standardOutput;
        private readonly IEnumerable<string> _standardError;

        public ProcessResults(Process process, IEnumerable<string> standardOutput, IEnumerable<string> standardError)
        {
            this._process = process;
            this._standardOutput = standardOutput;
            this._standardError = standardError;
        }

        public Process Process
        {
            get { return this._process; }
        }

        public IEnumerable<string> StandardOutput
        {
            get { return this._standardOutput; }
        }

        public IEnumerable<string> StandardError
        {
            get { return this._standardError; }
        }
    }
}