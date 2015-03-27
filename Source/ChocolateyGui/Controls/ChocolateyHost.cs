// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyHost.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Controls
{
    using System;
    using System.Globalization;
    using System.Management.Automation.Host;
    using System.Reflection;
    using ChocolateyGui.Services;

    internal class ChocolateyHost : PSHost
    {
        private readonly ChocolateyHostUserInterface _chocolateyHostUserInterface;

        private readonly Guid _hostInstanceId = Guid.NewGuid();

        private readonly CultureInfo _originalCultureInfo =
            System.Threading.Thread.CurrentThread.CurrentCulture;

        private readonly CultureInfo _originalUiCultureInfo =
            System.Threading.Thread.CurrentThread.CurrentUICulture;

        public ChocolateyHost(IProgressService progressService)
        {
            this._chocolateyHostUserInterface = new ChocolateyHostUserInterface(progressService);
        }

        public override CultureInfo CurrentCulture
        {
            get { return this._originalCultureInfo; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return this._originalUiCultureInfo; }
        }

        public override Guid InstanceId
        {
            get { return this._hostInstanceId; }
        }

        public override string Name
        {
            get { return @"ChocolateyGUI PowerShell Host"; }
        }

        public override PSHostUserInterface UI
        {
            get { return this._chocolateyHostUserInterface; }
        }

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override void SetShouldExit(int exitCode)
        {
            throw new NotImplementedException();
        }
    }
}