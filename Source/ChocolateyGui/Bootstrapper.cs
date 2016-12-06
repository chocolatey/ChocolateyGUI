// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Bootstrapper.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using chocolatey;
using ChocolateyGui.IoC;
using ChocolateyGui.ViewModels;
using Serilog;

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

        internal static ILogger Log { get; private set; }

        internal static string AppDataPath { get; private set; }

        protected override void Configure()
        {
            Container = AutoFacConfiguration.RegisterAutoFac();
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,
                Environment.SpecialFolderOption.DoNotVerify);
            AppDataPath = Path.Combine(appDataFolder, "ChocolateyGUI");
            var logPath = Path.Combine(AppDataPath, "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            var directPath = Path.Combine(logPath, "{Date}.log");

            Log = Serilog.Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#endif
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile(directPath)
                .CreateLogger();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
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
            Log.Information("Exiting.");
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
                Log.Fatal("Unhandled Exception", e.ExceptionObject as Exception);
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
                Log.Error("Unhandled Exception", e.ExceptionObject as Exception);
            }
        }
    }
}