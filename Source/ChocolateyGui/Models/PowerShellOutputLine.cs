// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PowerShellOutputLine.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    public enum PowerShellLineType
    {
        Output,
        Error,
        Warning,
        Verbose,
        Debug,
    }

    public class PowerShellOutputLine
    {
        public PowerShellOutputLine(string text, PowerShellLineType type, bool newLine = true)
        {
            this.Text = text;
            this.Type = type;
            this.NewLine = newLine;
        }

        public bool NewLine { get; private set; }

        public string Text { get; private set; }

        public PowerShellLineType Type { get; private set; }
    }
}