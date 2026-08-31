// <copyright file="ChocolateyGuiInstalledPackageTests.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Linq;
using System.Threading;
using ChocolateyGui.UITests.Support;
using FlaUI.TestUtilities;
using NUnit.Framework;

namespace ChocolateyGui.UITests
{
    /// <summary>
    ///     Remote source tests with the package under test INSTALLED. This is where issue #1146 reproduces:
    ///     with the package installed at <see cref="InstalledVersion"/>, an all-versions exact search must
    ///     still show the distinct remote versions, NOT the installed version repeated on every row.
    /// </summary>
    [TestFixture]
    public class ChocolateyGuiInstalledPackageTests : IsolatedRemoteSourceTestBase
    {
        private const string InstalledVersion = "3.0.0";

        // Distinct remote versions the mocked feed returns; with the #1146 bug present NONE of these show
        // (every row is overwritten with the installed 3.0.0), after the fix they do.
        private static readonly string[] ExpectedAllVersionsSample = { "3.2.0", "3.1.0", "3.0.1", "2.1.1" };

        protected override ApplicationStartMode ApplicationStartMode => ApplicationStartMode.OncePerFixture;

        // Install the package fresh before the application launches (remove any leftover first so the state is
        // deterministic and idempotent regardless of what a previous run left behind).
        protected override void PrepareInstalledState()
        {
            IsolatedChocolateyEnvironment.RemoveInstalledPackage(PackageId);
            IsolatedChocolateyEnvironment.SeedInstalledPackage(PackageId, InstalledVersion);
        }

        [SetUp]
        public void Arrange()
        {
            OpenRemoteSourceAtBaseline();
        }

        [Test]
        public void ExactSearch_WithInstalledPackage_ShowsInstalledVersion()
        {
            // Latest-only (single result) view shows the installed version, with the newer remote version
            // tracked separately as the available/outdated version - the intended #1109 behaviour.
            Search(SearchTerm);
            SetCheckBox(AutomationIds.MATCH_CHECK_BOX, true);

            var packageList = RemoteSourceScreen.GetPackageList();
            Assert.That(packageList, Has.Length.EqualTo(1));

            var targetPackage = packageList.First();
            targetPackage.Click();
            Thread.Sleep(100); // Without waiting, sometimes the double-click is missed.
            targetPackage.DoubleClick();

            var versionItem = MainScreen.FindFirstDescendant(cf => cf.ByAutomationId(AutomationIds.VERSION_TEXT));
            Assert.That(versionItem.Name, Is.EqualTo(InstalledVersion));
        }

        [Test]
        public void AllVersionsExactSearch_WithInstalledPackage_ShowsDistinctRemoteVersions()
        {
            // Issue #1146: with the bug present every row's version is overwritten with the installed 3.0.0,
            // so the rows remain but collapse to a single distinct version. The fix keeps each row's own
            // remote version.
            Search(SearchTerm);
            SetCheckBox(AutomationIds.MATCH_CHECK_BOX, true);
            SetCheckBox(AutomationIds.ALL_VERSIONS_CHECK_BOX, true);

            var packageList = RemoteSourceScreen.GetPackageList();
            Assert.That(packageList, Has.Length.GreaterThan(1), "Expected multiple version rows for an all-versions search.");

            var displayedVersions = RemoteSourceScreen.GetDisplayedVersions();
            Assert.That(
                displayedVersions.Length,
                Is.GreaterThan(1),
                "All rows are showing the same version - this is issue #1146 (installed version repeated for every row).");

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
