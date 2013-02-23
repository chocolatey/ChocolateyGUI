using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using Chocolatey.Explorer.Model;
using System.IO;
using Chocolatey.Explorer.Services.SourceService;

namespace Chocolatey.Explorer.Services.PackageVersionService
{
    /// <summary>
    /// This class gets its package information via api feed of the current source.
    /// This way more detailed package information can be obtained.
    /// The version of the currently installed package is read from the chocolatey
    /// lib directory.
    /// </summary>
    class ODataPackageVersionService : IPackageVersionService
    {
        public event Delegates.VersionResult VersionChanged;
        public event Delegates.StartedDelegate RunStarted;

        private readonly ISourceService _sourceService;
        private readonly IPackageVersionXMLParser _versionXmlParser;
        private readonly IChocolateyLibDirHelper _libDirHelper;
        private CancellationTokenSource _cancelTokenSource;
        private HttpWebRequest _loadingRssFeed;

        public ODataPackageVersionService(IPackageVersionXMLParser versionXmlParser, ISourceService sourceService, IChocolateyLibDirHelper libDirHelper)
        {
            _versionXmlParser = versionXmlParser;
            _sourceService = sourceService;
            _libDirHelper = libDirHelper;
        }

        public void PackageVersion(string package)
        {
            this.Log().Info("Getting version of package: " + package);
            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
            }
            _cancelTokenSource = new CancellationTokenSource();
            _cancelTokenSource.Token.Register(OnPackageVersionThreadCancel);
            OnStarted(package);
            var thread = new Thread(() => PackageVersionThread(_cancelTokenSource.Token, package)) { IsBackground = true };
            thread.Start();
        }

        private void OnPackageVersionThreadCancel()
        {
            this.Log().Debug("Packageversion thread cancelled");
            if (_loadingRssFeed != null)
            {
                _loadingRssFeed.Abort();
            }
        }

        private void PackageVersionThread(CancellationToken cancelToken, string packageNameObj)
        {
            try
            {
                this.Log().Debug("Getting package: {0}", packageNameObj);
                var packageName = packageNameObj;
                var packageVersion = FillWithOData(packageName) ?? new PackageVersion();

                // not found on server - use what we know

                cancelToken.ThrowIfCancellationRequested();
                packageVersion.Name = packageName;
                var highestInstalledVersion = _libDirHelper.GetHighestInstalledVersion(packageName);
                packageVersion.CurrentVersion = highestInstalledVersion == null?strings.not_available:highestInstalledVersion.InstalledVersion;
                packageVersion.IsCurrentVersionPreRelease = highestInstalledVersion != null && highestInstalledVersion.IsPreRelease;

                cancelToken.ThrowIfCancellationRequested();
                OnVersionChanged(packageVersion);
            }
            catch (OperationCanceledException ex)
            {
                this.Log().Error("Message: {0} - Stacktrac: {1}", ex.Message, ex.StackTrace);
            } // cancellation is expected and okay
        }

        private PackageVersion FillWithOData(string package)
        {
            this.Log().Debug("Filling data for {0}", package);
            var url = _sourceService.Source.Url + "/Packages?$filter=IsLatestVersion eq true and Id eq '" + package + "'";
            var xmlDoc = new XmlDocument();

            _loadingRssFeed = WebRequest.Create(url) as HttpWebRequest;
            _loadingRssFeed.Proxy = null;

            if (_loadingRssFeed != null)
            {
                Stream responseStream = null;
                try
                {
                    responseStream = _loadingRssFeed.GetResponse().GetResponseStream();
                    xmlDoc.Load(responseStream);
                    var packages = _versionXmlParser.parse(xmlDoc);
                    if (packages.Any())
                        return packages.First();
                }
                catch (XmlException ex) { this.Log().Error("Message: {0} - Stacktrac: {1}", ex.Message, ex.StackTrace); } // when xml could not be parsed
                catch (WebException ex) { this.Log().Error("Message: {0} - Stacktrac: {1}", ex.Message, ex.StackTrace); } // when loading xml from server failed
                finally
                {
                    if (responseStream != null)
                        responseStream.Close();
                }
            }

            var packageVersion = new PackageVersion {Summary = string.Format(strings.could_not_download, url)};
            packageVersion.Description = packageVersion.Summary;
            return packageVersion;
        }

        private void OnVersionChanged(PackageVersion version)
        {
            this.Log().Debug("Version changed: {0}", version);
            var handler = VersionChanged;
            if (handler != null) handler(version);
        }

        private void OnStarted(string packageName)
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler("Getting package " + packageName);
        }
    }
}
