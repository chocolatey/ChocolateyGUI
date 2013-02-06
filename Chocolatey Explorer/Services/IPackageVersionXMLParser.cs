using System.Xml;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services
{
    public interface IPackageVersionXMLParser
    {
        PackageVersion parse(XmlDocument xmlDoc);
    }
}
