// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyHostUserInterface.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;

    internal class ChocolateyHostUserInterface : PSHostUserInterface
    {
        private readonly ChocolateyHostRawUserInterface _chocoRawUI = new ChocolateyHostRawUserInterface();
        private readonly IProgressService _progressService;

        public ChocolateyHostUserInterface(IProgressService progressService)
        {
            this._progressService = progressService;
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return this._chocoRawUI; }
        }

        public override Dictionary<string, PSObject> Prompt(
                                                            string caption,
                                                            string message,
                                                            System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException(
                "Prompt is not implemented.");
        }

        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException("PromptForChoice is not implemented.");
        }

        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName)
        {
            throw new NotImplementedException("PromptForCredential is not implemented.");
        }

        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName,
                                                         PSCredentialTypes allowedCredentialTypes,
                                                         PSCredentialUIOptions options)
        {
            Console.WriteLine("ReadLine");
            throw new NotImplementedException("PromptForCredential is not implemented.");
        }

        public override string ReadLine()
        {
            throw new NotImplementedException("ReadLine is not implemented.");
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("ReadLineAsSecureString is not implemented.");
        }

        public override void Write(string value)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output, newLine: false));
        }

        public override void Write(
                                   ConsoleColor foregroundColor,
                                   ConsoleColor backgroundColor,
                                   string value)
        {
            // Colors are ignored.
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output, newLine: false));
        }

        public override void WriteDebugLine(string message)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(message, PowerShellLineType.Debug));
        }

        public override void WriteErrorLine(string value)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Error));
        }

        public override void WriteLine()
        {
            this._progressService.Output.Add(new PowerShellOutputLine(string.Empty, PowerShellLineType.Output));
        }

        public override void WriteLine(string value)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output));
        }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            // Write to the output stream, ignore the colors
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output));
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            this._progressService.Report(record.PercentComplete);
        }

        public override void WriteVerboseLine(string message)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(message, PowerShellLineType.Warning));
        }

        public override void WriteWarningLine(string message)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(message, PowerShellLineType.Warning));
        }
    }
}