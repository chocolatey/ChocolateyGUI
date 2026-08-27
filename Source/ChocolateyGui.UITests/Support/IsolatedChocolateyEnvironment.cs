// <copyright file="IsolatedChocolateyEnvironment.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using NUnit.Framework;

namespace ChocolateyGui.UITests.Support
{
    /// <summary>
    ///     Helpers for driving the isolated Chocolatey installation that a DEBUG build of Chocolatey GUI
    ///     uses (it points <c>ChocolateyInstall</c> at its own bin directory - see DebugInstallEnvironment). These
    ///     let the UITests seed an "installed" package and clear the HTTP cache so a run is deterministic.
    /// </summary>
    internal static class IsolatedChocolateyEnvironment
    {
        /// <summary>
        ///     The directory the Chocolatey GUI executable runs from. A DEBUG GUI build uses this as its
        ///     ChocolateyInstall location, so its <c>lib</c>, <c>config</c> and <c>.chocolatey</c> all live here.
        /// </summary>
        public static string GuiInstallLocation
        {
            get
            {
                // Mirror how ChocolateyGuiTestBase locates the GUI exe: the UITests output directory with
                // the ".UITests" segment removed points at the GUI's own output directory.
                return TestContext.CurrentContext.TestDirectory.Replace(".UITests", string.Empty)
                    .TrimEnd(Path.DirectorySeparatorChar);
            }
        }

        /// <summary>
        ///     The fixtures directory (copied next to the test assembly) containing the Dev Proxy config,
        ///     mock responses and the stub package used to seed an installed state.
        /// </summary>
        public static string FixturesDirectory =>
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures");

        public static string MockResponsesDirectory =>
            Path.Combine(FixturesDirectory, "community.chocolatey.org");

        public static string StubPackagesDirectory =>
            Path.Combine(FixturesDirectory, "packages");

        /// <summary>
        ///     Installs a package into the isolated GUI install location, skipping automation scripts so that
        ///     only the nupkg is extracted into the local lib folder (nothing actually runs). This is used to
        ///     put Chocolatey GUI into an "installed" state required to reproduce issue #1146.
        /// </summary>
        public static void SeedInstalledPackage(string id, string version)
        {
            EnsureStubPackage(id, version);

            var arguments = string.Format(
                "install {0} --version {1} --source \"{2}\" --skip-automation-scripts --yes --force --no-progress",
                id,
                version,
                StubPackagesDirectory);

            var result = RunChoco(arguments);

            if (!result.Located)
            {
                Assert.Ignore("Chocolatey CLI (choco.exe) could not be located; cannot seed the installed package.");
            }

            if (result.TimedOut)
            {
                Assert.Ignore(
                    $"Seeding installed package {id} {version} did not complete within 120 seconds; treating the run as inconclusive.");
            }

            if (result.ExitCode != 0)
            {
                Assert.Ignore(
                    $"Failed to seed installed package {id} {version} (exit {result.ExitCode}).{Environment.NewLine}{result.Output}");
            }
        }

        /// <summary>
        ///     Writes a minimal stub <c>.nupkg</c> for <paramref name="id" />/<paramref name="version" /> into
        ///     <see cref="StubPackagesDirectory" /> if one is not already there, so <see cref="SeedInstalledPackage" />
        ///     has something to install from a local folder source. The package carries only its nuspec inside a
        ///     valid OPC container - enough for Chocolatey to record it as installed (seeding uses
        ///     <c>--skip-automation-scripts</c>, so no install script is expected).
        /// </summary>
        public static void EnsureStubPackage(string id, string version)
        {
            Directory.CreateDirectory(StubPackagesDirectory);

            var nupkgPath = Path.Combine(StubPackagesDirectory, string.Format("{0}.{1}.nupkg", id, version));
            if (File.Exists(nupkgPath))
            {
                return;
            }

            var nuspec = string.Format(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<package xmlns=\"http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd\">\r\n" +
                "  <metadata>\r\n" +
                "    <id>{0}</id>\r\n" +
                "    <version>{1}</version>\r\n" +
                "    <title>{0}</title>\r\n" +
                "    <authors>Chocolatey</authors>\r\n" +
                "    <owners>Chocolatey</owners>\r\n" +
                "    <description>Stub package used by the UITests to seed an installed state.</description>\r\n" +
                "    <tags>chocolatey mock</tags>\r\n" +
                "  </metadata>\r\n" +
                "</package>\r\n",
                id,
                version);

            var contentTypes =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">\r\n" +
                "  <Default Extension=\"nuspec\" ContentType=\"application/octet\" />\r\n" +
                "  <Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\" />\r\n" +
                "</Types>\r\n";

            var rels = string.Format(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">\r\n" +
                "  <Relationship Type=\"http://schemas.microsoft.com/packaging/2010/07/manifest\" Target=\"/{0}.nuspec\" Id=\"Rnuspec\" />\r\n" +
                "</Relationships>\r\n",
                id);

            using (var archive = ZipFile.Open(nupkgPath, ZipArchiveMode.Create))
            {
                WriteEntry(archive, "[Content_Types].xml", contentTypes);
                WriteEntry(archive, "_rels/.rels", rels);
                WriteEntry(archive, id + ".nuspec", nuspec);
            }
        }

        /// <summary>
        ///     Registers (or replaces) a remote source in the isolated Chocolatey configuration so the GUI
        ///     lists and queries it. Used to point a named source (e.g. <c>hermes</c>) at a local mock feed
        ///     rather than an external server. Runs choco's own <c>source add</c>, so the config file stays in
        ///     choco's native format. The source is removed first, so the URL always reflects the current mock
        ///     (the mock's localhost port varies from run to run).
        /// </summary>
        public static void AddSource(string name, string url)
        {
            RemoveSource(name);

            var result = RunChoco(string.Format("source add --name \"{0}\" --source \"{1}\" --priority 0", name, url));

            if (!result.Located)
            {
                Assert.Ignore("Chocolatey CLI (choco.exe) could not be located; cannot register the mock source.");
            }

            if (result.TimedOut || result.ExitCode != 0)
            {
                Assert.Ignore(
                    $"Failed to register mock source {name} -> {url} (exit {result.ExitCode}).{Environment.NewLine}{result.Output}");
            }
        }

        /// <summary>
        ///     Removes a source from the isolated configuration. Best-effort and idempotent - safe to call
        ///     whether or not the source currently exists.
        /// </summary>
        public static void RemoveSource(string name)
        {
            RunChoco(string.Format("source remove --name \"{0}\"", name));
        }

        /// <summary>
        ///     Removes a package from the isolated GUI install location (the <c>lib</c> folder and its package
        ///     information store entries) so the GUI no longer considers it installed. Best-effort and
        ///     idempotent - safe to call whether or not the package is currently installed. Done by deleting
        ///     the folders directly (rather than <c>choco uninstall</c>) so it works while nothing holds a lock,
        ///     i.e. before the application is launched.
        /// </summary>
        public static void RemoveInstalledPackage(string id)
        {
            TryDeleteDirectory(Path.Combine(GuiInstallLocation, "lib", id));

            var infoStore = Path.Combine(GuiInstallLocation, ".chocolatey");
            if (Directory.Exists(infoStore))
            {
                foreach (var directory in Directory.GetDirectories(infoStore, id + ".*"))
                {
                    TryDeleteDirectory(directory);
                }
            }
        }

        /// <summary>
        ///     Clears Chocolatey's HTTP cache for the mocked source so OData responses are fetched fresh
        ///     (and therefore go through Dev Proxy) on the next run rather than being served from a previous
        ///     run's cache. Chocolatey's HTTP cache is context-dependent: a non-elevated process uses the
        ///     user cache (<c>%USERPROFILE%\.chocolatey\http-cache</c>), while an elevated one - which these
        ///     UITests are - uses the system cache (<c>%ProgramData%\ChocolateyHttpCache</c>). Both are
        ///     cleared here; skipping the system cache lets a stale response (e.g. an empty exact-match result
        ///     cached while a fixture was being authored) be served until its TTL expires and fail the tests
        ///     non-deterministically.
        /// </summary>
        public static void ClearHttpCache()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            TryDeleteDirectory(Path.Combine(userProfile, ".chocolatey", "http-cache"));
            TryDeleteDirectory(Path.Combine(GuiInstallLocation, ".chocolatey", "http-cache"));

            // The system cache stores one directory per source, named for the source URL (e.g.
            // "<hash>$community.chocolatey.org_api_v2_"). Only clear the mocked source's entries so the
            // developer's other cached sources are left untouched.
            var systemHttpCache = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "ChocolateyHttpCache");
            if (Directory.Exists(systemHttpCache))
            {
                foreach (var sourceDirectory in Directory.GetDirectories(systemHttpCache, "*community.chocolatey.org*"))
                {
                    TryDeleteDirectory(sourceDirectory);
                }
            }
        }

        /// <summary>
        ///     Deletes Chocolatey GUI's cached "outdated packages" results (<c>outdatedPackages*.xml</c> under
        ///     <c>%LocalAppData%\Chocolatey GUI</c>) so the next outdated check runs fresh against the mock feed.
        ///     The result is otherwise cached per source and per prerelease preference for 60 minutes, which makes
        ///     an outdated-status test non-deterministic across runs. Unlike the HTTP cache this lives in the real
        ///     user profile (the GUI computes it from <see cref="Environment.SpecialFolder.LocalApplicationData" />),
        ///     not the isolated install, so it is cleared separately.
        /// </summary>
        public static void ClearOutdatedPackagesCache()
        {
            var cacheDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify),
                "Chocolatey GUI");

            if (!Directory.Exists(cacheDirectory))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(cacheDirectory, "outdatedPackages*.xml"))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // Best effort - a locked cache file should not fail the test run.
                }
            }
        }

        /// <summary>
        ///     Runs the located <c>choco.exe</c> against the isolated GUI install location, draining both
        ///     output pipes asynchronously (reading one stream to the end before the other can deadlock when a
        ///     buffer fills) and treating a non-exit within the timeout as inconclusive rather than reading the
        ///     exit code of a still-running process. Callers decide how to react to the result.
        /// </summary>
        private static ChocoRunResult RunChoco(string arguments)
        {
            var choco = LocateChocoExecutable();
            if (choco == null)
            {
                return new ChocoRunResult { Located = false };
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = choco,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            // Point the choco process at the isolated GUI install location so it reads/writes there rather
            // than the machine-wide C:\ProgramData\chocolatey.
            startInfo.EnvironmentVariables["ChocolateyInstall"] = GuiInstallLocation;

            using (var process = Process.Start(startInfo))
            {
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                if (!process.WaitForExit(120000))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        // Best effort - the process may have exited between the check and the kill.
                    }

                    return new ChocoRunResult { Located = true, TimedOut = true };
                }

                var output = outputTask.GetAwaiter().GetResult();
                var error = errorTask.GetAwaiter().GetResult();

                return new ChocoRunResult
                {
                    Located = true,
                    ExitCode = process.ExitCode,
                    Output = output + error,
                };
            }
        }

        private static string LocateChocoExecutable()
        {
            var machineInstall = Environment.GetEnvironmentVariable("ChocolateyInstall", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(machineInstall))
            {
                machineInstall = @"C:\ProgramData\chocolatey";
            }

            var candidate = Path.Combine(machineInstall, "choco.exe");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var directory in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrWhiteSpace(directory))
                {
                    continue;
                }

                try
                {
                    var fromPath = Path.Combine(directory.Trim(), "choco.exe");
                    if (File.Exists(fromPath))
                    {
                        return fromPath;
                    }
                }
                catch
                {
                    // Ignore malformed PATH entries.
                }
            }

            return null;
        }

        private static void WriteEntry(ZipArchive archive, string entryName, string content)
        {
            var entry = archive.CreateEntry(entryName);
            using (var stream = entry.Open())
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
            {
                writer.Write(content);
            }
        }

        private static void TryDeleteDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, recursive: true);
                }
            }
            catch
            {
                // Best effort - a locked cache file should not fail the test run.
            }
        }

        /// <summary>The outcome of a <see cref="RunChoco" /> invocation.</summary>
        private struct ChocoRunResult
        {
            /// <summary><c>false</c> when choco.exe could not be found at all.</summary>
            public bool Located;

            /// <summary><c>true</c> when the process did not exit within the timeout.</summary>
            public bool TimedOut;

            /// <summary>The process exit code (only meaningful when <see cref="Located" /> and not <see cref="TimedOut" />).</summary>
            public int ExitCode;

            /// <summary>Combined stdout + stderr, for diagnostics on failure.</summary>
            public string Output;
        }
    }
}
