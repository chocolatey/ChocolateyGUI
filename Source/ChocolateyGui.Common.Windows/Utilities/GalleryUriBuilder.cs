// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="GalleryUriBuilder.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using chocolatey;
using NuGet.Versioning;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public static class GalleryUriBuilder
    {
        public static Uri BuildFromPackageAndSource(string packageId, string source, NuGetVersion version = null)
        {
            if (string.IsNullOrWhiteSpace(packageId) || string.IsNullOrWhiteSpace(source))
            {
                return null;
            }

            var uri = new UriBuilder(source)
            {
                Path = version == null ? $"packages/{packageId}" : $"packages/{packageId}/{version.OriginalVersion ?? version.ToFullStringChecked()}"
            };

            return uri.Uri;
        }

        public static Uri BuildCommunityGalleryFromPackage(string packageId, NuGetVersion version = null)
        {
            return BuildFromPackageAndSource(packageId, "https://community.chocolatey.org/", version);
        }

        public static bool IsKnownSource(string sourceUrl)
        {
            if (string.IsNullOrWhiteSpace(sourceUrl) || !Uri.TryCreate(sourceUrl, UriKind.Absolute, out var uri))
            {
                return false;
            }

            return uri.Host.Equals("chocolatey.org", StringComparison.OrdinalIgnoreCase)
                || uri.Host.Equals("community.chocolatey.org", StringComparison.OrdinalIgnoreCase)
                || uri.Host.Equals("community-test.chocolatey.org", StringComparison.OrdinalIgnoreCase);
        }
    }
}
