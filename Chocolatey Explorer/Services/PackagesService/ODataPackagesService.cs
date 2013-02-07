using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using Chocolatey.Explorer.Model;
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
            // load and parse first page to have total item count
            var xmlDocument = LoadFeedDoc(0);
            var packageVersions = _xmlParser.parse(xmlDocument);
            var totalCount = _xmlParser.LastTotalCount;
            var pageSize = packageVersions.Count();

            // load other pages with thread pool
            IList<BackgroundPageObject> bgPageObjects = new List<BackgroundPageObject>();
            for (var skip = pageSize; skip < totalCount; skip += pageSize)
            {
                var backgroundPageObject = new BackgroundPageObject(skip, new ManualResetEvent(false));
                bgPageObjects.Add(backgroundPageObject);
                ThreadPool.QueueUserWorkItem(LoadPageAsync, backgroundPageObject);
            }

            // concat and return results
            _libDirHelper.ReloadFromDir();
            IEnumerable<Package> allPackages = packageVersions.Select( e => PackageFromVersion(e) );
            foreach (var backgroundPageObject in bgPageObjects)
            {
                backgroundPageObject.DoneEvent.WaitOne();
                allPackages = allPackages.Concat(
                        backgroundPageObject.PackageVersions.Select( e => PackageFromVersion(e) )
                    );
            }
            OnRunFinshed(allPackages.ToList());
        }

        private Package PackageFromVersion(PackageVersion version)
        {
            return new Package()
            {
                Name = version.Name,
                InstalledVersion = _libDirHelper.GetHighestInstalledVersion(version.Name, false)
            };
        }

        private void LoadPageAsync(object threadContext)
        {
            var backgroundPageObject = threadContext as BackgroundPageObject;
            var xmlDocument = LoadFeedDoc(backgroundPageObject.Skip);
            backgroundPageObject.PackageVersions = _xmlParser.parse(xmlDocument);
            backgroundPageObject.DoneEvent.Set();
        }

        private XmlDocument LoadFeedDoc(int skip)
        {
            var fullUrl = _sourceService.Source + ALL_PACKAGES_URL;
            var skipUrl = fullUrl + "&$skip=" + skip;
            log.Debug("Getting list of packages on source: " + skipUrl);

            var xmlDoc = new XmlDocument();
            var rssFeed = WebRequest.Create(skipUrl) as HttpWebRequest;
            if (rssFeed != null)
            {
                try
                {
                    xmlDoc.Load(rssFeed.GetResponse().GetResponseStream());
                    return xmlDoc;
                }
                catch (WebException) { }
                catch (XmlException) { }
            }
            return null;
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

        public class BackgroundPageObject
        {
            public int Skip{ get; set; }
            public ManualResetEvent DoneEvent { get; set; }
            public IList<PackageVersion> PackageVersions { get; set; }

            public BackgroundPageObject(int skip, ManualResetEvent doneEvent)
            {
                this.Skip = skip;
                this.DoneEvent = doneEvent;
            }
        }
    }
}