// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="App.xaml.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Autofac;
using chocolatey;
using chocolatey.infrastructure.commandline;
using ChocolateyGui.Attributes;
using ChocolateyGui.CliCommands;
using ChocolateyGui.Utilities;

namespace ChocolateyGui
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal const string ApplicationName = "Chocolatey GUI";
        private static readonly OptionSet _optionSet = new OptionSet();
        private static readonly App _application = new App();

        public static OptionSet OptionSet
        {
            get { return _optionSet; }
        }

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
                var commandName = string.Empty;
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

                var configuration = new ChocolateyGuiConfiguration();
                SetUpGlobalOptions(args, configuration, Bootstrapper.Container);

                var runner = new GenericRunner();
                runner.run(configuration, Bootstrapper.Container, command =>
                {
                    ParseArgumentsAndUpdateConfiguration(
                        commandArgs,
                        configuration,
                        (optionSet) => command.configure_argument_parser(optionSet, configuration),
                        (unparsedArgs) =>
                        {
                            command.handle_additional_argument_parsing(unparsedArgs, configuration);
                        },
                        () =>
                        {
                            Bootstrapper.Logger.Debug("Performing validation checks...");
                            command.handle_validation(configuration);
                        },
                        () => command.help_message(configuration)
                    );
                });
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

                _application.InitializeComponent();
                try
                {
                    _application.Run();
                }
                catch (Exception ex)
                {
                    if (Bootstrapper.IsExiting)
                    {
                        Bootstrapper.Logger.Error(ex, ChocolateyGui.Properties.Resources.Command_GeneralError);
                        return;
                    }

                    throw;
                }
            }
        }

        private static void SetUpGlobalOptions(IList<string> args, ChocolateyGuiConfiguration configuration, IContainer container)
        {
            ParseArgumentsAndUpdateConfiguration(
                args,
                configuration,
                (option_set) =>
                {
                    option_set
                        .Add("r|limitoutput|limit-output",
                            ChocolateyGui.Properties.Resources.Command_LimitOutputOption,
                            option => configuration.RegularOutput = option == null);
                },
                (unparsedArgs) =>
                {
                    if (!string.IsNullOrWhiteSpace(configuration.CommandName))
                    {
                        // save help for next menu
                        configuration.HelpRequested = false;
                        configuration.UnsuccessfulParsing = false;
                    }
                },
                () => { },
                () =>
                {
                    var commandsLog = new StringBuilder();
                    var commands = container.Resolve<IEnumerable<ICommand>>();
                    foreach (var command in commands.or_empty_list_if_null())
                    {
                        var attributes = command.GetType().GetCustomAttributes(typeof(LocalizedCommandForAttribute), false).Cast<LocalizedCommandForAttribute>();
                        foreach (var attribute in attributes.or_empty_list_if_null())
                        {
                            commandsLog.AppendFormat(" * {0} - {1}\n", attribute.CommandName, attribute.Description);
                        }
                    }

                    Bootstrapper.Logger.Information(ChocolateyGui.Properties.Resources.Command_CommandsListText.format_with("chocolateygui"));
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Warning(ChocolateyGui.Properties.Resources.Command_CommandsTitle);
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Information("{0}".format_with(commandsLog.ToString()));
                    Bootstrapper.Logger.Information(ChocolateyGui.Properties.Resources.Command_CommandsText.format_with("chocolateygui command -help"));
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Warning(ChocolateyGui.Properties.Resources.Command_DefaultOptionsAndSwitches);
                }
            );
        }

        // Usage of this PInvoke came from this blog post:
        // https://blog.rsuter.com/write-application-can-act-console-application-wpf-gui-application/
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();

        private static void ParseArgumentsAndUpdateConfiguration(ICollection<string> args,
            ChocolateyGuiConfiguration configuration,
            Action<OptionSet> setOptions,
            Action<IList<string>> afterParse,
            Action validateConfiguration,
            Action helpMessage)
        {
            IList<string> unparsedArguments = new List<string>();

            // add help only once
            if (_optionSet.Count == 0)
            {
                _optionSet
                    .Add("?|help|h",
                        ChocolateyGui.Properties.Resources.Command_HelpOption,
                        option => configuration.HelpRequested = option != null);
            }

            if (setOptions != null)
            {
                setOptions(_optionSet);
            }

            try
            {
                unparsedArguments = _optionSet.Parse(args);
            }
            catch (OptionException)
            {
                ShowHelp(_optionSet, helpMessage);
                configuration.UnsuccessfulParsing = true;
            }

            // the command argument
            if (string.IsNullOrWhiteSpace(configuration.CommandName) &&
                unparsedArguments.Contains(args.FirstOrDefault()))
            {
                var commandName = args.FirstOrDefault();
                if (!Regex.IsMatch(commandName, @"^[-\/+]"))
                {
                    configuration.CommandName = commandName;
                }
                else if (commandName.is_equal_to("-v") || commandName.is_equal_to("--version"))
                {
                    // skip help menu
                }
                else
                {
                    configuration.HelpRequested = true;
                    configuration.UnsuccessfulParsing = true;
                }
            }

            if (afterParse != null)
            {
                afterParse(unparsedArguments);
            }

            if (configuration.HelpRequested)
            {
                ShowHelp(_optionSet, helpMessage);
            }
            else
            {
                if (validateConfiguration != null)
                {
                    validateConfiguration();
                }
            }
        }

        private static void ShowHelp(OptionSet optionSet, Action helpMessage)
        {
            if (helpMessage != null)
            {
                helpMessage.Invoke();
            }

            optionSet.WriteOptionDescriptions(Console.Out);
        }
    }
}