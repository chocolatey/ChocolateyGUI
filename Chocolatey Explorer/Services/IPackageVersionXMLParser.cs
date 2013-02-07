using System.Collections.Generic;
using System.Xml;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services
{
    public interface IPackageVersionXMLParser
    {
        IList<PackageVersion> parse(XmlDocument xmlDoc);
    }
}
