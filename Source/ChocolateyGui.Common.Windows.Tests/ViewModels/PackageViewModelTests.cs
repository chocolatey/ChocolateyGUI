// <copyright file="PackageViewModelTests.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Threading;
using AutoMapper;
using Caliburn.Micro;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.ViewModels.Items;
using FluentAssertions;
using Moq;
using NuGet.Versioning;
using NUnit.Framework;

namespace ChocolateyGui.Common.Windows.Tests.ViewModels
{
    /// <summary>
    ///     Unit tests for how <see cref="PackageViewModel"/> handles <see cref="PackageOutdatedMessage"/>.
    ///     The outdated broadcast can only be matched by Id, so in an all-versions listing - where every
    ///     version of a package is a separate row sharing a single Id - the handler must also match on the
    ///     version. Otherwise every sibling row's version is overwritten with the latest and they collapse
    ///     into a single version (#1146). This is the path that the RemoteSourceViewModel tests cannot cover,
    ///     because they map results to a stubbed IPackageViewModel that has no message handling.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class PackageViewModelTests
    {
        private const string PackageId = "chocolateygui";

        [Test]
        public void HandlePackageOutdatedMessage_NonInstalledOlderVersionRow_LeavesVersionUntouched()
        {
            // A sibling row for an older version (another row of the same package in an all-versions listing)
            // must keep its own version rather than being rewritten to the latest one - this is #1146.
            var package = CreatePackage(version: "3.1.0", isInstalled: false);

            package.Handle(new PackageOutdatedMessage(PackageId, NuGetVersion.Parse("3.2.0"), source: null));

            package.Version.ToNormalizedString().Should().Be("3.1.0");
        }

        [Test]
        public void HandlePackageOutdatedMessage_NonInstalledLatestVersionRow_KeepsItsVersion()
        {
            var package = CreatePackage(version: "3.2.0", isInstalled: false);

            package.Handle(new PackageOutdatedMessage(PackageId, NuGetVersion.Parse("3.2.0"), source: null));

            package.Version.ToNormalizedString().Should().Be("3.2.0");
            package.LatestVersion.Should().BeNull();
        }

        [Test]
        public void HandlePackageOutdatedMessage_InstalledPackage_IsMarkedOutdatedWithoutChangingVersion()
        {
            var package = CreatePackage(version: "3.0.0", isInstalled: true);

            package.Handle(new PackageOutdatedMessage(PackageId, NuGetVersion.Parse("3.2.0"), source: null));

            package.Version.ToNormalizedString().Should().Be("3.0.0");
            package.LatestVersion.ToNormalizedString().Should().Be("3.2.0");
            package.IsOutdated.Should().BeTrue();
        }

        [Test]
        public void HandlePackageOutdatedMessage_DifferentId_IsIgnored()
        {
            var package = CreatePackage(version: "3.1.0", isInstalled: false);

            package.Handle(new PackageOutdatedMessage("some-other-package", NuGetVersion.Parse("3.2.0"), source: null));

            package.Version.ToNormalizedString().Should().Be("3.1.0");
        }

        private static PackageViewModel CreatePackage(string version, bool isInstalled)
        {
            var package = new PackageViewModel(
                Mock.Of<IChocolateyService>(),
                Mock.Of<IEventAggregator>(),
                Mock.Of<IMapper>(),
                Mock.Of<IDialogService>(),
                Mock.Of<IProgressService>(),
                Mock.Of<IChocolateyGuiCacheService>(),
                Mock.Of<IConfigService>(),
                Mock.Of<IAllowedCommandsService>(),
                Mock.Of<IPackageArgumentsService>(),
                Mock.Of<IPersistenceService>())
            {
                Id = PackageId,
                Version = NuGetVersion.Parse(version),
                IsInstalled = isInstalled,
            };

            return package;
        }
    }
}
