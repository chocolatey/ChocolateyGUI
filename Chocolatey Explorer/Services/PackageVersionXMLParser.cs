using System.Xml;
using Chocolatey.Explorer.Model;

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
                return null;
            
            version.Name = entry.SelectSingleNode("ns:title", nsmgr).InnerText;
            version.Description = entry.SelectSingleNode("ns:summary", nsmgr).InnerText;
            version.AuthorName = entry.SelectSingleNode("ns:author/ns:name", nsmgr).InnerText;

            var properties = entry.SelectSingleNode("m:properties", nsmgr);
            version.Serverversion = properties.SelectSingleNode("d:Version", nsmgr).InnerText;

            return version;
        }
    }
}
