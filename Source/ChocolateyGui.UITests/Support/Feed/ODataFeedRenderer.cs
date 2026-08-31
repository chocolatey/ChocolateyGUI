// <copyright file="ODataFeedRenderer.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Collections.Generic;
using System.Text;

namespace ChocolateyGui.UITests.Support.Feed
{
    /// <summary>
    ///     Renders a NuGet v2 OData (Atom) feed and its <c>/$count</c> companion from a set of package
    ///     versions. The Atom <c>&lt;entry&gt;</c> envelope is almost entirely fixed - only a handful of
    ///     fields vary per version - so this holds the fixed envelope once and substitutes the varying
    ///     fields, keeping every response derivable from the <see cref="MockFeedPackage" /> data.
    /// </summary>
    public class ODataFeedRenderer
    {
        private readonly string _baseUrl;
        private readonly string _iconUrl;

        /// <param name="baseUrl">The feed root the served source points at, e.g. <c>http://localhost:9111/api/v2/</c>.</param>
        /// <param name="iconUrl">The icon URL to advertise for every entry (served by the same mock host).</param>
        public ODataFeedRenderer(string baseUrl, string iconUrl)
        {
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _iconUrl = iconUrl;
        }

        public string RenderFeed(IEnumerable<FeedEntry> entries)
        {
            var builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>\n");
            builder.Append("<feed xml:base=\"").Append(_baseUrl).Append("\" ");
            builder.Append("xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" ");
            builder.Append("xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\" ");
            builder.Append("xmlns=\"http://www.w3.org/2005/Atom\">\n");
            builder.Append("  <title type=\"text\">Search</title>\n");
            builder.Append("  <id>").Append(_baseUrl).Append("Search</id>\n");
            builder.Append("  <updated>2026-01-01T00:00:00Z</updated>\n");
            builder.Append("  <link rel=\"self\" title=\"Search\" href=\"Search\" />\n");

            foreach (var entry in entries)
            {
                AppendEntry(builder, entry);
            }

            builder.Append("</feed>\n");
            return builder.ToString();
        }

        public string RenderCount(int count)
        {
            return count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private void AppendEntry(StringBuilder builder, FeedEntry entry)
        {
            var package = entry.Package;
            var version = entry.Version.Version;
            var id = package.Id;
            var packageRef = "Packages(Id='" + id + "',Version='" + version + "')";

            builder.Append("  <entry>\n");
            builder.Append("    <id>").Append(_baseUrl).Append(packageRef).Append("</id>\n");
            builder.Append("    <title type=\"text\">").Append(Escape(id)).Append("</title>\n");
            builder.Append("    <summary type=\"text\">").Append(Escape(package.Summary)).Append("</summary>\n");
            builder.Append("    <updated>2026-01-01T00:00:00Z</updated>\n");
            builder.Append("    <author><name>Chocolatey</name></author>\n");
            builder.Append("    <link rel=\"edit-media\" title=\"V2FeedPackage\" href=\"").Append(packageRef).Append("/$value\" />\n");
            builder.Append("    <link rel=\"edit\" title=\"V2FeedPackage\" href=\"").Append(packageRef).Append("\" />\n");
            builder.Append("    <category term=\"CCR.Website.V2FeedPackage\" scheme=\"http://schemas.microsoft.com/ado/2007/08/dataservices/scheme\" />\n");
            builder.Append("    <content type=\"application/zip\" src=\"").Append(_baseUrl).Append("package/").Append(id).Append("/").Append(version).Append("\" />\n");
            builder.Append("    <m:properties>\n");
            builder.Append("      <d:Version>").Append(version).Append("</d:Version>\n");
            builder.Append("      <d:Title>").Append(Escape(package.Title)).Append("</d:Title>\n");
            builder.Append("      <d:Description>").Append(Escape(package.Summary)).Append("</d:Description>\n");
            builder.Append("      <d:Tags>chocolatey mock</d:Tags>\n");
            builder.Append("      <d:Copyright></d:Copyright>\n");
            builder.Append("      <d:Created m:type=\"Edm.DateTime\">2026-01-01T00:00:00</d:Created>\n");
            builder.Append("      <d:Dependencies></d:Dependencies>\n");
            builder.Append("      <d:DownloadCount m:type=\"Edm.Int32\">").Append(entry.DownloadCount).Append("</d:DownloadCount>\n");
            builder.Append("      <d:VersionDownloadCount m:type=\"Edm.Int32\">1</d:VersionDownloadCount>\n");
            builder.Append("      <d:GalleryDetailsUrl>").Append(_baseUrl).Append("packages/").Append(id).Append("/").Append(version).Append("</d:GalleryDetailsUrl>\n");
            builder.Append("      <d:ReportAbuseUrl>").Append(_baseUrl).Append("package/ReportAbuse/").Append(id).Append("/").Append(version).Append("</d:ReportAbuseUrl>\n");
            builder.Append("      <d:IconUrl>").Append(_iconUrl).Append("</d:IconUrl>\n");
            builder.Append("      <d:IsLatestVersion m:type=\"Edm.Boolean\">").Append(Bool(entry.IsLatestVersion)).Append("</d:IsLatestVersion>\n");
            builder.Append("      <d:IsAbsoluteLatestVersion m:type=\"Edm.Boolean\">").Append(Bool(entry.IsAbsoluteLatestVersion)).Append("</d:IsAbsoluteLatestVersion>\n");
            builder.Append("      <d:IsPrerelease m:type=\"Edm.Boolean\">").Append(Bool(entry.Version.IsPrerelease)).Append("</d:IsPrerelease>\n");
            builder.Append("      <d:Language></d:Language>\n");
            builder.Append("      <d:Published m:type=\"Edm.DateTime\">2026-01-01T00:00:00</d:Published>\n");
            builder.Append("      <d:LicenseUrl></d:LicenseUrl>\n");
            builder.Append("      <d:RequireLicenseAcceptance m:type=\"Edm.Boolean\">false</d:RequireLicenseAcceptance>\n");
            builder.Append("      <d:PackageHash>0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000==</d:PackageHash>\n");
            builder.Append("      <d:PackageHashAlgorithm>SHA512</d:PackageHashAlgorithm>\n");
            builder.Append("      <d:PackageSize m:type=\"Edm.Int64\">1024</d:PackageSize>\n");
            builder.Append("      <d:ProjectUrl></d:ProjectUrl>\n");
            builder.Append("      <d:ReleaseNotes></d:ReleaseNotes>\n");
            builder.Append("      <d:IsApproved m:type=\"Edm.Boolean\">true</d:IsApproved>\n");
            builder.Append("      <d:PackageStatus>Approved</d:PackageStatus>\n");
            builder.Append("      <d:IsDownloadCacheAvailable m:type=\"Edm.Boolean\">false</d:IsDownloadCacheAvailable>\n");
            builder.Append("      <d:PackageScanStatus>NotFlagged</d:PackageScanStatus>\n");
            builder.Append("      <d:PackageScanFlagResult>None</d:PackageScanFlagResult>\n");
            builder.Append("    </m:properties>\n");
            builder.Append("  </entry>\n");
        }

        private static string Bool(bool value)
        {
            return value ? "true" : "false";
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }
    }

    /// <summary>A single package version to render, with the OData "latest" flags already resolved.</summary>
    public class FeedEntry
    {
        public FeedEntry(MockFeedPackage package, MockFeedVersion version, int downloadCount)
        {
            Package = package;
            Version = version;
            DownloadCount = downloadCount;

            var latestStable = package.LatestStable;
            var latestAbsolute = package.LatestAbsolute;
            IsLatestVersion = latestStable != null && latestStable.Version == version.Version;
            IsAbsoluteLatestVersion = latestAbsolute != null && latestAbsolute.Version == version.Version;
        }

        public MockFeedPackage Package { get; }

        public MockFeedVersion Version { get; }

        public int DownloadCount { get; }

        public bool IsLatestVersion { get; }

        public bool IsAbsoluteLatestVersion { get; }
    }
}
