// <copyright file="RemoteSourceViewModelTests.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Utilities;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.Services;
using ChocolateyGui.Common.Windows.ViewModels;
using FluentAssertions;
using Moq;
using NuGet.Versioning;
using NUnit.Framework;

namespace ChocolateyGui.Common.Windows.Tests.ViewModels
{
    /// <summary>
    ///     Unit tests for <see cref="RemoteSourceViewModel.LoadPackages"/>, locking in the fix for issue
    ///     #1146 independently of the FlaUI / Dev Proxy path. The bug: with "all versions" enabled and the
    ///     package installed, every result row's version was overwritten with the single installed version.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class RemoteSourceViewModelTests
    {
        // RemoteSourceViewModel.LoadPackages maps results with the *static* AutoMapper.Mapper (this is how
        // the application wires it up in Bootstrapper.OnStartup), so the static mapper must be initialized
        // with a Package -> IPackageViewModel map. The real map resolves IPackageViewModel from the DI
        // container; here we construct a property-backed stub instead.
        [OneTimeSetUp]
        public void InitializeStaticMapper()
        {
            Mapper.Reset();
            Mapper.Initialize(config =>
                config.CreateMap<Package, IPackageViewModel>()
                    .ConstructUsing(_ => CreatePackageViewModelStub())
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                    .ForMember(d => d.Version, o => o.MapFrom(s => s.Version))
                    .ForMember(d => d.IsInstalled, o => o.MapFrom(s => s.IsInstalled))
                    .ForMember(d => d.IsPinned, o => o.MapFrom(s => s.IsPinned))
                    .ForAllOtherMembers(o => o.Ignore()));
        }

        [OneTimeTearDown]
        public void ResetStaticMapper()
        {
            Mapper.Reset();
        }

        [Test]
        public void LoadPackages_AllVersions_WithInstalledPackage_KeepsDistinctRemoteVersions()
        {
            var chocolateyService = BuildChocolateyService(
                remote: new[]
                {
                    Package("chocolateygui", "3.2.0"),
                    Package("chocolateygui", "3.1.0"),
                    Package("chocolateygui", "3.0.0"),
                },
                installed: new[] { Package("chocolateygui", "3.0.0", isInstalled: true) });

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: true);

            viewModel.LoadPackages(false).GetAwaiter().GetResult();

            var displayedVersions = viewModel.Packages.Select(p => p.Version.ToNormalizedString()).ToArray();

            // #1146: each row must keep its own remote version, not collapse to the installed 3.0.0.
            displayedVersions.Should().BeEquivalentTo(new[] { "3.2.0", "3.1.0", "3.0.0" });

            // Only the row whose version is actually installed should be flagged installed.
            viewModel.Packages.Where(p => p.IsInstalled).Select(p => p.Version.ToNormalizedString())
                .Should().Equal("3.0.0");
        }

        [Test]
        public void LoadPackages_AllVersions_OrdersEachPackagesVersionsNewestFirst()
        {
            // The source returns versions in an arbitrary (popularity) order; the all-versions view should
            // present each package's versions newest-first.
            var chocolateyService = BuildChocolateyService(
                remote: new[]
                {
                    Package("chocolateygui", "1.1.0"),
                    Package("chocolateygui", "3.2.0"),
                    Package("chocolateygui", "2.0.0"),
                    Package("chocolateygui", "3.0.0"),
                },
                installed: System.Array.Empty<Package>());

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: true);

            viewModel.LoadPackages(false).GetAwaiter().GetResult();

            viewModel.Packages.Select(p => p.Version.ToNormalizedString())
                .Should().Equal("3.2.0", "3.0.0", "2.0.0", "1.1.0");
        }

        [Test]
        public void LoadPackages_LatestOnly_WithInstalledPackage_ShowsInstalledVersion()
        {
            // Regression guard for the #1109 behaviour: in the latest-only view an installed package's row
            // shows the installed version (with the remote version kept separately in RemoteVersion).
            var chocolateyService = BuildChocolateyService(
                remote: new[] { Package("chocolateygui", "3.2.0") },
                installed: new[] { Package("chocolateygui", "3.0.0", isInstalled: true) });

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: false);

            viewModel.LoadPackages(false).GetAwaiter().GetResult();

            viewModel.Packages.Should().HaveCount(1);
            viewModel.Packages[0].Version.ToNormalizedString().Should().Be("3.0.0");
            viewModel.Packages[0].RemoteVersion.ToNormalizedString().Should().Be("3.2.0");
            viewModel.Packages[0].IsInstalled.Should().BeTrue();
        }

        [Test]
        public void LoadPackages_PrefetchesTwoAdditionalPages()
        {
            var chocolateyService = BuildChocolateyService(
                remote: new[]
                {
                    Package("alpha", "1.0.0"),
                    Package("bravo", "1.0.0"),
                    Package("charlie", "1.0.0"),
                    Package("delta", "1.0.0"),
                    Package("echo", "1.0.0"),
                    Package("foxtrot", "1.0.0"),
                    Package("golf", "1.0.0"),
                    Package("hotel", "1.0.0"),
                },
                installed: Array.Empty<Package>());

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: false);
            viewModel.PageSize = 2;

            viewModel.LoadPackages(false).GetAwaiter().GetResult();

            // First page plus two prefetched pages, so the next icons are ready before the user reaches the bottom.
            viewModel.Packages.Select(p => p.Id).Should().Equal("alpha", "bravo", "charlie", "delta", "echo", "foxtrot");
            viewModel.HasMore.Should().BeTrue();
            viewModel.TotalCount.Should().Be(8);
            chocolateyService.Verify(
                s => s.Search(It.IsAny<string>(), It.IsAny<PackageSearchOptions>()),
                Times.Exactly(3));
            chocolateyService.Verify(s => s.GetInstalledPackages(), Times.Once);
        }

        [Test]
        public void LoadMorePackages_AppendsNextBatch_WithoutClearing()
        {
            var chocolateyService = BuildChocolateyService(
                remote: new[]
                {
                    Package("alpha", "1.0.0"),
                    Package("bravo", "1.0.0"),
                    Package("charlie", "1.0.0"),
                    Package("delta", "1.0.0"),
                    Package("echo", "1.0.0"),
                    Package("foxtrot", "1.0.0"),
                    Package("golf", "1.0.0"),
                    Package("hotel", "1.0.0"),
                },
                installed: Array.Empty<Package>());

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: false);
            viewModel.PageSize = 2;

            viewModel.LoadPackages(false).GetAwaiter().GetResult();
            viewModel.Packages.Should().HaveCount(6);

            viewModel.LoadMorePackages().GetAwaiter().GetResult();

            viewModel.Packages.Select(p => p.Id).Should().Equal(
                "alpha", "bravo", "charlie", "delta", "echo", "foxtrot", "golf", "hotel");
            viewModel.LoadedCount.Should().Be(8);

            // A full final page cannot prove the feed is exhausted; the next fetch is empty.
            viewModel.HasMore.Should().BeTrue();
            viewModel.LoadMorePackages().GetAwaiter().GetResult();
            viewModel.HasMore.Should().BeFalse();
            viewModel.LoadedCount.Should().Be(8);

            chocolateyService.Verify(s => s.GetInstalledPackages(), Times.Once);
        }

        [Test]
        public void LoadMorePackages_WhenLastBatchIsShort_DoesNotFetchAgain()
        {
            var chocolateyService = BuildChocolateyService(
                remote: new[]
                {
                    Package("alpha", "1.0.0"),
                    Package("bravo", "1.0.0"),
                    Package("charlie", "1.0.0"),
                },
                installed: Array.Empty<Package>());

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: false);
            viewModel.PageSize = 2;

            viewModel.LoadPackages(false).GetAwaiter().GetResult();
            viewModel.LoadMorePackages().GetAwaiter().GetResult();

            chocolateyService.Verify(
                s => s.Search(It.IsAny<string>(), It.IsAny<PackageSearchOptions>()),
                Times.Exactly(2));
            viewModel.Packages.Should().HaveCount(3);
            viewModel.HasMore.Should().BeFalse();
        }

        [Test]
        public void LoadPackages_AfterAppend_ReplacesPreviouslyLoadedItems()
        {
            var chocolateyService = BuildChocolateyService(
                remote: new[]
                {
                    Package("alpha", "1.0.0"),
                    Package("bravo", "1.0.0"),
                    Package("charlie", "1.0.0"),
                    Package("delta", "1.0.0"),
                    Package("echo", "1.0.0"),
                    Package("foxtrot", "1.0.0"),
                    Package("golf", "1.0.0"),
                    Package("hotel", "1.0.0"),
                    Package("india", "1.0.0"),
                    Package("juliet", "1.0.0"),
                },
                installed: Array.Empty<Package>());

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: false);
            viewModel.PageSize = 2;

            viewModel.LoadPackages(false).GetAwaiter().GetResult();
            viewModel.LoadMorePackages().GetAwaiter().GetResult();
            viewModel.Packages.Should().HaveCount(8);

            viewModel.LoadPackages(false).GetAwaiter().GetResult();

            viewModel.Packages.Select(p => p.Id).Should().Equal("alpha", "bravo", "charlie", "delta", "echo", "foxtrot");
            viewModel.HasMore.Should().BeTrue();
        }

        [Test]
        public void LoadPackages_WhenTotalCountIsUnknown_DoesNotShowNegativeTotal()
        {
            var chocolateyService = new Mock<IChocolateyService>();
            chocolateyService.Setup(s => s.Search(It.IsAny<string>(), It.IsAny<PackageSearchOptions>()))
                .Returns((string _, PackageSearchOptions options) =>
                {
                    var packages = options.CurrentPage == 0
                        ? new[] { Package("alpha", "1.0.0") }
                        : Array.Empty<Package>();
                    return Task.FromResult(new PackageResults
                    {
                        Packages = packages,
                        TotalCount = -1,
                    });
                });
            chocolateyService.Setup(s => s.GetInstalledPackages())
                .ReturnsAsync(Array.Empty<Package>().AsEnumerable());
            chocolateyService.Setup(s => s.GetOutdatedPackages(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<ChocolateySource>()))
                .ReturnsAsync((IReadOnlyList<OutdatedPackage>)new List<OutdatedPackage>());

            var viewModel = BuildViewModel(chocolateyService, includeAllVersions: true);

            viewModel.LoadPackages(false).GetAwaiter().GetResult();

            viewModel.Packages.Should().HaveCount(1);
            viewModel.TotalCount.Should().Be(1);
            viewModel.HasMore.Should().BeFalse();
        }

        private static IPackageViewModel CreatePackageViewModelStub()
        {
            var packageViewModel = new Mock<IPackageViewModel>();
            packageViewModel.SetupAllProperties();
            return packageViewModel.Object;
        }

        private static Package Package(string id, string version, bool isInstalled = false, bool isPinned = false)
        {
            return new Package
            {
                Id = id,
                Version = NuGetVersion.Parse(version),
                IsInstalled = isInstalled,
                IsPinned = isPinned,
            };
        }

        private static Mock<IChocolateyService> BuildChocolateyService(Package[] remote, Package[] installed)
        {
            var service = new Mock<IChocolateyService>();

            service.Setup(s => s.Search(It.IsAny<string>(), It.IsAny<PackageSearchOptions>()))
                .Returns((string _, PackageSearchOptions options) =>
                {
                    var pageSize = options.PageSize > 0 ? options.PageSize : remote.Length;
                    var packages = remote.Skip(options.CurrentPage * pageSize).Take(pageSize).ToArray();
                    return Task.FromResult(new PackageResults
                    {
                        Packages = packages,
                        TotalCount = remote.Length
                    });
                });

            service.Setup(s => s.GetInstalledPackages())
                .ReturnsAsync(installed.AsEnumerable());

            service.Setup(s => s.GetOutdatedPackages(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<ChocolateySource>()))
                .ReturnsAsync((IReadOnlyList<OutdatedPackage>)new List<OutdatedPackage>());

            return service;
        }

        private static RemoteSourceViewModel BuildViewModel(Mock<IChocolateyService> chocolateyService, bool includeAllVersions)
        {
            var progressService = new Mock<IProgressService>();
            progressService.Setup(p => p.StartLoading(It.IsAny<string>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
            progressService.Setup(p => p.StopLoading()).Returns(Task.CompletedTask);

            var configService = new Mock<IConfigService>();
            configService.Setup(c => c.GetEffectiveConfiguration()).Returns(new AppConfiguration());

            // Use a real aggregator: the view model publishes messages (e.g. ResetScrollPositionMessage) but
            // implements no IHandle, so publishing is a no-op. This avoids depending on the exact Caliburn
            // publish extension/overload that a mock would have to satisfy.
            var eventAggregator = new EventAggregator();

            var source = new ChocolateySource { Id = "chocolatey", Value = "https://community.chocolatey.org/api/v2/" };

            // The view model maps via the static AutoMapper.Mapper (see InitializeStaticMapper), so the
            // injected IMapper is unused by LoadPackages.
            var viewModel = new RemoteSourceViewModel(
                chocolateyService.Object,
                Mock.Of<IDialogService>(),
                progressService.Object,
                Mock.Of<IChocolateyGuiCacheService>(),
                configService.Object,
                eventAggregator,
                source,
                Mock.Of<IMapper>(),
                TranslationSource.Instance);

            viewModel.IncludeAllVersions = includeAllVersions;
            SetActive(viewModel);
            return viewModel;
        }

        // LoadPackages early-returns unless the screen is active. Activating it would trigger its own
        // LoadPackages call and reactive subscriptions, so set IsActive directly for an isolated test.
        private static void SetActive(RemoteSourceViewModel viewModel)
        {
            var isActive = typeof(Screen).GetProperty(nameof(Screen.IsActive));
            isActive.GetSetMethod(nonPublic: true).Invoke(viewModel, new object[] { true });
        }
    }
}
