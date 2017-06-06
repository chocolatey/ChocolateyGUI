// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyFeature.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace ChocolateyGui.Models
{
    [DataContract]
    public class ChocolateyFeature
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public bool SetExplicitly { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}