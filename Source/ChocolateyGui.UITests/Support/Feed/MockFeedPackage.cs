// <copyright file="MockFeedPackage.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace ChocolateyGui.UITests.Support.Feed
{
    /// <summary>
    ///     A single package (one Id) in a mocked OData feed, with all of the versions the feed should
    ///     report for it. This is the single source of truth a <see cref="MockFeedServer" /> answers from -
    ///     every OData response (search, exact match, all-versions, counts) is computed from this, rather
    ///     than hand-authored per scenario.
    /// </summary>
    public class MockFeedPackage
    {
        public MockFeedPackage(string id, string title, string summary, IEnumerable<string> versions)
        {
            Id = id;
            Title = title;
            Summary = summary;
            Versions = versions.Select(v => new MockFeedVersion(v)).ToList();
        }

        public string Id { get; }

        public string Title { get; }

        public string Summary { get; }

        /// <summary>Every version the feed knows about for this package, in no particular order.</summary>
        public IList<MockFeedVersion> Versions { get; }

        /// <summary>The highest stable (non-prerelease) version, or <c>null</c> if the package has none.</summary>
        public MockFeedVersion LatestStable
        {
            get
            {
                return Versions
                    .Where(v => !v.IsPrerelease)
                    .OrderByDescending(v => v.Parsed, VersionComparer.Default)
                    .FirstOrDefault();
            }
        }

        /// <summary>The highest version of any kind, including prereleases (OData's IsAbsoluteLatestVersion).</summary>
        public MockFeedVersion LatestAbsolute
        {
            get
            {
                return Versions
                    .OrderByDescending(v => v.Parsed, VersionComparer.Default)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        ///     The versions to show for an "all versions" listing, newest first. When prereleases are not
        ///     requested only the stable versions are returned.
        /// </summary>
        public IList<MockFeedVersion> AllVersions(bool includePrerelease)
        {
            return Versions
                .Where(v => includePrerelease || !v.IsPrerelease)
                .OrderByDescending(v => v.Parsed, VersionComparer.Default)
                .ToList();
        }

        /// <summary>The single version a "latest" query should return for the given prerelease preference.</summary>
        public MockFeedVersion Latest(bool includePrerelease)
        {
            return includePrerelease ? LatestAbsolute : LatestStable;
        }
    }

    /// <summary>One version of a <see cref="MockFeedPackage" />.</summary>
    public class MockFeedVersion
    {
        public MockFeedVersion(string version)
        {
            Version = version;
            Parsed = NuGetVersion.Parse(version);
        }

        /// <summary>The version exactly as authored in the dataset - what the feed reports and the UI displays.</summary>
        public string Version { get; }

        /// <summary>
        ///     The same version parsed with the NuGet.Versioning type Chocolatey.Lib itself uses, so ordering and
        ///     prerelease detection match the real client rather than a hand-rolled approximation.
        /// </summary>
        public NuGetVersion Parsed { get; }

        /// <summary>A version is a prerelease when it carries a SemVer pre-release label (e.g. <c>2.1.0-beta-1</c>).</summary>
        public bool IsPrerelease
        {
            get { return Parsed.IsPrerelease; }
        }
    }
}
