// <copyright file="WireMockRemoteSourceTestBase.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Collections.Generic;
using System.IO;
using ChocolateyGui.UITests.Support;
using ChocolateyGui.UITests.Support.Feed;
using FlaUI.Core;
using NUnit.Framework;

namespace ChocolateyGui.UITests
{
    /// <summary>
    ///     Base class for UITests that run against one or more mocked remote sources served by WireMock.Net
    ///     (see <see cref="MockFeedServer" />) instead of a live or internal feed. Before the application
    ///     launches it stands up a mock server per <see cref="Sources" /> definition, registers each as a
    ///     Chocolatey source pointing at that server's localhost URL, and clears the HTTP cache so every run is
    ///     deterministic. Standing up several sources - each with its own dataset - is how the multi-source
    ///     prerelease/outdated matrix from issue #1109 is expressed.
    ///     <para>
    ///     Each server records the requests it received; on tear-down those are written next to the test
    ///     assembly as <c>mockfeed-requests-&lt;source&gt;.log</c> so the exact queries Chocolatey.Lib emits can
    ///     be confirmed (the matchers in <see cref="MockFeedServer" /> are built from them).
    ///     </para>
    /// </summary>
    public abstract class WireMockRemoteSourceTestBase : ChocolateyGuiTestBase
    {
        private readonly List<MockFeedServer> _servers = new List<MockFeedServer>();
        private readonly List<string> _registeredSources = new List<string>();

        /// <summary>The sources to stand up for the fixture. One <see cref="MockFeedServer" /> is created per entry.</summary>
        protected abstract IEnumerable<MockSourceDefinition> Sources { get; }

        protected override Application StartApplication()
        {
            // Guard against a mid-fixture application restart re-entering here with servers already running.
            TearDownServers();

            foreach (var definition in Sources)
            {
                var server = new MockFeedServer(definition.Packages, IsolatedChocolateyEnvironment.MockResponsesDirectory);
                server.Start();
                _servers.Add(server);

                IsolatedChocolateyEnvironment.AddSource(definition.Name, server.SourceUrl);
                _registeredSources.Add(definition.Name);
            }

            IsolatedChocolateyEnvironment.ClearHttpCache();

            try
            {
                return base.StartApplication();
            }
            catch
            {
                TearDownServers();
                throw;
            }
        }

        [OneTimeTearDown]
        public void StopServers()
        {
            TearDownServers();
        }

        private void TearDownServers()
        {
            WriteRequestLogs();

            foreach (var name in _registeredSources)
            {
                IsolatedChocolateyEnvironment.RemoveSource(name);
            }

            _registeredSources.Clear();

            foreach (var server in _servers)
            {
                server.Dispose();
            }

            _servers.Clear();
        }

        private void WriteRequestLogs()
        {
            for (var i = 0; i < _servers.Count; i++)
            {
                var name = i < _registeredSources.Count ? _registeredSources[i] : i.ToString();
                var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "mockfeed-requests-" + name + ".log");

                try
                {
                    _servers[i].WriteRequestLog(path);
                    TestContext.Progress.WriteLine("Mock feed request log written to " + path);
                }
                catch
                {
                    // Diagnostics only - never fail a run because the log could not be written.
                }
            }
        }
    }

    /// <summary>Associates a source name with the dataset the mock server should answer from.</summary>
    public class MockSourceDefinition
    {
        public MockSourceDefinition(string name, IList<MockFeedPackage> packages)
        {
            Name = name;
            Packages = packages;
        }

        public string Name { get; }

        public IList<MockFeedPackage> Packages { get; }
    }
}
