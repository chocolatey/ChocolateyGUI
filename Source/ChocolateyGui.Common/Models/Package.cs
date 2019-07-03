// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Package.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NuGet;

namespace ChocolateyGui.Common.Models
{
    public class Package
    {
        public string[] Authors { get; set; }

        public string Copyright { get; set; }

        public string Dependencies { get; set; }

        public string Description { get; set; }

        public int DownloadCount { get; set; }

        public string GalleryDetailsUrl { get; set; }

        public string IconUrl { get; set; }

        public string Id { get; set; }

        public bool IsAbsoluteLatestVersion { get; set; }

        public bool IsInstalled { get; set; }

        public bool IsPinned { get; set; }

        public bool IsSideBySide { get; set; }

        public bool IsLatestVersion { get; set; }

        public bool IsPrerelease { get; set; }

        public string Language { get; set; }

        public string LatestVersion { get; set; }

        public string LicenseUrl { get; set; }

        public string[] Owners { get; set; }

        public string PackageHash { get; set; }

        public string PackageHashAlgorithm { get; set; }

        public long PackageSize { get; set; }

        public string ProjectUrl { get; set; }

        public DateTimeOffset Published { get; set; }

        public string ReleaseNotes { get; set; }

        public string ReportAbuseUrl { get; set; }

        public string RequireLicenseAcceptance { get; set; }

        public Uri Source { get; set; }

        public string Summary { get; set; }

        public string Tags { get; set; }

        public string Title { get; set; }

        public SemanticVersion Version { get; set; }

        public int VersionDownloadCount { get; set; }
    }
}