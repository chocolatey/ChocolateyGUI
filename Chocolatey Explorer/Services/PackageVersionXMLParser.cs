using System.Xml;
using Chocolatey.Explorer.Model;
using System;

namespace Chocolatey.Explorer.Services
{
    class PackageVersionXMLParser : IPackageVersionXMLParser
    {
        private const string METADATA_NS = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private const string DATA_SERVICES_NS = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        /// <summary>
        /// Parses a xml document from the chocolatey api feed 
        /// into a package version object.
        /// </summary>
        /// <param name="xmlDoc">XML document that should be parsed</param>
        public PackageVersion parse(XmlDocument xmlDoc)
        {
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("ns", xmlDoc.DocumentElement.NamespaceURI);
            nsmgr.AddNamespace("m", METADATA_NS);
            nsmgr.AddNamespace("d", DATA_SERVICES_NS);

            var version = new PackageVersion();
            var entry = xmlDoc.DocumentElement.SelectSingleNode("/ns:feed/ns:entry", nsmgr);

            if (entry == null)
            {
                version.Summary = "Could not parse package information.";
                version.Serverversion = "no version";
                return version;
            }
            
            version.Name = entry.SelectSingleNode("ns:title", nsmgr).InnerText;
            version.Summary = entry.SelectSingleNode("ns:summary", nsmgr).InnerText;
            version.AuthorName = entry.SelectSingleNode("ns:author/ns:name", nsmgr).InnerText;
            version.LastUpdatedAt = DateTime.Parse(entry.SelectSingleNode("ns:updated", nsmgr).InnerText);

            var properties = entry.SelectSingleNode("m:properties", nsmgr);
            if (properties != null)
            {
                version.Serverversion = properties.SelectSingleNode("d:Version", nsmgr).InnerText;
                version.CopyrightInformation = properties.SelectSingleNode("d:Copyright", nsmgr).InnerText;
                version.CreatedAt = DateTime.Parse(properties.SelectSingleNode("d:Created", nsmgr).InnerText);
                version.Dependencies = properties.SelectSingleNode("d:Dependencies", nsmgr).InnerText;
                version.Description = properties.SelectSingleNode("d:Description", nsmgr).InnerText;
                version.DownloadCount = int.Parse(properties.SelectSingleNode("d:DownloadCount", nsmgr).InnerText);
                version.GalleryDetailsUrl = properties.SelectSingleNode("d:GalleryDetailsUrl ", nsmgr).InnerText;
                version.IconUrl = properties.SelectSingleNode("d:IconUrl ", nsmgr).InnerText;
                version.IsPrerelease = bool.Parse(properties.SelectSingleNode("d:IsPrerelease", nsmgr).InnerText);
                version.Language = properties.SelectSingleNode("d:Language", nsmgr).InnerText;
                version.PublishedAt = DateTime.Parse(properties.SelectSingleNode("d:Published", nsmgr).InnerText);
                version.LicenseUrl = properties.SelectSingleNode("d:LicenseUrl", nsmgr).InnerText;
                version.PackageSize = UInt64.Parse(properties.SelectSingleNode("d:PackageSize", nsmgr).InnerText);
                version.ProjectUrl = properties.SelectSingleNode("d:ProjectUrl", nsmgr).InnerText;
                version.ReportAbuseUrl = properties.SelectSingleNode("d:ReportAbuseUrl", nsmgr).InnerText;
                version.ReleaseNotes = properties.SelectSingleNode("d:ReleaseNotes", nsmgr).InnerText;
                version.RequireLicenseAcceptance = bool.Parse(properties.SelectSingleNode("d:RequireLicenseAcceptance", nsmgr).InnerText);
                version.Tags = properties.SelectSingleNode("d:Tags", nsmgr).InnerText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                version.VersionDownloadCount = int.Parse(properties.SelectSingleNode("d:VersionDownloadCount", nsmgr).InnerText);
            }

            return version;
        }
    }
}
