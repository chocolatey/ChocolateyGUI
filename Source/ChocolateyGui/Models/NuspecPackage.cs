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

    [Serializable]
    public class PackageMetadata
    {
        [XmlElement("id")]
        public string Id { get; set; }
        [XmlElement("version")]
        public string Version { get; set; }
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("authors")]
        public string Authors { get; set; }
        [XmlElement("owners")]
        public string Owners { get; set; }
        [XmlElement("licenseUrl")]
        public string LicenceUrl { get; set; }
        [XmlElement("projectUrl")]
        public string ProjectUrl { get; set; }
        [XmlElement("iconUrl")]
        public string IconUrl { get; set; }
        [XmlElement("requireLicenseAcceptance")]
        public bool RequireLicenseAcceptance { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("summary")]
        public string Summary { get; set; }
        [XmlElement("tags")]
        public string Tags { get; set; }
    }
}
