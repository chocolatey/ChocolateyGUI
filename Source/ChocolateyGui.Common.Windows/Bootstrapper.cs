// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Bootstrapper.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using AutoMapper;
using Caliburn.Micro;
using chocolatey;
using chocolatey.infrastructure.filesystem;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Startup;
using ChocolateyGui.Common.Utilities;
using ChocolateyGui.Common.ViewModels.Items;
using ChocolateyGui.Common.Windows.Startup;
using ChocolateyGui.Common.Windows.ViewModels;
using LiteDB;
using Serilog;
using ILogger = Serilog.ILogger;
using Log = Serilog.Log;

namespace ChocolateyGui.Common.Windows
{
    public class Bootstrapper : BootstrapperBase
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

        public Bootstrapper()
        {
            Initialize();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public static IContainer Container { get; private set; }

        public static ILogger Logger { get; private set; }

        public static bool IsExiting { get; private set; }

        public static string ApplicationFilesPath { get; } = Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location);

        public static string AppDataPath { get; } = LogSetup.GetAppDataPath(Name);

        public static string LocalAppDataPath { get; } = LogSetup.GetLocalAppDataPath(Name);

        public static string UserConfigurationDatabaseName { get; } = "UserDatabase";

        public static string GlobalConfigurationDatabaseName { get; } = "GlobalDatabase";

        public Task OnExitAsync()
        {
            IsExiting = true;
            Log.CloseAndFlush();
            FinalizeDatabaseTransaction();
            Container.Dispose();
            return Task.FromResult(true);
        }

        protected override void Configure()
        {
            var logPath = LogSetup.GetLogsFolderPath("Logs");

            LogSetup.Execute();

            var directPath = Path.Combine(logPath, "ChocolateyGui.{Date}.log");

            var logConfig = new LoggerConfiguration()
                .WriteTo.Async(config =>
                    config.RollingFile(directPath, retainedFileCountLimit: 10, fileSizeLimitBytes: 150 * 1000 * 1000))
                .SetDefaultLevel();

            Logger = Log.Logger = logConfig.CreateLogger();

            Container = AutoFacConfiguration.RegisterAutoFac(LicensedChocolateyGuiAssemblySimpleName, LicensedGuiAssemblyLocation);

            Internationalization.Initialize();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                // Do not remove! Load Chocolatey once so all config gets set
                // properly for future calls
                var choco = Lets.GetChocolatey();

                Mapper.Initialize(config =>
                {
                    config.CreateMap<Package, IPackageViewModel>().ConstructUsing(rc => Container.Resolve<IPackageViewModel>());
                });

                var packageService = Container.Resolve<IChocolateyService>();
                var features = await packageService.GetFeatures();

                var backgroundFeature = features.FirstOrDefault(feature => string.Equals(feature.Name, "useBackgroundService", StringComparison.OrdinalIgnoreCase));
                var elevationProvider = Elevation.Instance;
                elevationProvider.IsBackgroundRunning = backgroundFeature?.Enabled ?? false;

                var splashScreen = Container.Resolve<ISplashScreenService>();
                splashScreen.Close(TimeSpan.FromMilliseconds(300));

                DisplayRootViewFor<ShellViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.Fatal_Startup_Error_Formatted, ex.Message));
                Logger.Fatal(ex, Resources.Fatal_Startup_Error);
                await OnExitAsync();
            }
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.IsRegistered(service))
                {
                    return Container.Resolve(service);
                }
            }
            else
            {
                if (Container.IsRegisteredWithName(key, service))
                {
                    return Container.ResolveNamed(key, service);
                }
            }

            throw new Exception(string.Format(Resources.Application_ContainerError, key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            Logger.Information(Resources.Application_Exiting);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            FinalizeDatabaseTransaction();
            if (e.IsTerminating)
            {
                Logger.Fatal(Resources.Application_UnhandledException, e.ExceptionObject as Exception);
                if (IsExiting)
                {
                    return;
                }

                MessageBox.Show(
                    e.ExceptionObject.ToString(),
                    Resources.Bootstrapper_UnhandledException,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK,
                    MessageBoxOptions.ServiceNotification);
            }
            else
            {
                Logger.Error(Resources.Application_UnhandledException, e.ExceptionObject as Exception);
            }
        }

        private static void FinalizeDatabaseTransaction()
        {
            if (Container != null)
            {
                if (Container.IsRegisteredWithName<LiteDatabase>(GlobalConfigurationDatabaseName))
                {
                    var globalDatabase = Container.ResolveNamed<LiteDatabase>(GlobalConfigurationDatabaseName);
                    globalDatabase.Dispose();
                }

                if (Container.IsRegisteredWithName<LiteDatabase>(UserConfigurationDatabaseName))
                {
                    var userDatabase = Container.ResolveNamed<LiteDatabase>(UserConfigurationDatabaseName);
                    userDatabase.Dispose();
                }
            }
        }
    }
}