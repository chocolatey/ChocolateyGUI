using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using Autofac;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.IoC;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.Properties;
using Chocolatey.Gui.Services;
using Chocolatey.Gui.Utilities.Extensions;
using Chocolatey.Gui.ViewModels.Items;
using log4net;

namespace Chocolatey.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal static IContainer Container { get; private set; }

        private static ILogService Log { get; set; }

        static App()
        {
            Container = AutoFacConfiguration.RegisterAutoFac();

            Log = typeof (App).GetLogger();

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Log.Info("Starting...");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.InfoFormat("Exiting with code {0}.", e.ApplicationExitCode);
            Log.ForceFlush();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if(e.IsTerminating)
                Log.Fatal("Unhandled Exception", e.ExceptionObject as Exception);
            else 
                Log.Error("Unhandled Exception", e.ExceptionObject as Exception);

            MessageBox.Show(e.ExceptionObject.ToString(), "First Chance Exception", MessageBoxButton.OK, MessageBoxImage.Error,
                MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        }

        static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Log.Debug("First Chance Exception", e.Exception);
        }
    }
}
