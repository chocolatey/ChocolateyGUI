// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="StreamingLogMessage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    public class StreamingLogMessage
    {
        public string Context { get; set; }

        public StreamingLogLevel LogLevel { get; set; }

        public string Message { get; set; }
    }
}
