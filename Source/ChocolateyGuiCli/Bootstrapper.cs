// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using Autofac;
using chocolatey.infrastructure.filesystem;
using ChocolateyGui.Common;
using ChocolateyGui.Common.Startup;
using ChocolateyGui.Common.Utilities;
using Serilog;
using Serilog.Events;

namespace ChocolateyGuiCli
{
    public static class Bootstrapper
    {
        private static readonly IFileSystem _fileSystem = new DotNetFileSystem();

#pragma warning disable SA1202
        public static readonly string ChocolateyGuiInstallLocation = _fileSystem.get_directory_name(_fileSystem.get_current_assembly_path());
        public static readonly string ChocolateyInstallEnvironmentVariableName = "ChocolateyInstall";
        public static readonly string ChocolateyInstallLocation = System.Environment.GetEnvironmentVariable(ChocolateyInstallEnvironmentVariableName) ?? _fileSystem.get_directory_name(_fileSystem.get_current_assembly_path());
        public static readonly string LicensedGuiAssemblyLocation = _fileSystem.combine_paths(ChocolateyInstallLocation, "extensions", "chocolateygui", "chocolateygui.licensed.dll");

        public static readonly string ChocolateyGuiCommonAssemblyLocation = _fileSystem.combine_paths(ChocolateyGuiInstallLocation, "ChocolateyGui.Common.dll");
        public static readonly string ChocolateyGuiCommonWindowsAssemblyLocation = _fileSystem.combine_paths(ChocolateyGuiInstallLocation, "ChocolateyGui.Common.Windows.dll");

        public static readonly string ChocolateyGuiCommonAssemblySimpleName = "ChocolateyGui.Common";
        public static readonly string ChocolateyGuiCommonWindowsAssemblySimpleName = "ChocolateyGui.Common.Windows";

        public static readonly string UnofficialChocolateyPublicKey = "ffc115b9f4eb5c26";
        public static readonly string OfficialChocolateyPublicKey = "dfd1909b30b79d8b";

        public static readonly string Name = "Chocolatey GUI";

        public static readonly string LicensedChocolateyGuiAssemblySimpleName = "chocolateygui.licensed";
#pragma warning restore SA1202

        internal static ILogger Logger { get; private set; }

        internal static IContainer Container { get; private set; }

        internal static string AppDataPath { get; } = LogSetup.GetAppDataPath(Name);

        internal static string LocalAppDataPath { get; } = LogSetup.GetLocalAppDataPath(Name);

        internal static string UserConfigurationDatabaseName { get; } = "UserDatabase";

        internal static string GlobalConfigurationDatabaseName { get; } = "GlobalDatabase";

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

            Container = AutoFacConfiguration.RegisterAutoFac(LicensedChocolateyGuiAssemblySimpleName, LicensedGuiAssemblyLocation);
        }
    }
}