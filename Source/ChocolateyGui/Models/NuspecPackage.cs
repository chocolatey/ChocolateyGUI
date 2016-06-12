// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NuspecPackage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace ChocolateyGui.Models
{
    [Serializable]
    [XmlRoot(ElementName = "package")]
    public class NuspecPackage
    {
        [XmlElement("metadata")]
        public PackageMetadata Metadata { get; set; }
    }
}