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
using ChocolateyGui.Utilities;
using ChocolateyGui.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public static void UpdateSetting(string key, string value)
        {
            var path = Path.Combine(LocalAppDataPath, "appsettings.json");
            var settings = !File.Exists(path) ? new JObject() : JObject.Parse(File.ReadAllText(path));

            settings[key] = value;

            using (var writer = File.OpenWrite($"{path}.temp"))
            {
                var textWriter = new StreamWriter(writer);
                textWriter.WriteLine(JsonConvert.SerializeObject(settings, Formatting.Indented));
            }

            if (File.Exists(path))
            {
                File.Replace($"{path}.temp", path, null);
            }
            else
            {
                File.Move($"{path}.temp", path);
            }
        }

        internal static IContainer Container { get; private set; }

        internal static ILogger Log { get; private set; }

        internal static IConfigurationRoot Configuration { get; private set; }

        internal static string AppDataPath { get; private set; }

        internal static string LocalAppDataPath { get; private set; }

        protected override void Configure()
        {
            LocalAppDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                "ChocolateyGUI");

            AppDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                "ChocolateyGUI");

            var builder = new ConfigurationBuilder()
                .SetBasePath(LocalAppDataPath)
                .AddInMemoryCollection(DefaultSettings)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile(Path.Combine(AppDataPath, "appsettings.json"), optional: true);

            Configuration = builder.Build();

            Container = AutoFacConfiguration.RegisterAutoFac();
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
                .WriteTo.RollingFile(directPath, retainedFileCountLimit: 10, fileSizeLimitBytes: 150 * 1000 * 1000)
                .CreateLogger();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            if (!Configuration.GetValue<bool>("FirstRun") && Configuration.GetValue<bool>("RequireAdmin") &&
                !Privileged.IsElevated)
            {
                var rawArgs = Environment.CommandLine;
                rawArgs = rawArgs.Remove(Environment.GetCommandLineArgs()[0].Length);
                if (!Privileged.Elevate(rawArgs))
                {
                    var result =
                        MessageBox.Show(
                            "Failed to start application as administator. Would you like to continue as unelevated?",
                            "Error Elevating", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                    {
                        Application.Current.Shutdown(1);
                        return;
                    }
                }
                else
                {
                    Application.Current.Shutdown(0);
                    return;
                }
            }

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
            Log.Information("Exiting.");
        }

        private static readonly IReadOnlyDictionary<string, string> DefaultSettings = new Dictionary<string, string>
        {
            { "FirstRun", "true" },
            { "RequireAdmin", "false" }
        };

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