namespace Chocolatey.Gui.Models
{
    public class PowerShellOutputLine
    {
        public PowerShellOutputLine(string text, PowerShellLineType type)
        {
            Text = text;
            Type = type;
        }
        public string Text { get; private set; }
        public PowerShellLineType Type { get; private set; }
    }

    public enum PowerShellLineType
    {
        Output,
        Error,
        Debug,
        OutputNoNewLine,
        ErrorNoNewLine,
        DebugNoNewLine,
    }
}
