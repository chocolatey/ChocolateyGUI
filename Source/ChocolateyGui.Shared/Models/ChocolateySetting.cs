// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateySetting.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace ChocolateyGui.Models
{
    [DataContract]
    public class ChocolateySetting
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]

        public string Value { get; set; }

        [DataMember]

        public string Description { get; set; }
    }
}