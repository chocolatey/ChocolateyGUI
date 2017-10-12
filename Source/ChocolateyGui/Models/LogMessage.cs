// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LogMessage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    public class LogMessage
    {
        public string Context { get; set; }

        public LogLevel LogLevel { get; set; }

        public string Message { get; set; }
    }
}