using System.Collections.Generic;
using System.Xml;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services
{
    public interface IPackageVersionXMLParser
    {
        int LastTotalCount {get; }
        IList<PackageVersion> parse(XmlDocument xmlDoc);
    }
}
