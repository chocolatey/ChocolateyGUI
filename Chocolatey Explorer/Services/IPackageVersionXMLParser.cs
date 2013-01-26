using System.Xml;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services
{
    interface IPackageVersionXMLParser
    {
        PackageVersion parse(XmlDocument xmlDoc);
    }
}
