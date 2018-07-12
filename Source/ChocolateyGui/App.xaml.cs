// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using chocolatey;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.commandline;
using ChocolateyGui.Attributes;
using ChocolateyGui.Models;
using ChocolateyGui.Services;
using ChocolateyGui.Utilities;
using LiteDB;
using Log = Serilog.Log;

namespace ChocolateyGui
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal const string ApplicationName = "Chocolatey GUI";
        private const string NoChangeMessage = "Nothing to change. Config already set.";

        public App()
        {
            InitializeComponent();
        }

        internal static SplashScreen SplashScreen { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                ParseArgumentsAndRunCommand(args);
            }
            else
            {
                FreeConsole();
                var dpi = NativeMethods.GetScaleFactor();
                var img = "chocolatey.png";
                if (dpi >= 2f)
                {
                    img = "chocolatey@3.png";
                }
                else if (dpi > 1.00f)
                {
                    img = "chocolatey@2.png";
                }

                SplashScreen = new SplashScreen(img);
                SplashScreen.Show(true, true);

                var application = new App();
                application.InitializeComponent();
                try
                {
                    application.Run();
                }
                catch (Exception ex)
                {
                    if (Bootstrapper.IsExiting)
                    {
                        Log.Logger?.Error(ex, "Exception propagated to root while shutting down.");
                        return;
                    }

                    throw;
                }
            }
        }

        // Usage of this PInvoke came from this blog post:
        // https://blog.rsuter.com/write-application-can-act-console-application-wpf-gui-application/
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();

        private static void ParseArgumentsAndRunCommand(string[] args)
        {
            var commandName = string.Empty;
            IList<string> unparsedArguments = new List<string>();
            var featureName = string.Empty;
            var optionSet = new OptionSet();
            var helpRequested = false;
            var regularOutput = true;

            optionSet
                .Add(
                    "?|help|h",
                    "Prints out the help menu.",
                    option => helpRequested = option != null)
                .Add(
                    "n=|name=",
                    "Name - the name of the feature. Defaults to empty.",
                    option => featureName = option.remove_surrounding_quotes())
                .Add(
                    "r|limitoutput|limit-output",
                    "LimitOutput - Limit the output to essential information",
                    option => regularOutput = option == null);

            IList<string> commandArgs = new List<string>();

            // shift the first arg off
            int count = 0;
            foreach (var arg in args)
            {
                if (count == 0)
                {
                    count += 1;
                    continue;
                }

                commandArgs.Add(arg);
            }

            try
            {
                unparsedArguments = optionSet.Parse(commandArgs);
            }
            catch (OptionException)
            {
                ShowHelpMessage(optionSet);
            }

            if (helpRequested)
            {
                ShowHelpMessage(optionSet);
                Environment.Exit(0);
            }

            if (unparsedArguments.Count > 1)
            {
                ShowHelpMessage(optionSet);
                throw new ApplicationException("A single feature command must be listed.");
            }

            // the command argument
            commandName = args.FirstOrDefault();
            if (commandName != "feature")
            {
                ShowHelpMessage(optionSet);
                return;
            }

            var featureCommand = FeatureCommandType.unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault();
            Enum.TryParse(unparsedCommand, true, out featureCommand);
            if (featureCommand == FeatureCommandType.unknown)
            {
                featureCommand = FeatureCommandType.list;
            }

            // let's grab the current configuration database
            var localAppDataPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                ApplicationName);

            if (!Directory.Exists(localAppDataPath))
            {
                Directory.CreateDirectory(localAppDataPath);
            }

            var database = new LiteDatabase($"filename={Path.Combine(localAppDataPath, "data.db")};upgrade=true");
            var configService = new ConfigService(database);
            var configuration = configService.GetSettings();

            switch (featureCommand)
            {
                case FeatureCommandType.list:
                    ListFeatures(configuration, regularOutput);
                    break;
                case FeatureCommandType.disable:
                    DisableFeature(configService, configuration, featureName);
                    break;
                case FeatureCommandType.enable:
                    EnableFeature(configService, configuration, featureName);
                    break;
            }
        }

        private static void ListFeatures(AppConfiguration configuration, bool regularOutput)
        {
            var properties = typeof(AppConfiguration).GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;

                if (property.Name != "Id")
                {
                    var attributes = property.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), true);
                    var attribute = attributes.Length > 0 ? (LocalizedDescriptionAttribute)attributes[0] : null;
                    var descriptionText = attribute?.Description;

                    var propertyValue = (bool)property.GetValue(configuration);

                    if (regularOutput)
                    {
                        Console.WriteLine("{0} {1} - {2}".format_with(propertyValue ? "[x]" : "[ ]", propertyName, descriptionText));
                    }
                    else
                    {
                        Console.WriteLine("{0}|{1}|{2}".format_with(propertyName, !propertyValue ? "Disabled" : "Enabled", descriptionText));
                    }
                }
            }
        }

        private static PropertyInfo GetFeatureProperty(string featureName)
        {
            var featureProperty = typeof(AppConfiguration).GetProperties().FirstOrDefault(f => f.Name.ToLowerInvariant() == featureName.ToLowerInvariant());

            if (featureProperty == null)
            {
                Console.WriteLine("Feature '{0}' not found", featureName);
                Environment.Exit(-1);
            }

            return featureProperty;
        }

        private static bool GetFeatureValue(AppConfiguration configuration, PropertyInfo featureProperty)
        {
            var featureValue = (bool)featureProperty.GetValue(configuration);
            return featureValue;
        }

        private static void DisableFeature(IConfigService configService, AppConfiguration configuration, string featureName)
        {
            var featureProperty = GetFeatureProperty(featureName);
            var featureValue = GetFeatureValue(configuration, featureProperty);

            if (featureValue)
            {
                featureProperty.SetValue(configuration, false);
                configService.UpdateSettings(configuration);
                Console.WriteLine("Disabled {0}", featureName);
            }
            else
            {
                Console.WriteLine(NoChangeMessage);
            }
        }

        private static void EnableFeature(IConfigService configService, AppConfiguration configuration, string featureName)
        {
            var featureProperty = GetFeatureProperty(featureName);
            var featureValue = GetFeatureValue(configuration, featureProperty);

            if (!featureValue)
            {
                featureProperty.SetValue(configuration, true);
                configService.UpdateSettings(configuration);
                Console.WriteLine("Enabled {0}", featureName);
            }
            else
            {
                Console.WriteLine(NoChangeMessage);
            }
        }

        private static void ShowHelpMessage(OptionSet optionSet)
        {
            Console.WriteLine();
            Console.WriteLine("Feature Command");
            Console.WriteLine(@"
Chocolatey GUI will allow you to interact with features.
");

            Console.WriteLine("Usage");
            Console.WriteLine(@"
    chocolateygui feature [list]|disable|enable <options/switches>]
");

            Console.WriteLine("Examples");
            Console.WriteLine(@"
    chocolateygui feature
    chocolateygui feature list
    chocolateygui feature disable -n=ShowConsoleOutput
    chocolateygui feature enable -n=ShowConsoleOutput
");

            Console.WriteLine("Options and Switches");

            optionSet.WriteOptionDescriptions(Console.Out);
        }
    }
}