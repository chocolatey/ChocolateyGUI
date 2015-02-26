// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NuspecPackage.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Models
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "package")]
    public class NuspecPackage
    {
        [XmlElement("metadata")]
        public PackageMetadata Metadata { get; set; }
    }
}