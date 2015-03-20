// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ProcessResults.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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