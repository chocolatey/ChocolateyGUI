// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyRemotePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using ChocolateyGui.Interface;
using ChocolateyGui.Models;
using ChocolateyGui.Services.PackageServices;
using ChocolateyGui.Utilities;
using ChocolateyGui.ViewModels.Items;
using NuGet;
using Serilog;
using WampSharp.V2;
using WampSharp.V2.Client;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Fluent;

namespace ChocolateyGui.Services
{
    public class ChocolateyRemotePackageService : IChocolateyPackageService, IDisposable
    {
        private static readonly Serilog.ILogger Logger = Log.ForContext<ChocolateyRemotePackageService>();
        private readonly IProgressService _progressService;
        private readonly IMapper _mapper;
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<IPackageViewModel> _packageFactory;
        private IWampChannel _wampChannel;
        private IChocolateyService _chocolateyService;
        private IDisposable _logStream;
        private bool _isInitialized;
        private AsyncLock _lock = new AsyncLock();

        public ChocolateyRemotePackageService(
            IProgressService progressService,
            IMapper mapper,
            IEventAggregator eventAggregator,
            Func<IPackageViewModel> packageFactory)
        {
            _progressService = progressService;
            _mapper = mapper;
            _eventAggregator = eventAggregator;
            _packageFactory = packageFactory;
        }

        public Task<PackageSearchResults> Search(string query, PackageSearchOptions options)
        {
            return null;
        }

        public async Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version, bool isPrerelease)
        {
            await Initialize();
            var result = await _chocolateyService.GetByVersionAndIdAsync(id, version.ToString(), isPrerelease);
            return _mapper.Map(result, _packageFactory());
        }

        public async Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false)
        {
            await _progressService.StartLoading("Loading packages...");
            try
            {
                await Initialize();
                var packages = await _chocolateyService.GetInstalledPackages();
                var vms = packages.Select(p => _mapper.Map(p, _packageFactory()));
                return vms;
            }
            finally
            {
                await _progressService.StopLoading();
            }
        }

        public async Task<IReadOnlyList<Tuple<string, SemanticVersion>>> GetOutdatedPackages(bool includePrerelease = false)
        {
            await Initialize();
            var results = await _chocolateyService.GetOutdatedPackages(includePrerelease);
            var parsed = results.Select(result => Tuple.Create(result.Item1, new SemanticVersion(result.Item2)));
            return parsed.ToList();
        }

        public Task InstallPackage(string id, SemanticVersion version = null, Uri source = null, bool force = false)
        {
            return null;
        }

        public Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            return null;
        }

        public Task UpdatePackage(string id, Uri source = null)
        {
            return null;
        }

        public Task PinPackage(string id, SemanticVersion version)
        {
            return null;
        }

        public Task UnpinPackage(string id, SemanticVersion version)
        {
            return null;
        }

        public void Dispose()
        {
            _logStream?.Dispose();
            _wampChannel?.Close("Exiting", new GoodbyeDetails { Message = "Exiting" });
        }

        private Task Initialize()
        {
            return Task.Run(InitializeImpl);
        }

        private async Task InitializeImpl()
        {
            if (_isInitialized)
            {
                return;
            }

            using (await _lock.LockAsync())
            {
                const string Port = "24606";
                var subprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChocolateyGui.Subprocess.exe");
                var startInfo = new ProcessStartInfo
                {
                    Arguments = Port,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = subprocessPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var subprocessHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "ChocolateyGui_Wait"))
                {
                    var chocoSubProcess = Process.Start(startInfo);
                    if (!subprocessHandle.WaitOne(TimeSpan.FromSeconds(3)))
                    {
                        if (chocoSubProcess.HasExited)
                        {
                            var output = chocoSubProcess.StandardOutput.ReadToEnd();
                            var error = chocoSubProcess.StandardError.ReadToEnd();
                            Log.Logger.Fatal("Failed to start Chocolatey subprocess. Exit Code {ExitCode}.\n{Output}\n{Error}",
                                chocoSubProcess.ExitCode, output, error);
                            throw new ApplicationException($"Failed to start chocolatey subprocess.\n{output}");
                        }

                        if (!chocoSubProcess.WaitForExit(TimeSpan.FromSeconds(3).Milliseconds) &&
                            !subprocessHandle.WaitOne(0))
                        {
                            chocoSubProcess.Kill();
                            Log.Logger.Fatal(
                                "Failed to start Chocolatey subprocess. Process appears to be broken or otherwise non-functional.",
                                chocoSubProcess.ExitCode);
                            throw new ApplicationException("Failed to start chocolatey subprocess.");
                        }
                        else
                        {
                            if (chocoSubProcess.HasExited)
                            {
                                var output = chocoSubProcess.StandardOutput.ReadToEnd();
                                var error = chocoSubProcess.StandardError.ReadToEnd();
                                Log.Logger.Fatal("Failed to start Chocolatey subprocess. Exit Code {ExitCode}.\n{Output}\n{Error}",
                                    chocoSubProcess.ExitCode, output, error);
                                throw new ApplicationException($"Failed to start chocolatey subprocess.\n{output}");
                            }
                        }
                    }

                    if (chocoSubProcess.WaitForExit(500))
                    {
                        var output = chocoSubProcess.StandardOutput.ReadToEnd();
                        var error = chocoSubProcess.StandardError.ReadToEnd();
                        Log.Logger.Fatal("Failed to start Chocolatey subprocess. Exit Code {ExitCode}.\n{Output}\n{Error}",
                            chocoSubProcess.ExitCode, output, error);
                        throw new ApplicationException($"Failed to start chocolatey subprocess.\n{output}");
                    }
                }

                var factory = new WampChannelFactory();
                _wampChannel =
                    factory.ConnectToRealm("default")
                        .WebSocketTransport($"ws://127.0.0.1:{Port}/ws")
                        .JsonSerialization()
                        .Build();

                await _wampChannel.Open().ConfigureAwait(false);
                _isInitialized = true;

                _chocolateyService = _wampChannel.RealmProxy.Services.GetCalleeProxy<IChocolateyService>();

                // Create pipe for chocolatey stream output.
                var logStream = _wampChannel.RealmProxy.Services.GetSubject<StreamingLogMessage>("com.chocolatey.log");
                _logStream = logStream.Subscribe(message =>
                {
                    PowerShellLineType powerShellLineType;
                    switch (message.LogLevel)
                    {
                        case StreamingLogLevel.Debug:
                            powerShellLineType = PowerShellLineType.Debug;
                            break;
                        case StreamingLogLevel.Verbose:
                            powerShellLineType = PowerShellLineType.Verbose;
                            break;
                        case StreamingLogLevel.Info:
                            powerShellLineType = PowerShellLineType.Output;
                            break;
                        case StreamingLogLevel.Warn:
                            powerShellLineType = PowerShellLineType.Warning;
                            break;
                        case StreamingLogLevel.Error:
                            powerShellLineType = PowerShellLineType.Error;
                            break;
                        case StreamingLogLevel.Fatal:
                            powerShellLineType = PowerShellLineType.Error;
                            break;
                        default:
                            powerShellLineType = PowerShellLineType.Output;
                            break;
                    }

                    _progressService.WriteMessage(message.Message, powerShellLineType);
                });
            }
        }
    }
}
