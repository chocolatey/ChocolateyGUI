// <copyright file="MockFeeds.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Collections.Generic;

namespace ChocolateyGui.UITests.Support.Feed
{
    /// <summary>
    ///     The concrete datasets the FlaUI tests run against. Kept separate from the serving machinery so the
    ///     "what should the feed contain" decisions live in one obvious place, and so the issue #1109
    ///     multi-source matrix can be added here as extra named feeds without touching the server.
    /// </summary>
    public static class MockFeeds
    {
        /// <summary>
        ///     The <c>hermes</c> source the existing UITests expect. Replaces the internal-only NuGet feed and
        ///     its <c>mixedpackage</c> so the tests no longer depend on anything outside the repo.
        ///     <para>
        ///     <c>mixedpackage</c> is shaped to satisfy <c>BetaPackagesTests</c>: latest stable <c>2.0.0</c>,
        ///     latest prerelease <c>2.1.0-beta-1</c>, three stable versions in total, and ten versions overall.
        ///     </para>
        /// </summary>
        public static IList<MockFeedPackage> Hermes()
        {
            return new List<MockFeedPackage>
            {
                new MockFeedPackage(
                    "mixedpackage",
                    "Mixed Package",
                    "A package with both stable and prerelease versions.",
                    new[]
                    {
                        // 3 stable (latest stable = 2.0.0) ...
                        "1.0.0",
                        "1.1.0",
                        "2.0.0",

                        // ... plus 7 prereleases (latest overall = 2.1.0-beta-1), 10 versions in total.
                        "1.0.0-beta-1",
                        "1.0.0-beta-2",
                        "1.1.0-beta-1",
                        "2.0.0-beta-1",
                        "2.0.0-beta-2",
                        "2.1.0-alpha-1",
                        "2.1.0-beta-1",
                    }),

                // Present in the default (empty-search) listing; ChocolateyGuiTests.RemoteSourceScreenTest
                // drills into this package from the hermes source.
                new MockFeedPackage(
                    "absolute-extracted-path",
                    "absolute-extracted-path",
                    "A package used to verify opening package details from the hermes source.",
                    new[] { "1.0.0" }),
            };
        }
    }
}
