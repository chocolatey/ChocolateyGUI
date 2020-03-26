// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IPackageViewModel.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NuGet;

namespace ChocolateyGui.Common.ViewModels.Items
{
    public interface IPackageViewModel
    {
        string[] Authors { get; set; }

        bool CanUpdate { get; }

        string Copyright { get; set; }

        string Dependencies { get; set; }

        string Description { get; set; }

        int DownloadCount { get; set; }

        string GalleryDetailsUrl { get; set; }

        string IconUrl { get; set; }

        string Id { get; set; }

        bool IsAbsoluteLatestVersion { get; set; }

        bool IsInstalled { get; set; }

        bool IsPinned { get; set; }

        bool IsSideBySide { get; set; }

        bool IsLatestVersion { get; set; }

        bool IsPrerelease { get; set; }

        string Language { get; set; }

        SemanticVersion LatestVersion { get; }

        string LicenseUrl { get; set; }

        string[] Owners { get; set; }

        string PackageHash { get; set; }

        string PackageHashAlgorithm { get; set; }

        long PackageSize { get; set; }

        string ProjectUrl { get; set; }

        DateTimeOffset Published { get; set; }

        string ReleaseNotes { get; set; }

        string ReportAbuseUrl { get; set; }

        string RequireLicenseAcceptance { get; set; }

        Uri Source { get; set; }

        string Summary { get; set; }

        string Tags { get; set; }

        string Title { get; set; }

        SemanticVersion Version { get; set; }

        int VersionDownloadCount { get; set; }

        Task Install();

        Task Uninstall();

        Task Update();

        void ViewDetails();
    }
}