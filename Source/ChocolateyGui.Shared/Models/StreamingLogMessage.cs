// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="StreamingLogMessage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace ChocolateyGui.Models
{
    [DataContract]
    public class StreamingLogMessage
    {
        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public StreamingLogLevel LogLevel { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}