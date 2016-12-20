// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Bootstrapper.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using CefSharp;
using chocolatey;
using ChocolateyGui.Startup;
using ChocolateyGui.ViewModels;
using Serilog;
using Serilog.Events;

namespace ChocolateyGui
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        internal static IContainer Container { get; private set; }

        internal static ILogger Logger { get; private set; }

        internal static string AppDataPath { get; private set; }

        internal static string LocalAppDataPath { get; private set; }

        internal const string ApplicationName = "ChocolateyGUI";

        public Task OnExitAsync()
        {
            Log.CloseAndFlush();
            Cef.Shutdown();
            Container.Dispose();
            return Task.FromResult(true);
        }

        protected override void Configure()
        {
            LocalAppDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                ApplicationName);

            if (!Directory.Exists(LocalAppDataPath))
            {
                Directory.CreateDirectory(LocalAppDataPath);
            }

            AppDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                ApplicationName);

            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }

            Container = AutoFacConfiguration.RegisterAutoFac();
            var logPath = Path.Combine(AppDataPath, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            var directPath = Path.Combine(logPath, "ChocolateyGui.{Date}.log");

            Logger = Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#endif
                // Wamp gets *very* noise. Comment out at your own peril
                .MinimumLevel.Override("WampSharp", LogEventLevel.Information)
                .WriteTo.Async(config => config.LiterateConsole())
                .WriteTo.Async(config =>
                    config.RollingFile(directPath, retainedFileCountLimit: 10, fileSizeLimitBytes: 150 * 1000 * 1000))
                .CreateLogger();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            App.SplashScreen.Close(TimeSpan.FromMilliseconds(300));
            DisplayRootViewFor<ShellViewModel>();
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

        // Monkey patch for confliciting versions of Reactive in Chocolatey and ChocolateyGUI.
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return args.Name ==
                   "System.Reactive.PlatformServices, Version=0.9.10.0, Culture=neutral, PublicKeyToken=79d02ea9cad655eb"
                ? typeof(Lets).Assembly
                : null;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Logger.Fatal("Unhandled Exception", e.ExceptionObject as Exception);
                MessageBox.Show(
                    e.ExceptionObject.ToString(),
                    "Unhandled Exception",
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