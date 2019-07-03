// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PowerShellOutputLine.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models
{
    public enum PowerShellLineType
    {
        Output,
        Error,
        Warning,
        Verbose,
        Debug
    }

    public class PowerShellOutputLine
    {
        public PowerShellOutputLine(string text, PowerShellLineType lineType, bool newLine = true)
        {
            Text = text;
            LineType = lineType;
            NewLine = newLine;
        }

        public bool NewLine { get; private set; }

        public string Text { get; private set; }

        public PowerShellLineType LineType { get; private set; }
    }
}