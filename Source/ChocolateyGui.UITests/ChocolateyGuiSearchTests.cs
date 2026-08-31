// <copyright file="ChocolateyGuiSearchTests.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Linq;
using System.Threading;
using FlaUI.TestUtilities;
using NUnit.Framework;

namespace ChocolateyGui.UITests
{
    /// <summary>
    ///     Progressive search tests against the mocked community.chocolatey.org feed with the package under
    ///     test NOT installed - the baseline "happy path" for the remote source view. The mocked feed
    ///     deliberately prefixes every package id/title with "mock-"/"Mock " so it is obvious in the UI and
    ///     assertions that this is fake data. The installed-state variant (issue #1146) lives in
    ///     <see cref="ChocolateyGuiInstalledPackageTests"/>.
    /// </summary>
    [TestFixture]
    public class ChocolateyGuiSearchTests : IsolatedRemoteSourceTestBase
    {
        private const string LATEST_VERSION = "3.2.0";
        private const int EXPECTED_DEFAULT_PACKAGE_COUNT = 30;
        private const int EXPECTED_SEARCH_MATCH_COUNT = 3;

        // Distinct versions the mocked feed returns for an all-versions, exact search (newest-first).
        private static readonly string[] ExpectedAllVersionsSample = { "3.2.0", "3.1.0", "3.0.1", "2.1.1" };

        protected override ApplicationStartMode ApplicationStartMode => ApplicationStartMode.OncePerFixture;

        // Uses the base PrepareInstalledState default => the package under test is NOT installed.
        [SetUp]
        public void Arrange()
        {
            OpenRemoteSourceAtBaseline();
        }

        [Test]
        public void Level1_OpeningRemoteSourceWithNoFilters_ShowsDefaultPackages()
        {
            var packageList = RemoteSourceScreen.GetPackageList();

            Assert.That(packageList, Has.Length.EqualTo(EXPECTED_DEFAULT_PACKAGE_COUNT));
        }

        [Test]
        public void Level2_SearchingForChocolateyGui_ReturnsMultipleMatchingPackages()
        {
            Search(SearchTerm);

            var packageList = RemoteSourceScreen.GetPackageList();

            Assert.That(packageList, Has.Length.EqualTo(EXPECTED_SEARCH_MATCH_COUNT));
        }

        [Test]
        public void Level3_SearchingForChocolateyGuiExactly_ReturnsSinglePackageWithLatestVersion()
        {
            // Search first, then narrow with the filter (matches the BetaPackageTests pattern and applies the
            // filter to the package search rather than the empty/default list).
            Search(SearchTerm);
            SetCheckBox(AutomationIds.MATCH_CHECK_BOX, true);

            var packageList = RemoteSourceScreen.GetPackageList();
            Assert.That(packageList, Has.Length.EqualTo(1));

            var targetPackage = packageList.First();
            targetPackage.Click();
            Thread.Sleep(100); // Without waiting, sometimes the double-click is missed.
            targetPackage.DoubleClick();

            // The package is not installed, so the single-result view shows the latest available version.
            var versionItem = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.VERSION_TEXT));
            Assert.That(versionItem.Name, Is.EqualTo(LATEST_VERSION));
        }

        [Test]
        public void Level4_SearchingExactlyWithAllVersions_ShowsDistinctVersionsNewestFirst()
        {
            Search(SearchTerm);
            SetCheckBox(AutomationIds.MATCH_CHECK_BOX, true);
            SetCheckBox(AutomationIds.ALL_VERSIONS_CHECK_BOX, true);

            var packageList = RemoteSourceScreen.GetPackageList();
            Assert.That(packageList, Has.Length.GreaterThan(1), "Expected multiple version rows for an all-versions search.");

            var displayedVersions = RemoteSourceScreen.GetDisplayedVersions();
            Assert.That(displayedVersions.Length, Is.GreaterThan(1), "Expected multiple distinct versions.");

            foreach (var expected in ExpectedAllVersionsSample)
            {
                Assert.That(
                    displayedVersions,
                    Contains.Item(expected),
                    $"Expected version {expected} to be displayed in the all-versions list.");
            }
        }
    }
}
