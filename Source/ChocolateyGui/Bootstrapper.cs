// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Bootstrapper.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using CefSharp;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;
using ChocolateyGui.Startup;
using ChocolateyGui.Utilities;
using ChocolateyGui.ViewModels;
using Serilog;

namespace ChocolateyGui
{
    using AutoMapper;
    using chocolatey;
    using Models;
    using ViewModels.Items;
    using ILogger = Serilog.ILogger;
    using Log = Serilog.Log;

    public class Bootstrapper : BootstrapperBase
    {
        internal const string ApplicationName = "ChocolateyGUI";

        public Bootstrapper()
        {
            Initialize();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        internal static IContainer Container { get; private set; }

        internal static ILogger Logger { get; private set; }

        internal static string AppDataPath { get; private set; }

        internal static string LocalAppDataPath { get; private set; }

        internal static bool IsExiting { get; private set; }

        public Task OnExitAsync()
        {
            IsExiting = true;
            Log.CloseAndFlush();
            Cef.Shutdown();
            Container.Dispose();
            return Task.FromResult(true);
        }

        protected override void Configure()
        {
            LocalAppDataPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                ApplicationName);

            if (!Directory.Exists(LocalAppDataPath))
            {
                Directory.CreateDirectory(LocalAppDataPath);
            }

            AppDataPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                ApplicationName);

            Container = AutoFacConfiguration.RegisterAutoFac();
            var logPath = Path.Combine(AppDataPath, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            var directPath = Path.Combine(logPath, "ChocolateyGui.{Date}.log");

            var logConfig = new LoggerConfiguration()
                .WriteTo.Async(config => config.LiterateConsole())
                .WriteTo.Async(config =>
                    config.RollingFile(directPath, retainedFileCountLimit: 10, fileSizeLimitBytes: 150 * 1000 * 1000))
                .SetDefaultLevel();

            Logger = Log.Logger = logConfig.CreateLogger();

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

                App.SplashScreen.Close(TimeSpan.FromMilliseconds(300));

                DisplayRootViewFor<ShellViewModel>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start application.\n{ex.Message}\n\nMore details available in application logs.");
                Logger.Fatal(ex, "Failed to start application.");
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

            throw new Exception($"Could not locate any instances of contract {key ?? service.Name}.");
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
            Logger.Information("Exiting.");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Logger.Fatal("Unhandled Exception", e.ExceptionObject as Exception);
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
                Logger.Error("Unhandled Exception", e.ExceptionObject as Exception);
            }
        }
    }
}