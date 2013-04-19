using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.SourceService;

namespace Chocolatey.Explorer.Services.PackagesService
{
    public class ODataAvailablePackagesService : IODataAvailablePackagesService
    {
        private const string AllPackagesUrl = "/Packages?$filter=IsLatestVersion eq true&$inlinecount=allpages&$select=Title";

        private readonly ISourceService _sourceService;
        private readonly IPackageVersionXMLParser _xmlParser;
        private readonly IChocolateyLibDirHelper _libDirHelper;

        public event Delegates.FinishedDelegate RunFinshed;
		public event Delegates.FailedDelegate RunFailed;
        public event Delegates.StartedDelegate RunStarted;

        public ODataAvailablePackagesService(ISourceService sourceService, IPackageVersionXMLParser xmlParser, IChocolateyLibDirHelper libDirHelper)
        {
            _sourceService = sourceService;
            _xmlParser = xmlParser;
            _libDirHelper = libDirHelper;
        }

        public void ListOfAvailablePackages()
        {
            this.Log().Info("Getting list of available packages.");
            OnRunStarted();
            var thread = new Thread(LoadAllPackagesThread) { IsBackground = true };
            thread.Start();
        }

        private void LoadAllPackagesThread()
        {
            // load and parse first page to have total item count
            this.Log().Debug("Load packages");
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
            var allPackages = packageVersions.Select( PackageFromVersion );
            foreach (var backgroundPageObject in bgPageObjects)
            {
                backgroundPageObject.DoneEvent.WaitOne();
                allPackages = allPackages.Concat(
                        backgroundPageObject.PackageVersions.Select( PackageFromVersion )
                    );
            }
            OnRunFinshed(allPackages.ToList());
        }

        private Package PackageFromVersion(PackageVersion version)
        {
            this.Log().Debug("Get package from version: {0}", version);
            var highestPackage = _libDirHelper.GetHighestInstalledVersion(version.Name, false);
            return new Package
                {
                Name = version.Name,
                InstalledVersion = highestPackage == null?strings.not_available:highestPackage.InstalledVersion
            };
        }

        private void LoadPageAsync(object threadContext)
        {
            this.Log().Debug("Load page async");
            var backgroundPageObject = threadContext as BackgroundPageObject;
            var xmlDocument = LoadFeedDoc(backgroundPageObject.Skip);
            backgroundPageObject.PackageVersions = _xmlParser.parse(xmlDocument);
            backgroundPageObject.DoneEvent.Set();
        }

        private XmlDocument LoadFeedDoc(int skip)
        {
            this.Log().Debug("LoadFeedDoc: {0}", skip);
            var fullUrl = _sourceService.Source.Url + AllPackagesUrl;
            var skipUrl = fullUrl + "&$skip=" + skip;
            this.Log().Debug("Getting list of packages on source: {0}", skipUrl);

            var xmlDoc = new XmlDocument();
            var rssFeed = WebRequest.Create(skipUrl) as HttpWebRequest;
            rssFeed.Proxy = null;
            if (rssFeed != null)
            {
                try
                {
                    xmlDoc.Load(rssFeed.GetResponse().GetResponseStream());
                    return xmlDoc;
                }
                catch (WebException ex)
                {
                    this.Log().Error("Message: {0} - Stacktrac: {1}", ex.Message, ex.StackTrace);
                }
                catch (XmlException ex)
                {
                    this.Log().Error("Message: {0} - Stacktrac: {1}",ex.Message, ex.StackTrace);
                }
            }
            return null;
        }

		private void OnRunFinshed(IList<Package> packages)
        {
            this.Log().Debug("Run finshed");
            var handler = RunFinshed;
            if (handler != null) handler(packages);
        }

        private void OnRunStarted()
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler("Getting list of available packages.");
        }

        public class BackgroundPageObject
        {
            public int Skip{ get; set; }
            public ManualResetEvent DoneEvent { get; set; }
            public IList<PackageVersion> PackageVersions { get; set; }

            public BackgroundPageObject(int skip, ManualResetEvent doneEvent)
            {
                Skip = skip;
                DoneEvent = doneEvent;
            }
        }
    }
}