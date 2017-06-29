// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Program.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using AutoMapper;
using chocolatey.infrastructure.app.configuration;
using ChocolateyGui.Models;
using ChocolateyGui.Utilities;
using NuGet;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Subprocess
{
    using chocolatey;

    public class Program
    {
        public static ManualResetEventSlim CanceledEvent { get; private set; }

        public static ILogger Logger { get; private set; }

        public static int Main(string[] args)
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.DoNotVerify),
                "ChocolateyGUI");

            var logFolder = Path.Combine(appDataPath, "Logs");
            var directPath = Path.Combine(logFolder, "ChocolateyGui.Subprocess.{Date}.log");

            var logConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Async(config => config.LiterateConsole())
                .WriteTo.Async(config =>
                    config.RollingFile(directPath, retainedFileCountLimit: 10, fileSizeLimitBytes: 150 * 1000 * 1000))
                .SetDefaultLevel();
            Log.Logger = logConfig.CreateLogger();
            Logger = Log.ForContext<Program>();

            var source = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                CanceledEvent.Set();
                source.Cancel();
            };

            // Do not remove! Load Chocolatey once so all config gets set
            // properly for future calls
            var choco = Lets.GetChocolatey();

            CanceledEvent = new ManualResetEventSlim();

            try
            {
                Mapper.Initialize(config =>
                {
                    config.CreateMap<IPackage, Package>();
                    config.CreateMap<ConfigFileFeatureSetting, ChocolateyFeature>();
                    config.CreateMap<ConfigFileConfigSetting, ChocolateySetting>();
                    config.CreateMap<ConfigFileSourceSetting, Models.ChocolateySource>();
                });

                Logger.Information("Starting Chocolatey Server.");

                using (var host = new ServiceHost(typeof(ChocolateyService)))
                {
                    host.CloseTimeout = TimeSpan.FromMinutes(2);
                    host.AddServiceEndpoint(typeof(IIpcChocolateyService), IpcDefaults.DefaultBinding, IpcDefaults.DefaultServiceUri);

                    var timer = new Timer(Tick);
                    timer.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));

                    // Start!
                    try
                    {
                        host.Open();
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex, "Fatal error while running server. Exception: {Exception}", ex);
                        throw;
                    }

                    Logger.Information("Started chocolatey server.");
                    CanceledEvent.Wait(source.Token);
                    timer.Dispose();
                }

                Logger.Information("Stopping Chocolatey Server.");
                return 0; // Success.
            }
            catch (OperationCanceledException)
            {
                return 1223; // Cancelled.
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Fatal error while running server. Exception: {Exception}", ex);
                throw;
            }
        }

        private static void Tick(object state)
        {
            // If we don't have any clients, die.
            if (ChocolateyService.ConnectedClients <= 0)
            {
                Logger.Information("All clients have disconnected. Closing.");
                CanceledEvent.Set();
            }
        }
    }
}