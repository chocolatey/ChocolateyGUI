using System.Net;
using System.Threading;
using System.Xml;
using Chocolatey.Explorer.Model;
using log4net;

namespace Chocolatey.Explorer.Services
{
    /// <summary>
    /// This class gets its package information via api feed of the current source.
    /// This way more detailed package information can be obtained.
    /// The version of the currently installed package is read from the chocolatey
    /// lib directory.
    /// </summary>
    class ODataPackageVersionService : IPackageVersionService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ODataPackageVersionService));

        public event PackageVersionService.VersionResult VersionChanged;
        public delegate void VersionResult(PackageVersion version);

        private readonly ISourceService _sourceService;
        private readonly IPackageVersionXMLParser _versionXmlParser;
        private readonly ChocolateyLibDirHelper _libDirHelper;

        public ODataPackageVersionService(IPackageVersionXMLParser versionXmlParser, ISourceService sourceService)
        {
            this._versionXmlParser = versionXmlParser;
            this._sourceService = sourceService;
            this._libDirHelper = new ChocolateyLibDirHelper();
        }

        public void PackageVersion(string package)
        {
            log.Info("Getting version of package: " + package);
            var start = new ParameterizedThreadStart(PackageVersionThread);
            var thread = new Thread(start) { IsBackground = true };
            thread.Start(package);
        }

        private void PackageVersionThread(object packageNameObj)
        {
            var packageName = packageNameObj as string;
            var packageVersion = FillWithOData(packageName);

            // not found on server - use what we know
            if (packageVersion == null)
            {
                packageVersion = new PackageVersion();
            }

            packageVersion.Name = packageName;
            packageVersion.CurrentVersion = _libDirHelper.GetHighestInstalledVersion(packageName);
            OnVersionChanged(packageVersion);
        }

        private PackageVersion FillWithOData(string package)
        {
            string url = _sourceService.Source + "/Packages?$filter=IsLatestVersion eq true and Id eq '" + package + "'";
            XmlDocument xmlDoc = new XmlDocument();

            var rssFeed = WebRequest.Create(url) as HttpWebRequest;

            if (rssFeed != null)
            {
                try
                {
                    xmlDoc.Load(rssFeed.GetResponse().GetResponseStream());
                    return _versionXmlParser.parse(xmlDoc);
                }
                catch (XmlException) { }
                catch (WebException) { }
            }

            var packageVersion = new PackageVersion();
            packageVersion.Summary = "Could not download package information from '" + url + "'";
            return packageVersion;
        }

        private void OnVersionChanged(PackageVersion version)
        {
            var handler = VersionChanged;
            if (handler != null) handler(version);
        }
    }
}
