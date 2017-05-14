// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageOperationResult.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace ChocolateyGui.Models
{
    [DataContract]
    public class PackageOperationResult
    {
        internal static readonly PackageOperationResult SuccessfulCached = new PackageOperationResult
        {
            Successful = true
        };

        [DataMember]
        public bool Successful { get; set; }

        [DataMember]
        public string[] Messages { get; set; } = new string[0];

        [DataMember]
        public Exception Exception { get; set; }
    }
}