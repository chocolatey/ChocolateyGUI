namespace ChocolateyGui.Models
{
    public class PowerShellOutputLine
    {
        public PowerShellOutputLine(string text, PowerShellLineType type, bool newLine = true)
        {
            Text = text;
            Type = type;
            NewLine = newLine;
        }
        public string Text { get; private set; }
        public PowerShellLineType Type { get; private set; }
        public bool NewLine { get; private set; }
    }

    public enum PowerShellLineType
    {
        Output,
        Error,
        Warning,
        Verbose,
        Debug,
    }
}
