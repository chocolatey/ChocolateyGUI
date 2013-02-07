using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Properties;
using log4net;

namespace Chocolatey.Explorer.Services
{
    public class ODataPackagesService : IPackagesService
    {
        private const string ALL_PACKAGES_URL = "/Packages?$filter=IsLatestVersion eq true&$inlinecount=allpages&$select=Title";
        private static readonly ILog log = LogManager.GetLogger(typeof(ODataPackagesService));

        private readonly ISourceService _sourceService;
        private readonly IPackageVersionXMLParser _xmlParser;
        private readonly ChocolateyLibDirHelper _libDirHelper;

        public delegate void PackagesServiceFinishedDelegate(IList<Package> packages);
        public event PackagesService.FinishedDelegate RunFinshed;

        public ODataPackagesService(): this(new SourceService(), new PackageVersionXMLParser())
        {
        }

        public ODataPackagesService(ISourceService sourceService, IPackageVersionXMLParser xmlParser)
        {
            _sourceService = sourceService;
            _xmlParser = xmlParser;
            _libDirHelper = new ChocolateyLibDirHelper();
        }

        public void ListOfPackages()
        {
            var thread = new Thread(LoadAllPackagesThread) { IsBackground = true };
            thread.Start();

        }

        private void LoadAllPackagesThread()
        {
            string fullUrl = _sourceService.Source + ALL_PACKAGES_URL;

            XmlDocument xmlDoc = new XmlDocument();
            IList<Package> allPackages = new List<Package>();
            IList<Package> packages;
            do
            {
                var skipUrl = fullUrl + "&$skip=" + allPackages.Count();
                log.Debug("Getting list of packages on source: " + skipUrl);

                var rssFeed = WebRequest.Create(skipUrl) as HttpWebRequest;
                packages = new List<Package>();
                if (rssFeed != null)
                {
                    try
                    {
                        xmlDoc.Load(rssFeed.GetResponse().GetResponseStream());
                        packages = _xmlParser.parse(xmlDoc).Select(
                                e => new Package() 
                                { 
                                    Name = e.Name,
                                    InstalledVersion = _libDirHelper.GetHighestInstalledVersion(e.Name)
                                }
                            ).ToList<Package>();
                    }
                    catch (XmlException) { }
                    catch (WebException) { }
                }
                allPackages = allPackages.Concat(packages).ToList<Package>();
            } while (packages.Count() > 0);

            OnRunFinshed(allPackages);
        }

        public void ListOfInstalledPackages()
        {
            log.Info("Getting list of installed packages");
            var thread = new Thread(ListOfInstalledPackagsThread) {IsBackground = true};
            thread.Start();
        }

        private void ListOfInstalledPackagsThread()
        {
            OnRunFinshed(_libDirHelper.ReloadFromDir());
        }

        private void OnRunFinshed(IList<Package> packages)
        {
            PackagesService.FinishedDelegate handler = RunFinshed;
            if (handler != null) handler(packages);
        }

    }
}