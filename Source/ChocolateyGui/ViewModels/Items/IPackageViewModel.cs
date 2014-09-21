// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IPackageViewModel.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.ViewModels.Items
{
    using ChocolateyGui.Models;
    using System;
    using System.Threading.Tasks;
    
    public interface IPackageViewModel
    {
        Task EnsureIsLoaded();

        Task Install();

        Task Update();

        Task Uninstall();

        Task RetriveLatestVersion();

        string Authors { get; set; }

        bool CanUpdate { get; }

        string Copyright { get; set; }

        DateTime Created { get; set; }

        string Dependencies { get; set; }

        string Description { get; set; }

        int DownloadCount { get; set; }

        string GalleryDetailsUrl { get; set; }

        string IconUrl { get; set; }

        string Id { get; set; }

        bool IsAbsoluteLatestVersion { get; set; }

        bool IsInstalled { get; }

        bool IsLatestVersion { get; set; }

        bool IsPrerelease { get; set; }

        string Language { get; set; }

        DateTime LastUpdated { get; set; }

        string LicenseUrl { get; set; }

        string Owners { get; set; }

        string PackageHash { get; set; }

        string PackageHashAlgorithm { get; set; }

        long PackageSize { get; set; }

        string ProjectUrl { get; set; }

        DateTime Published { get; set; }

        string ReleaseNotes { get; set; }

        string ReportAbuseUrl { get; set; }

        string RequireLicenseAcceptance { get; set; }

        string Summary { get; set; }

        string Tags { get; set; }

        string Title { get; set; }

        SemanticVersion Version { get; set; }

        SemanticVersion LatestVersion { get; }

        int VersionDownloadCount { get; set; }

        Uri Source { get; set; }
    }
}