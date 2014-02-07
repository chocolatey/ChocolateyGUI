using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocolatey.Gui.Models
{
    public class PowerShellOutputLine
    {
        public string Text { get; set; }
        public PowerShellLineType Type { get; set; }
    }

    public enum PowerShellLineType
    {
        Output,
        Error
    }
}
