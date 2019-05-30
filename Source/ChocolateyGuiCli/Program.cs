// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Program.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Autofac;
using chocolatey;
using chocolatey.infrastructure.adapters;
using chocolatey.infrastructure.commandline;
using chocolatey.infrastructure.information;
using ChocolateyGui.Common;
using ChocolateyGui.Common.Attributes;
using ChocolateyGui.Common.Models;
using ChocolateyGuiCli.Commands;
using Console = System.Console;

namespace ChocolateyGuiCli
{
    public class Program
    {
        private static readonly OptionSet _optionSet = new OptionSet();

        public static OptionSet OptionSet
        {
            get { return _optionSet; }
        }

        public static void Main(string[] args)
        {
            Bootstrapper.Configure();

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
            SetEnvironmentOptions(configuration);

            if (configuration.RegularOutput)
            {
#if DEBUG
                Bootstrapper.Logger.Information("{0} v{1} (DEBUG BUILD)".format_with(ApplicationParameters.Name, configuration.Information.ChocolateyGuiProductVersion));
#else
                Bootstrapper.Logger.Information("{0} v{1}{2}".format_with(ApplicationParameters.Name, configuration.Information.ChocolateyGuiProductVersion));
#endif

                if (args.Length == 0)
                {
                    Bootstrapper.Logger.Information("Please run 'chocolateyguicli -?' or 'chocolateyguicli <command> -?' for help menu.");
                }
            }

            var runner = new GenericRunner();
            runner.Run(configuration, Bootstrapper.Container, command =>
            {
                ParseArgumentsAndUpdateConfiguration(
                    commandArgs,
                    configuration,
                    (optionSet) => command.ConfigureArgumentParser(optionSet, configuration),
                    (unparsedArgs) =>
                    {
                        command.HandleAdditionalArgumentParsing(unparsedArgs, configuration);
                    },
                    () =>
                    {
                        Bootstrapper.Logger.Debug("Performing validation checks...");
                        command.HandleValidation(configuration);
                    },
                    () => command.HelpMessage(configuration));
            });
        }

        private static void SetUpGlobalOptions(IList<string> args, ChocolateyGuiConfiguration configuration, IContainer container)
        {
            ParseArgumentsAndUpdateConfiguration(
                args,
                configuration,
                (option_set) =>
                {
                    option_set
                        .Add(
                            "r|limitoutput|limit-output",
                            ChocolateyGui.Common.Properties.Resources.Command_LimitOutputOption,
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

                    Bootstrapper.Logger.Information(ChocolateyGui.Common.Properties.Resources.Command_CommandsListText.format_with("chocolateygui"));
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Warning(ChocolateyGui.Common.Properties.Resources.Command_CommandsTitle);
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Information("{0}".format_with(commandsLog.ToString()));
                    Bootstrapper.Logger.Information(ChocolateyGui.Common.Properties.Resources.Command_CommandsText.format_with("chocolateygui command -help"));
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Warning(ChocolateyGui.Common.Properties.Resources.Command_DefaultOptionsAndSwitches);
                });
        }

        private static void SetEnvironmentOptions(ChocolateyGuiConfiguration config)
        {
            config.Information.ChocolateyGuiVersion = VersionInformation.get_current_assembly_version(Assembly.GetCallingAssembly());
            config.Information.ChocolateyGuiProductVersion = VersionInformation.get_current_informational_version(Assembly.GetCallingAssembly());
            config.Information.FullName = Assembly.GetExecutingAssembly().FullName;
        }

        private static void ParseArgumentsAndUpdateConfiguration(
            ICollection<string> args,
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
                    .Add(
                        "?|help|h",
                        ChocolateyGui.Common.Properties.Resources.Command_HelpOption,
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