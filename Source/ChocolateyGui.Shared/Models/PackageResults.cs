// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageResults.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace ChocolateyGui.Models
{
    [DataContract]
    public class PackageResults
    {
        [DataMember]
        public Package[] Packages { get; set; }

        [DataMember]
        public int TotalCount { get; set; }
    }
}