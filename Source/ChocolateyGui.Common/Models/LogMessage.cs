// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LogMessage.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Common.Models
{
    public class LogMessage
    {
        public string Context { get; set; }

        public LogLevel LogLevel { get; set; }

        public string Message { get; set; }
    }
}