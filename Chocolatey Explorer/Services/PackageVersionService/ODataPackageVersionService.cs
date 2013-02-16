using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using Chocolatey.Explorer.Model;
using System.IO;

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
        public event PackageVersionService.VersionResult VersionChanged;
        public delegate void VersionResult(PackageVersion version);

        private readonly ISourceService _sourceService;
        private readonly IPackageVersionXMLParser _versionXmlParser;
        private readonly ChocolateyLibDirHelper _libDirHelper;
        private CancellationTokenSource _cancelTokenSource;
        private HttpWebRequest _loadingRssFeed;

        public ODataPackageVersionService(IPackageVersionXMLParser versionXmlParser, ISourceService sourceService)
        {
            this._versionXmlParser = versionXmlParser;
            this._sourceService = sourceService;
            this._libDirHelper = new ChocolateyLibDirHelper();
        }

        public void PackageVersion(string package)
        {
            this.Log().Info("Getting version of package: " + package);
            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
            }
            _cancelTokenSource = new CancellationTokenSource();
            _cancelTokenSource.Token.Register(new Action(OnPackageVersionThreadCancel));
            var thread = new Thread(() => PackageVersionThread(_cancelTokenSource.Token, package)) { IsBackground = true };
            thread.Start();
        }

        private void OnPackageVersionThreadCancel()
        {
            if (_loadingRssFeed != null)
            {
                _loadingRssFeed.Abort();
            }
        }

        private void PackageVersionThread(CancellationToken cancelToken, string packageNameObj)
        {
            try
            {
                var packageName = packageNameObj as string;
                var packageVersion = FillWithOData(packageName);

                // not found on server - use what we know
                if (packageVersion == null)
                {
                    packageVersion = new PackageVersion();
                }

                cancelToken.ThrowIfCancellationRequested();
                packageVersion.Name = packageName;
                packageVersion.CurrentVersion = _libDirHelper.GetHighestInstalledVersion(packageName);

                cancelToken.ThrowIfCancellationRequested();
                OnVersionChanged(packageVersion);
            }
            catch (OperationCanceledException) { } // cancellation is expected and okay
        }

        private PackageVersion FillWithOData(string package)
        {
            string url = _sourceService.Source + "/Packages?$filter=IsLatestVersion eq true and Id eq '" + package + "'";
            XmlDocument xmlDoc = new XmlDocument();

            _loadingRssFeed = WebRequest.Create(url) as HttpWebRequest;
            _loadingRssFeed.Proxy = null;

            if (_loadingRssFeed != null)
            {
                Stream responseStream = null;
                try
                {
                    responseStream = _loadingRssFeed.GetResponse().GetResponseStream();
                    xmlDoc.Load(responseStream);
                    IList<PackageVersion> packages = _versionXmlParser.parse(xmlDoc);
                    if (packages.Count() > 0)
                        return packages.First();
                }
                catch (XmlException) { } // when xml could not be parsed
                catch (WebException) { } // when loading xml from server failed
                finally
                {
                    if (responseStream != null)
                        responseStream.Close();
                }
            }

            var packageVersion = new PackageVersion();
            packageVersion.Summary = string.Format(strings.could_not_download, url);
            packageVersion.Description = packageVersion.Summary;
            return packageVersion;
        }

        private void OnVersionChanged(PackageVersion version)
        {
            var handler = VersionChanged;
            if (handler != null) handler(version);
        }
    }
}
