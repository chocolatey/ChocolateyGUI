using System.Xml;
using Chocolatey.Explorer.Model;
using System;
using log4net;
using System.Collections.Generic;

namespace Chocolatey.Explorer.Services
{
    class PackageVersionXMLParser : IPackageVersionXMLParser
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PackageVersionXMLParser));

        private const string METADATA_NS = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private const string DATA_SERVICES_NS = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        public int LastTotalCount { get; set; }

        /// <summary>
        /// Parses a xml document from the chocolatey api feed 
        /// into a list of package version objects.
        /// </summary>
        /// <param name="xmlDoc">XML document that should be parsed</param>
        public IList<PackageVersion> parse(XmlDocument xmlDoc)
        {
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("ns", xmlDoc.DocumentElement.NamespaceURI);
            nsmgr.AddNamespace("m", METADATA_NS);
            nsmgr.AddNamespace("d", DATA_SERVICES_NS);

            var countNode = xmlDoc.DocumentElement.SelectSingleNode("/ns:feed/m:count", nsmgr);
            if (countNode != null)
            {
                LastTotalCount = int.Parse(countNode.InnerText);
            }

            var entries = xmlDoc.DocumentElement.SelectNodes("/ns:feed/ns:entry", nsmgr);

            IList<PackageVersion> versionList = new List<PackageVersion>();
            foreach (XmlNode entry in entries)
            {
                versionList.Add(ParseEntry(entry, nsmgr));
            }
            return versionList;
        }

        private PackageVersion ParseEntry(XmlNode entryNode, XmlNamespaceManager nsmgr)
        {
            var version = new PackageVersion();
            if (entryNode == null)
            {
                version.Summary = "Could not parse package information.";
                version.Description = version.Summary;
                version.Serverversion = "no version";
                return version;
            }

            version.Name = entryNode.SelectSingleNode("ns:title", nsmgr).InnerText;
            version.Summary = entryNode.SelectSingleNode("ns:summary", nsmgr).InnerText;
            version.AuthorName = entryNode.SelectSingleNode("ns:author/ns:name", nsmgr).InnerText;
            version.LastUpdatedAt = DateTime.Parse(entryNode.SelectSingleNode("ns:updated", nsmgr).InnerText);

            var properties = entryNode.SelectSingleNode("m:properties", nsmgr);
            if (properties != null)
            {
                var serverversion = properties.SelectSingleNode("d:Version", nsmgr);
                var copyrightInformation = properties.SelectSingleNode("d:Copyright", nsmgr);
                var createdAt = properties.SelectSingleNode("d:Created", nsmgr);
                var dependencies = properties.SelectSingleNode("d:Dependencies", nsmgr);
                var description = properties.SelectSingleNode("d:Description", nsmgr);
                var downloadCount = properties.SelectSingleNode("d:DownloadCount", nsmgr);
                var galleryDetailsUrl = properties.SelectSingleNode("d:GalleryDetailsUrl ", nsmgr);
                var iconUrl = properties.SelectSingleNode("d:IconUrl ", nsmgr);
                var isPrerelease = properties.SelectSingleNode("d:IsPrerelease", nsmgr);
                var language = properties.SelectSingleNode("d:Language", nsmgr);
                var publishedAt = properties.SelectSingleNode("d:Published", nsmgr);
                var licenseUrl = properties.SelectSingleNode("d:LicenseUrl", nsmgr);
                var packageSize = properties.SelectSingleNode("d:PackageSize", nsmgr);
                var projectUrl = properties.SelectSingleNode("d:ProjectUrl", nsmgr);
                var reportAbuseUrl = properties.SelectSingleNode("d:ReportAbuseUrl", nsmgr);
                var releaseNotes = properties.SelectSingleNode("d:ReleaseNotes", nsmgr);
                var requireLicenseAcceptance = properties.SelectSingleNode("d:RequireLicenseAcceptance", nsmgr);
                var tags = properties.SelectSingleNode("d:Tags", nsmgr);
                var versionDownloadCount = properties.SelectSingleNode("d:VersionDownloadCount", nsmgr);

                if (serverversion != null)
                    version.Serverversion = serverversion.InnerText;
                if (copyrightInformation != null)
                    version.CopyrightInformation = copyrightInformation.InnerText;
                if (createdAt != null)
                    version.CreatedAt = DateTime.Parse(createdAt.InnerText);
                if (dependencies != null)
                    version.Dependencies = dependencies.InnerText.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (description != null)
                    version.Description = description.InnerText.Replace("\n", "\r\n");
                if (downloadCount != null)
                    version.DownloadCount = int.Parse(downloadCount.InnerText);
                if (galleryDetailsUrl != null)
                    version.GalleryDetailsUrl = galleryDetailsUrl.InnerText;
                if (iconUrl != null)
                    version.IconUrl = iconUrl.InnerText;
                if (isPrerelease != null)
                    version.IsPrerelease = bool.Parse(isPrerelease.InnerText);
                if (language != null)
                    version.Language = language.InnerText;
                if (publishedAt != null)
                    version.PublishedAt = DateTime.Parse(publishedAt.InnerText);
                if (licenseUrl != null)
                    version.LicenseUrl = licenseUrl.InnerText;
                if (packageSize != null)
                    version.PackageSize = UInt64.Parse(packageSize.InnerText);
                if (projectUrl != null)
                    version.ProjectUrl = projectUrl.InnerText;
                if (reportAbuseUrl != null)
                    version.ReportAbuseUrl = reportAbuseUrl.InnerText;
                if (releaseNotes != null)
                    version.ReleaseNotes = releaseNotes.InnerText;
                if (requireLicenseAcceptance != null)
                    version.RequireLicenseAcceptance = bool.Parse(requireLicenseAcceptance.InnerText);
                if (tags != null)
                    version.Tags = tags.InnerText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (versionDownloadCount != null)
                    version.VersionDownloadCount = int.Parse(versionDownloadCount.InnerText);
            }

            return version;
        }
    }
}
