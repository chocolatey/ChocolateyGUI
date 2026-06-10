using System.Collections;
using ChocolateyGui.Common.Windows.Utilities;
using FluentAssertions;
using NUnit.Framework;

namespace ChocolateyGui.Common.Windows.Tests.Utilities
{
    [TestFixture]
    [TestOf(typeof(GalleryUriBuilder))]
    public class GalleryUriBuilderTests
    {
        public static IEnumerable EmptyValues => new[]
        {
            null,
            string.Empty,
            "     "
        };

        [TestCaseSource(nameof(EmptyValues))]
        public void BuildFromPackageAndSource_ReturnsNull_ForNullOrWhitespacePackageId(string packageId)
        {
            GalleryUriBuilder.BuildFromPackageAndSource(packageId, "https://community.chocolatey.org/").Should().BeNull();
        }

        [TestCaseSource(nameof(EmptyValues))]
        public void BuildFromPackageAndSource_ReturnsNull_ForNullOrWhitespaceSource(string source)
        {
            GalleryUriBuilder.BuildFromPackageAndSource("some-package", source).Should().BeNull();
        }

        [Test]
        public void BuildFromPackageAndSource_ReturnsValidUri_ForValidPackageIdAndSource()
        {
            var uri = GalleryUriBuilder.BuildFromPackageAndSource("some-package", "https://community.chocolatey.org/");
            uri.Should().NotBeNull();
            uri.ToString().Should().Be("https://community.chocolatey.org/packages/some-package");
        }

        [Test]
        public void BuildFromPackageAndSource_ReturnsValidUri_WithUnknownSource()
        {
            var uri = GalleryUriBuilder.BuildFromPackageAndSource("some-package", "https://myget.org/F/chocolatey/api/v2/");
            uri.Should().NotBeNull();
            uri.ToString().Should().Be("https://myget.org/packages/some-package");
        }

        [Test]
        public void BuildFromPackageAndSource_ReturnsValidUri_WithVersion()
        {
            var uri = GalleryUriBuilder.BuildFromPackageAndSource("some-package", "https://community.chocolatey.org/", new NuGet.Versioning.NuGetVersion(1, 2, 3));
            uri.Should().NotBeNull();
            uri.ToString().Should().Be("https://community.chocolatey.org/packages/some-package/1.2.3");
        }

        [Test]
        public void BuildFromPackageSource_ReturnsValidUri_WithPreReleaseVersion()
        {
            var uri = GalleryUriBuilder.BuildFromPackageAndSource("some-package", "https://community.chocolatey.org/", new NuGet.Versioning.NuGetVersion(1, 2, 3, "beta"));
            uri.Should().NotBeNull();
            uri.ToString().Should().Be("https://community.chocolatey.org/packages/some-package/1.2.3-beta");
        }

        [Test]
        public void IsKnownSource_ReturnsFalse_ForInvalidUri()
        {
            GalleryUriBuilder.IsKnownSource("not a valid uri").Should().BeFalse();
        }

        [TestCaseSource(nameof(EmptyValues))]
        public void IsKnownSource_ReturnsFalse_ForNullOrWhitespace(string source)
        {
            GalleryUriBuilder.IsKnownSource(source).Should().BeFalse();
        }

        [TestCase("https://blog.chocolatey.org/api/v2/")]
        [TestCase("http://myget.org/F/chocolatey/api/v2/")]
        public void IsKnownSource_ReturnsFalse_ForUnknownSources(string source)
        {
            GalleryUriBuilder.IsKnownSource(source).Should().BeFalse();
        }

        [TestCase("https://chocolatey.org/api/v2/")]
        [TestCase("https://community.chocolatey.org/api/v2/")]
        [TestCase("https://community-test.chocolatey.org/api/v2/")]
        public void IsKnownSource_ReturnsTrue_ForKnownSources(string source)
        {
            GalleryUriBuilder.IsKnownSource(source).Should().BeTrue();
        }
    }
}