// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyRemotePackageService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using ChocolateyGui.Models;
using ChocolateyGui.Models.Messages;
using ChocolateyGui.Properties;
using ChocolateyGui.Providers;
using ChocolateyGui.ViewModels.Items;
using Microsoft.VisualStudio.Threading;
using NuGet;
using Serilog;
using ILogger = Serilog.ILogger;
using PackageSearchResults = ChocolateyGui.Models.PackageSearchResults;

namespace ChocolateyGui.Services
{
    public class ChocolateyRemotePackageService : IChocolateyPackageService, IDisposable
    {
        private static readonly ILogger Logger = Log.ForContext<ChocolateyRemotePackageService>();
        private readonly IProgressService _progressService;
        private readonly IMapper _mapper;
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<IPackageViewModel> _packageFactory;
        private readonly AsyncSemaphore _lock = new AsyncSemaphore(1);
        private readonly Lazy<bool> _forceElevation;

        private Process _chocolateyProcess;
        private IIpcChocolateyService _chocolateyService;
        private bool? _requiresElevation;

        public ChocolateyRemotePackageService(
            IProgressService progressService,
            IMapper mapper,
            IEventAggregator eventAggregator,
            IConfigService configService,
            Func<IPackageViewModel> packageFactory)
        {
            _progressService = progressService;
            _mapper = mapper;
            _eventAggregator = eventAggregator;
            _packageFactory = packageFactory;
            _forceElevation = new Lazy<bool>(() => configService.GetSettings().ElevateByDefault);
        }

        public async Task<PackageSearchResults> Search(string query, PackageSearchOptions options)
        {
            await Initialize();
            var results = await _chocolateyService.Search(query, options);
            return new PackageSearchResults
                       {
                           Packages =
                               results.Packages.Select(
                                   pcgke => _mapper.Map(pcgke, _packageFactory())),
                           TotalCount = results.TotalCount
                       };
        }

        public async Task<IPackageViewModel> GetByVersionAndIdAsync(string id, SemanticVersion version, bool isPrerelease)
        {
            await Initialize();
            var result = await _chocolateyService.GetByVersionAndIdAsync(id, version.ToString(), isPrerelease);
            return _mapper.Map(result, _packageFactory());
        }

        public async Task<IEnumerable<IPackageViewModel>> GetInstalledPackages(bool force = false)
        {
            await Initialize();
            var packages = await _chocolateyService.GetInstalledPackages();
            var vms = packages.Select(p => _mapper.Map(p, _packageFactory())).ToList();
            return vms;
        }

        public async Task<IReadOnlyList<Tuple<string, SemanticVersion>>> GetOutdatedPackages(bool includePrerelease = false)
        {
            await Initialize();
            var results = await _chocolateyService.GetOutdatedPackages(includePrerelease);
            var parsed = results.Select(result => Tuple.Create(result.Item1, new SemanticVersion(result.Item2)));
            return parsed.ToList();
        }

        public async Task InstallPackage(string id, SemanticVersion version = null, Uri source = null, bool force = false)
        {
            await Initialize(true);
            if (Elevation.Instance.IsBackgroundRunning)
            {
                source = null;
            }

            var result = await _chocolateyService.InstallPackage(id, version?.ToString(), source, force);
            if (!result.Successful)
            {
                var exceptionMessage = result.Exception == null
                    ? string.Empty
                    : string.Format(Resources.ChocolateyRemotePackageService_ExceptionFormat, result.Exception);
                var message = string.Format(
                    Resources.ChocolateyRemotePackageService_InstallFailedMessage,
                    id,
                    version,
                    string.Join("\n", result.Messages),
                    exceptionMessage);
                await _progressService.ShowMessageAsync(
                    Resources.ChocolateyRemotePackageService_InstallFailedTitle,
                    message);
                Logger.Warning(result.Exception, "Failed to install {Package}, version {Version}. Errors: {Errors}", id, version, result.Messages);
                return;
            }

            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Installed, version));
        }

        public async Task UninstallPackage(string id, SemanticVersion version, bool force = false)
        {
            await Initialize(true);
            var result = await _chocolateyService.UninstallPackage(id, version.ToString(), force);
            if (!result.Successful)
            {
                var exceptionMessage = result.Exception == null
                    ? string.Empty
                    : string.Format(Resources.ChocolateyRemotePackageService_ExceptionFormat, result.Exception);
                var message = string.Format(
                    Resources.ChocolateyRemotePackageService_UninstallFailedMessage,
                    id,
                    version,
                    string.Join("\n", result.Messages),
                    exceptionMessage);
                await _progressService.ShowMessageAsync(
                    Resources.ChocolateyRemotePackageService_UninstallFailedTitle,
                    message);
                Logger.Warning(result.Exception, "Failed to uninstall {Package}, version {Version}. Errors: {Errors}", id, version, result.Messages);
                return;
            }

            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Uninstalled, version));
        }

        public async Task UpdatePackage(string id, Uri source = null)
        {
            await Initialize(true);
            if (Elevation.Instance.IsBackgroundRunning)
            {
                source = null;
            }

            var result = await _chocolateyService.UpdatePackage(id, source);
            if (!result.Successful)
            {
                var exceptionMessage = result.Exception == null
                    ? string.Empty
                    : string.Format(Resources.ChocolateyRemotePackageService_ExceptionFormat, result.Exception);
                var message = string.Format(
                    Resources.ChocolateyRemotePackageService_UpdateFailedMessage,
                    id,
                    string.Join("\n", result.Messages),
                    exceptionMessage);
                await _progressService.ShowMessageAsync(
                    Resources.ChocolateyRemotePackageService_UpdateFailedTitle,
                    message);
                Logger.Warning(result.Exception, "Failed to update {Package}. Errors: {Errors}", id, result.Messages);
                return;
            }

            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Updated));
        }

        public async Task PinPackage(string id, SemanticVersion version)
        {
            await Initialize(true);
            var result = await _chocolateyService.PinPackage(id, version.ToString());
            if (!result.Successful)
            {
                var exceptionMessage = result.Exception == null
                    ? string.Empty
                    : string.Format(Resources.ChocolateyRemotePackageService_ExceptionFormat, result.Exception);
                var message = string.Format(
                    Resources.ChocolateyRemotePackageService_PinFailedMessage,
                    id,
                    version,
                    string.Join("\n", result.Messages),
                    exceptionMessage);
                await _progressService.ShowMessageAsync(
                    Resources.ChocolateyRemotePackageService_PinFailedTitle,
                    message);
                Logger.Warning(result.Exception, "Failed to pin {Package}, version {Version}. Errors: {Errors}", id, version, result.Messages);
                return;
            }

            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Pinned, version));
        }

        public async Task UnpinPackage(string id, SemanticVersion version)
        {
            await Initialize(true);
            var result = await _chocolateyService.UnpinPackage(id, version.ToString());
            if (!result.Successful)
            {
                var exceptionMessage = result.Exception == null
                    ? string.Empty
                    : string.Format(Resources.ChocolateyRemotePackageService_ExceptionFormat, result.Exception);
                var message = string.Format(
                    Resources.ChocolateyRemotePackageService_UnpinFailedMessage,
                    id,
                    version,
                    string.Join("\n", result.Messages),
                    exceptionMessage);
                await _progressService.ShowMessageAsync(
                    Resources.ChocolateyRemotePackageService_UninstallFailedTitle,
                    message);
                Logger.Warning(result.Exception, "Failed to unpin {Package}, version {Version}. Errors: {Errors}", id, version, result.Messages);
                return;
            }

            _eventAggregator.BeginPublishOnUIThread(new PackageChangedMessage(id, PackageChangeType.Unpinned, version));
        }

        public async Task<IReadOnlyList<ChocolateyFeature>> GetFeatures()
        {
            await Initialize();
            return await _chocolateyService.GetFeatures();
        }

        public async Task SetFeature(ChocolateyFeature feature)
        {
            await Initialize(true);
            await _chocolateyService.SetFeature(feature);
            if (string.Equals(feature.Name, "useBackgroundService", StringComparison.OrdinalIgnoreCase))
            {
                Elevation.Instance.IsBackgroundRunning = feature.Enabled;
            }
        }

        public async Task<IReadOnlyList<ChocolateySetting>> GetSettings()
        {
            await Initialize();
            return await _chocolateyService.GetSettings();
        }

        public async Task SetSetting(ChocolateySetting setting)
        {
            await Initialize(true);
            await _chocolateyService.SetSetting(setting);
        }

        public async Task<IReadOnlyList<ChocolateySource>> GetSources()
        {
            await Initialize();
            return await _chocolateyService.GetSources();
        }

        public async Task AddSource(ChocolateySource source)
        {
            await Initialize(true);
            await _chocolateyService.AddSource(source);
        }

        public async Task UpdateSource(string id, ChocolateySource source)
        {
            await Initialize(true);
            await _chocolateyService.UpdateSource(id, source);
        }

        public async Task<bool> RemoveSource(string id)
        {
            await Initialize(true);
            return await _chocolateyService.RemoveSource(id);
        }

        public ValueTask<bool> RequiresElevation()
        {
            return _requiresElevation.HasValue ? new ValueTask<bool>(_requiresElevation.Value) : new ValueTask<bool>(RequiresElevationImpl());
        }

        public void Dispose()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var clientChannel = _chocolateyService as IClientChannel;
            if (clientChannel != null && clientChannel.State == CommunicationState.Opened)
            {
                _chocolateyService.Unregister();
                clientChannel.Close();
            }
        }

        private async Task<bool> RequiresElevationImpl()
        {
            await Initialize();
            _requiresElevation = !await _chocolateyService.IsElevated();
            return _requiresElevation.Value;
        }

        private async Task Initialize(bool requireAdmin = false)
        {
            try
            {
                await InitializeImpl(requireAdmin);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Failed to initialize the chocolatey server.");
                throw;
            }
        }

        private async Task InitializeImpl(bool requireAdmin = false)
        {
            requireAdmin = requireAdmin || _forceElevation.Value;

            // Check if we're not already initialized or running, as well as our permissions level.
            if (_chocolateyProcess != null && !_chocolateyProcess.HasExited)
            {
                if (!requireAdmin || await _chocolateyService.IsElevated())
                {
                    return;
                }
            }

            using (await _lock.EnterAsync())
            {
                // Double check our initialization and permissions status.
                if (_chocolateyProcess != null && !_chocolateyProcess.HasExited)
                {
                    if (!requireAdmin || await _chocolateyService.IsElevated())
                    {
                        return;
                    }

                    _chocolateyService.Exit(true);
                    _chocolateyService = null;

                    if (!_chocolateyProcess.HasExited)
                    {
                        if (!_chocolateyProcess.WaitForExit(2000))
                        {
                            _chocolateyProcess.Kill();
                        }
                    }
                }

                var processes = Process.GetProcessesByName("ChocolateyGui.Subprocess");

                // Do we already have a living subprocess somewhere? (Multiple GUIs running simultaneously)
                if (processes.Length != 0)
                {
                    _chocolateyProcess = processes[0];
                    _chocolateyService = CreateClient();
                    return;
                }

                // If we don't have an endpoint already, spin up a new process.
                var subprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Subprocess/ChocolateyGui.Subprocess.exe");
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = subprocessPath,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                if (requireAdmin)
                {
                    startInfo.Verb = "runas";
                }

                try
                {
                    _chocolateyProcess = Process.Start(startInfo);
                }
                catch (Win32Exception ex)
                {
                    Logger.Error(ex, "Failed to start chocolatey gui subprocess.");
                    throw new ApplicationException(
                        $"Failed to elevate chocolatey: {ex.Message}.");
                }

                Debug.Assert(_chocolateyProcess != null, "_chocolateyProcess != null");

                if (_chocolateyProcess.WaitForExit(500))
                {
                    LogError();
                }

                _chocolateyService = CreateClient();

                // ReSharper disable once PossibleNullReferenceException
                Elevation.Instance.IsElevated = await _chocolateyService.IsElevated();
            }
        }

        private void LogError()
        {
            Log.Logger.Fatal(
                "Failed to start Chocolatey subprocess. Exit Code {ExitCode}.",
                _chocolateyProcess.ExitCode);
            throw new ApplicationException($"Failed to start chocolatey subprocess.\n"
                                           +
                                           $"You can check the log file at {Path.Combine(Bootstrapper.AppDataPath, "ChocolateyGui.Subprocess.[Date].log")} for errors");
        }

        private IIpcChocolateyService CreateClient()
        {
            try
            {
                var callback = new ServiceCallbackHandler(_progressService);
                var context = new InstanceContext(callback);
                var endpoint = new EndpointAddress(IpcDefaults.DefaultServiceUri);
                var channel =
                    new DuplexChannelFactory<IIpcChocolateyService>(context, IpcDefaults.DefaultBinding, endpoint).CreateChannel();
                channel.Register();
                return channel;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Failed to create client channel to server.");
                throw;
            }
        }
    }
}