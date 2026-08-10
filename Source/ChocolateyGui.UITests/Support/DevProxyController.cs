// <copyright file="DevProxyController.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace ChocolateyGui.UITests.Support
{
    /// <summary>
    ///     Starts and stops Dev Proxy (https://aka.ms/devproxy) so that the UITests can run in
    ///     isolation from any real package repository. Dev Proxy registers itself as the Windows
    ///     system proxy; Chocolatey CLI (chocolatey.lib), which Chocolatey GUI delegates all of its
    ///     network calls to, falls back to the system proxy when no explicit proxy is configured, so
    ///     the GUI's OData calls to community.chocolatey.org are transparently served from the mock
    ///     responses under <c>Fixtures/community.chocolatey.org</c> - no GUI code change required.
    /// </summary>
    internal sealed class DevProxyController : IDisposable
    {
        private const string InternetSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

        private readonly string _executablePath;
        private readonly string _configDirectory;

        private Process _process;
        private int _originalProxyEnable;
        private string _originalProxyServer;
        private bool _proxyStateCaptured;

        public DevProxyController(string configDirectory)
        {
            _configDirectory = configDirectory;
            _executablePath = LocateExecutable();
        }

        /// <summary>
        ///     Gets a value indicating whether a Dev Proxy executable could be located on this machine.
        ///     Tests should mark themselves inconclusive (Assert.Ignore) when this is false rather than fail.
        /// </summary>
        public bool IsAvailable => !string.IsNullOrEmpty(_executablePath);

        public string ExecutablePath => _executablePath;

        public void Start()
        {
            if (!IsAvailable)
            {
                throw new InvalidOperationException(
                    "Dev Proxy could not be located. Install it from https://aka.ms/devproxy (e.g. 'winget install DevProxy.DevProxy') or set the DEVPROXY_PATH environment variable.");
            }

            // Ensure the root certificate exists and is trusted so HTTPS interception works.
            RunDevProxy("cert ensure", waitForExit: true);

            CaptureProxyState();

            var startInfo = new ProcessStartInfo
            {
                FileName = _executablePath,
                Arguments = "--config-file ./devproxyrc.json --no-first-run",
                WorkingDirectory = _configDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var listening = new ManualResetEventSlim(false);

            _process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            _process.OutputDataReceived += (_, e) =>
            {
                // Match the proxy listener ("Dev Proxy listening on 127.0.0.1:8000...") specifically, NOT the
                // API listener line ("Dev Proxy API listening on http://127.0.0.1:8897..."), which comes up first.
                if (e.Data != null && e.Data.IndexOf("Dev Proxy listening on", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    listening.Set();
                }
            };

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            if (!listening.Wait(TimeSpan.FromSeconds(60)))
            {
                Stop();
                throw new TimeoutException("Dev Proxy did not report that it was listening within 60 seconds.");
            }
        }

        public void Stop()
        {
            try
            {
                if (_process != null && !_process.HasExited)
                {
                    // Dev Proxy restores the system proxy on graceful exit, but a forced kill will not,
                    // so we always restore the captured proxy state below as a safety net.
                    try
                    {
                        _process.Kill();
                        _process.WaitForExit(10000);
                    }
                    catch
                    {
                        // Ignore - we still restore the proxy below.
                    }
                }
            }
            finally
            {
                _process?.Dispose();
                _process = null;
                RestoreProxyState();
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private static string LocateExecutable()
        {
            var fromEnvironment = Environment.GetEnvironmentVariable("DEVPROXY_PATH");
            if (!string.IsNullOrEmpty(fromEnvironment) && File.Exists(fromEnvironment))
            {
                return fromEnvironment;
            }

            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var defaultInstall = Path.Combine(localAppData, "Programs", "Dev Proxy", "devproxy.exe");
            if (File.Exists(defaultInstall))
            {
                return defaultInstall;
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
                    var candidate = Path.Combine(directory.Trim(), "devproxy.exe");
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
                catch
                {
                    // Ignore malformed PATH entries.
                }
            }

            return null;
        }

        private void RunDevProxy(string arguments, bool waitForExit)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _executablePath,
                Arguments = arguments,
                WorkingDirectory = _configDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(startInfo))
            {
                if (waitForExit && process != null)
                {
                    process.WaitForExit(60000);
                }
            }
        }

        private void CaptureProxyState()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey, writable: false))
            {
                _originalProxyEnable = (int)(key?.GetValue("ProxyEnable") ?? 0);
                _originalProxyServer = key?.GetValue("ProxyServer") as string;
            }

            _proxyStateCaptured = true;
        }

        private void RestoreProxyState()
        {
            if (!_proxyStateCaptured)
            {
                return;
            }

            using (var key = Registry.CurrentUser.OpenSubKey(InternetSettingsKey, writable: true))
            {
                if (key == null)
                {
                    return;
                }

                key.SetValue("ProxyEnable", _originalProxyEnable, RegistryValueKind.DWord);

                if (string.IsNullOrEmpty(_originalProxyServer))
                {
                    if (key.GetValue("ProxyServer") != null)
                    {
                        key.DeleteValue("ProxyServer", throwOnMissingValue: false);
                    }
                }
                else
                {
                    key.SetValue("ProxyServer", _originalProxyServer, RegistryValueKind.String);
                }
            }
        }
    }
}
