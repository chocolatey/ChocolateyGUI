using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Properties;
using log4net;
using System.Threading;

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

        public ODataPackageVersionService(IPackageVersionXMLParser versionXmlParser, ISourceService sourceService)
        {
            this._versionXmlParser = versionXmlParser;
            this._sourceService = sourceService;
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
            var package = packageNameObj as string;
            var packageVersion = FillWithOData(package);

            // not found on server - use what we know
            if (packageVersion == null)
            {
                packageVersion = new PackageVersion();
            }

            packageVersion.Name = package;
            packageVersion.CurrentVersion = GetInstalledVersion(package);
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
                    return _versionXmlParser.parseVersion(xmlDoc);
                }
                catch (XmlException) { }
                catch (WebException) { }
            }

            var packageVersion = new PackageVersion();
            packageVersion.Summary = "Could not download package information from '" + url + "'";
            return packageVersion;
        }

        /// <summary>
        /// Searches the chocolatey install directory to look up 
        /// the installed version of the given package.
        /// </summary>
        private string GetInstalledVersion(string package)
        {
            var packageRegexp = new Regex(@"(" + package + @")((\.\d+)+)$");

            var settings = new Settings();
            var expandedLibDirectory = System.Environment.ExpandEnvironmentVariables(settings.ChocolateyLibDirectory);
            var directories = System.IO.Directory.GetDirectories(expandedLibDirectory);

            foreach (string directory in directories)
            {
                var versionMatch = packageRegexp.Match(directory);
                var installedName = versionMatch.Groups[1].Value;
                if (installedName == package)
                {
                    return versionMatch.Groups[2].Value.Substring(1);
                }
            }
            return "no version";
        }

        private void OnVersionChanged(PackageVersion version)
        {
            var handler = VersionChanged;
            if (handler != null) handler(version);
        }
    }
}
