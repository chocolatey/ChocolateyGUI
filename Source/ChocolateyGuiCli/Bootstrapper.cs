// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using Autofac;
using ChocolateyGui.Common;
using ChocolateyGui.Common.Startup;
using ChocolateyGui.Common.Utilities;
using Serilog;
using Serilog.Events;

namespace ChocolateyGuiCli
{
    public static class Bootstrapper
    {
        internal static ILogger Logger { get; private set; }

        internal static IContainer Container { get; private set; }

        internal static string AppDataPath { get; } = LogSetup.GetAppDataPath(ApplicationParameters.Name);

        internal static string LocalAppDataPath { get; } = LogSetup.GetLocalAppDataPath(ApplicationParameters.Name);

        internal static void Configure()
        {
            var logPath = LogSetup.GetLogsFolderPath("Logs");

            LogSetup.Execute();

            var directPath = Path.Combine(logPath, "ChocolateyGuiCli.{Date}.log");

            var logConfig = new LoggerConfiguration()
                .WriteTo.Sink(new ColouredConsoleSink(), LogEventLevel.Information)
                .WriteTo.Async(config =>
                    config.RollingFile(directPath, retainedFileCountLimit: 10, fileSizeLimitBytes: 150 * 1000 * 1000))
                .SetDefaultLevel();

            Logger = Log.Logger = logConfig.CreateLogger();

            Container = AutoFacConfiguration.RegisterAutoFac();
        }
    }
}