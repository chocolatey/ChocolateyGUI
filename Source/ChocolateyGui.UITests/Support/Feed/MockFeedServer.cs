// <copyright file="MockFeedServer.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Types;
using WireMock.Util;

namespace ChocolateyGui.UITests.Support.Feed
{
    /// <summary>
    ///     A single mocked NuGet v2 OData feed, served over plain HTTP on a random localhost port by
    ///     WireMock.Net. One server represents one Chocolatey source; standing up several (each with its own
    ///     dataset) models several sources returning different data for the same package - the scenario the
    ///     prerelease/all-versions FlaUI tests (and issue #1109's multi-source matrix) need.
    ///     <para>
    ///     Every OData response is computed from the <see cref="MockFeedPackage" /> dataset by inspecting the
    ///     query the Chocolatey CLI/Chocolatey.Lib emits (endpoint + <c>$filter</c> + <c>searchTerm</c> +
    ///     <c>id</c>), rather than being a canned body per scenario. Because those exact query shapes are built
    ///     inside Chocolatey.Lib (not this repo), every request is recorded in <see cref="RequestLog" /> so a
    ///     calibration run can confirm the matchers.
    ///     </para>
    /// </summary>
    public class MockFeedServer : IDisposable
    {
        private static readonly Regex QuotedValue = new Regex("'([^']*)'", RegexOptions.Compiled);

        private readonly IList<MockFeedPackage> _packages;
        private readonly string _metadataXml;
        private readonly string _serviceRootXml;
        private readonly string _iconSvg;
        private readonly List<string> _requestLog = new List<string>();

        private WireMockServer _server;
        private ODataFeedRenderer _renderer;

        /// <param name="packages">The dataset this source answers from.</param>
        /// <param name="fixturesDirectory">
        ///     Directory holding the static <c>metadata.xml</c> and <c>service-root.xml</c> (reused verbatim from
        ///     the existing Dev Proxy fixtures - they are host-agnostic OData schema/service documents).
        /// </param>
        public MockFeedServer(IEnumerable<MockFeedPackage> packages, string fixturesDirectory)
        {
            _packages = packages.ToList();
            _metadataXml = File.ReadAllText(Path.Combine(fixturesDirectory, "metadata.xml"));
            _serviceRootXml = File.ReadAllText(Path.Combine(fixturesDirectory, "service-root.xml"));

            var iconPath = Path.Combine(fixturesDirectory, "mock-icon.svg");
            _iconSvg = File.Exists(iconPath)
                ? File.ReadAllText(iconPath)
                : "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"1\" height=\"1\" />";
        }

        /// <summary>The source URL to register in Chocolatey's config once <see cref="Start" /> has run.</summary>
        public string SourceUrl { get; private set; }

        /// <summary>Every request this server received, in order, for calibration/diagnostics.</summary>
        public IReadOnlyList<string> RequestLog
        {
            get { return _requestLog; }
        }

        public void Start()
        {
            _server = WireMockServer.Start();
            SourceUrl = string.Format("http://localhost:{0}/api/v2/", _server.Port);
            _renderer = new ODataFeedRenderer(SourceUrl, SourceUrl + "mock-icon.svg");

            _server
                .Given(Request.Create().UsingGet())
                .RespondWith(Response.Create().WithCallback(Dispatch));
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Stop();
                _server.Dispose();
                _server = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }

        /// <summary>Dumps the recorded request log to a file (used by the test base for calibration).</summary>
        public void WriteRequestLog(string path)
        {
            File.WriteAllLines(path, _requestLog);
        }

        private ResponseMessage Dispatch(IRequestMessage request)
        {
            var path = request.Path ?? string.Empty;
            var url = request.Url ?? string.Empty;
            _requestLog.Add(request.Method + " " + url);

            if (path.EndsWith("mock-icon.svg", StringComparison.OrdinalIgnoreCase))
            {
                return Build(200, "image/svg+xml", _iconSvg);
            }

            if (path.EndsWith("$metadata", StringComparison.OrdinalIgnoreCase))
            {
                return Build(200, "application/xml;charset=utf-8", _metadataXml);
            }

            var isCount = path.EndsWith("/$count", StringComparison.OrdinalIgnoreCase);
            var query = ParseQuery(url);
            var filter = query.ContainsKey("$filter") ? query["$filter"] : string.Empty;

            var isFindById = path.IndexOf("FindPackagesById", StringComparison.OrdinalIgnoreCase) >= 0;
            var isPackages = path.IndexOf("Packages()", StringComparison.OrdinalIgnoreCase) >= 0;
            var isSearch = path.IndexOf("Search()", StringComparison.OrdinalIgnoreCase) >= 0;

            // No OData operation in the path -> the client is fetching the service document.
            if (!isCount && !isFindById && !isPackages && !isSearch)
            {
                return Build(200, "application/xml;charset=utf-8", _serviceRootXml);
            }

            var includePrerelease = DetermineIncludePrerelease(query, filter);
            List<FeedEntry> entries;

            if (isFindById)
            {
                // FindPackagesById returns EVERY version of the id - the calibration run shows the query carries
                // no $filter and no includePrerelease, because Chocolatey.Lib filters prereleases client-side.
                // This drives both the package-details version list and the --all-versions listing (which expands
                // each Search() hit via FindPackagesById), so the full set must come back and let the client filter.
                var id = Unquote(query.ContainsKey("id") ? query["id"] : ExtractIdFromFilter(filter));
                entries = AllVersionEntries(id, includePrerelease: true);
            }
            else if (isPackages)
            {
                entries = LatestEntries(Unquote(ExtractIdFromFilter(filter)), includePrerelease);
            }
            else
            {
                var term = Unquote(query.ContainsKey("searchTerm") ? query["searchTerm"] : string.Empty);
                entries = SearchEntries(term, includePrerelease);
            }

            return isCount
                ? Build(200, "text/plain", _renderer.RenderCount(entries.Count))
                : Build(200, "application/atom+xml;charset=utf-8", _renderer.RenderFeed(entries));
        }

        // Chocolatey.Lib signals "include prerelease" by filtering on IsAbsoluteLatestVersion (prerelease) vs
        // IsLatestVersion (stable). This is the matcher most likely to need confirming from a calibration run -
        // hence the request log.
        private static bool DetermineIncludePrerelease(IDictionary<string, string> query, string filter)
        {
            if (query.ContainsKey("includePrerelease") &&
                string.Equals(query["includePrerelease"], "true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (filter.IndexOf("IsAbsoluteLatestVersion", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        // Chocolatey.Lib's search always applies an IsLatestVersion (stable) or IsAbsoluteLatestVersion
        // (prerelease) $filter - even with --all-versions, where the calibration log shows it uses the Search()
        // result only to discover the matching package and then expands every version via FindPackagesById. So a
        // Search() response is always the single latest row per matching package.
        private List<FeedEntry> SearchEntries(string term, bool includePrerelease)
        {
            var matches = _packages.Where(p => Matches(p, term));
            var entries = new List<FeedEntry>();
            var rank = matches.Count();

            foreach (var package in matches)
            {
                var version = package.Latest(includePrerelease);
                if (version != null)
                {
                    entries.Add(new FeedEntry(package, version, rank * 1000));
                    rank--;
                }
            }

            return entries;
        }

        private List<FeedEntry> LatestEntries(string id, bool includePrerelease)
        {
            var package = FindById(id);
            var version = package == null ? null : package.Latest(includePrerelease);
            return version == null
                ? new List<FeedEntry>()
                : new List<FeedEntry> { new FeedEntry(package, version, 1000) };
        }

        private List<FeedEntry> AllVersionEntries(string id, bool includePrerelease)
        {
            var package = FindById(id);
            if (package == null)
            {
                return new List<FeedEntry>();
            }

            return package
                .AllVersions(includePrerelease)
                .Select(version => new FeedEntry(package, version, 1000))
                .ToList();
        }

        private static bool Matches(MockFeedPackage package, string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return true;
            }

            return package.Id.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0
                || (package.Title != null && package.Title.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private MockFeedPackage FindById(string id)
        {
            return string.IsNullOrEmpty(id)
                ? null
                : _packages.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        private static string ExtractIdFromFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return string.Empty;
            }

            var match = QuotedValue.Match(filter);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static string Unquote(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Trim().Trim('\'');
        }

        private static IDictionary<string, string> ParseQuery(string url)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var questionMark = url.IndexOf('?');
            if (questionMark < 0 || questionMark == url.Length - 1)
            {
                return result;
            }

            var queryString = url.Substring(questionMark + 1);
            foreach (var pair in queryString.Split('&'))
            {
                if (pair.Length == 0)
                {
                    continue;
                }

                var equals = pair.IndexOf('=');
                var key = equals < 0 ? pair : pair.Substring(0, equals);
                var value = equals < 0 ? string.Empty : pair.Substring(equals + 1);
                result[Uri.UnescapeDataString(key)] = Uri.UnescapeDataString(value);
            }

            return result;
        }

        private static ResponseMessage Build(int statusCode, string contentType, string body)
        {
            return new ResponseMessage
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, WireMockList<string>>
                {
                    { "Content-Type", new WireMockList<string>(contentType) },
                },
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = body,
                    Encoding = Encoding.UTF8,
                },
            };
        }
    }
}
