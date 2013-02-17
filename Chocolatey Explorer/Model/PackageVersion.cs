using System;
using System.Collections.Generic;
using Chocolatey.Explorer.Services;

namespace Chocolatey.Explorer.Model
{
    public class PackageVersion
    {
        public string Name { get; set; }
        public string CurrentVersion { get; set; }
        public bool IsCurrentVersionPreRelease { get; set ; }
        public string Serverversion { get; set; }
        public bool IsInstalled { get { return !CurrentVersion.EndsWith(strings.not_available); } }
        public bool CanBeUpdated { get { return !Serverversion.EndsWith(strings.not_available) && IsLower() && IsInstalled; } }
        public string Summary { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string AuthorName { get; set; }
        public string CopyrightInformation { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public int DownloadCount { get; set; }
        public int VersionDownloadCount { get; set; }
        public string GalleryDetailsUrl { get; set; }
        public string IconUrl { get; set; }
        public bool IsPrerelease { get; set; }
        public string Language { get; set; }
        public DateTime PublishedAt { get; set; }
        public string LicenseUrl { get; set; }
        public UInt64 PackageSize { get; set; }
        public string ProjectUrl { get; set; }
        public string ReportAbuseUrl { get; set; }
        public string ReleaseNotes { get; set; }
        public bool RequireLicenseAcceptance { get; set; }
        public string[] Tags { get; set; }
        public string[] Dependencies { get; set; }

        private bool IsLower()
        {
            var packages = new List<String> {CurrentVersion, Serverversion};
            if (CurrentVersion.Equals(Serverversion)) return false;
            if (CurrentVersion.EndsWith(strings.not_available)) return false;
            packages.Sort(new PackagesSorter());
            return packages[0].Equals(CurrentVersion);
        }

        public PackageVersion()
        {
            Serverversion = strings.not_available;
            CurrentVersion = strings.not_available;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != typeof(PackageVersion)) return -1;
            return String.Compare(Name, ((PackageVersion)obj).Name, StringComparison.Ordinal);
        }

        public bool Equals(PackageVersion other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(PackageVersion) && Equals((PackageVersion)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        } 

        public override string ToString()
        {
            return "Name = " + Name + "\n" +
                "CurrentVersion = " + CurrentVersion + "\n" +
                "Serverversion = " + Serverversion + "\n" +
                "IsInstalled = " + IsInstalled + "\n" +
                "CanBeUpdated = " + CanBeUpdated + "\n" +
                "Summary = " + Summary + "\n" +
                "LastUpdatedAt = " + LastUpdatedAt + "\n" +
                "AuthorName = " + AuthorName + "\n" +
                "CopyrightInformation = " + CopyrightInformation + "\n" +
                "CreatedAt = " + CreatedAt + "\n" +
                "Description = " + Description + "\n" +
                "DownloadCount = " + DownloadCount + "\n" +
                "VersionDownloadCount = " + VersionDownloadCount + "\n" +
                "GalleryDetailsUrl = " + GalleryDetailsUrl + "\n" +
                "IconUrl = " + IconUrl + "\n" +
                "IsPrerelease = " + IsPrerelease + "\n" +
                "Language = " + Language + "\n" +
                "PublishedAt = " + PublishedAt + "\n" +
                "LicenseUrl = " + LicenseUrl + "\n" +
                "PackageSize = " + PackageSize + "\n" +
                "ProjectUrl = " + ProjectUrl + "\n" +
                "ReportAbuseUrl = " + ReportAbuseUrl + "\n" +
                "ReleaseNotes = " + ReleaseNotes + "\n" +
                "RequireLicenseAcceptance = " + RequireLicenseAcceptance + "\n" +
                "Tags = " + Tags + "\n" +
                "Dependencies = " + Dependencies + "\n";
        }
    }
}