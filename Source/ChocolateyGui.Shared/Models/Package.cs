// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Package.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace ChocolateyGui.Models
{
    [DataContract]
    public class Package
    {
        [DataMember]
        public string[] Authors { get; set; }

        [DataMember]

        public string Copyright { get; set; }

        [DataMember]

        public string Dependencies { get; set; }

        [DataMember]

        public string Description { get; set; }

        [DataMember]

        public int DownloadCount { get; set; }

        [DataMember]

        public string GalleryDetailsUrl { get; set; }

        [DataMember]

        public string IconUrl { get; set; }

        [DataMember]

        public string Id { get; set; }

        [DataMember]

        public bool IsAbsoluteLatestVersion { get; set; }

        [DataMember]

        public bool IsInstalled { get; set; }

        [DataMember]

        public bool IsPinned { get; set; }

        [DataMember]

        public bool IsLatestVersion { get; set; }

        [DataMember]

        public bool IsPrerelease { get; set; }

        [DataMember]

        public string Language { get; set; }

        [DataMember]

        public string LatestVersion { get; set; }

        [DataMember]

        public string LicenseUrl { get; set; }

        [DataMember]

        public string[] Owners { get; set; }

        [DataMember]

        public string PackageHash { get; set; }

        [DataMember]

        public string PackageHashAlgorithm { get; set; }

        [DataMember]

        public long PackageSize { get; set; }

        [DataMember]

        public string ProjectUrl { get; set; }

        [DataMember]

        public DateTimeOffset Published { get; set; }

        [DataMember]

        public string ReleaseNotes { get; set; }

        [DataMember]

        public string ReportAbuseUrl { get; set; }

        [DataMember]

        public string RequireLicenseAcceptance { get; set; }

        [DataMember]

        public Uri Source { get; set; }

        [DataMember]

        public string Summary { get; set; }

        [DataMember]

        public string Tags { get; set; }

        [DataMember]

        public string Title { get; set; }

        [DataMember]

        public string Version { get; set; }

        [DataMember]

        public int VersionDownloadCount { get; set; }
    }
}