// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Program.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Autofac;
using chocolatey;
using chocolatey.infrastructure.commandline;
using chocolatey.infrastructure.registration;
using ChocolateyGui.Common.Attributes;
using ChocolateyGui.Common.Commands;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Services;
using LiteDB;
using Serilog;
using Assembly = chocolatey.infrastructure.adapters.Assembly;
using Console = System.Console;
using GenericRunner = ChocolateyGui.Common.Commands.GenericRunner;

namespace ChocolateyGuiCli
{
    public class Program
    {
        private static readonly OptionSet _optionSet = new OptionSet();
        private static ResolveEventHandler _handler = null;

        public static OptionSet OptionSet
        {
            get { return _optionSet; }
        }

        public static void Main(string[] args)
        {
            try
            {
                AddAssemblyResolver();

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
                    Bootstrapper.Logger.Warning(" (DEBUG BUILD)".format_with("Chocolatey GUI", configuration.Information.DisplayVersion));
    #else
                    Bootstrapper.Logger.Warning("{0}".format_with(configuration.Information.DisplayVersion));
    #endif

                    if (args.Length == 0)
                    {
                        Bootstrapper.Logger.Information(ChocolateyGui.Common.Properties.Resources.Command_CommandsText.format_with("chocolateyguicli"));
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
            catch (Exception ex)
            {
                Bootstrapper.Logger.Error(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();

                if (Bootstrapper.Container != null)
                {
                    if (Bootstrapper.Container.IsRegisteredWithName<LiteDatabase>(Bootstrapper.GlobalConfigurationDatabaseName))
                    {
                        var globalDatabase = Bootstrapper.Container.ResolveNamed<LiteDatabase>(Bootstrapper.GlobalConfigurationDatabaseName);
                        globalDatabase.Dispose();
                    }

                    if (Bootstrapper.Container.IsRegisteredWithName<LiteDatabase>(Bootstrapper.UserConfigurationDatabaseName))
                    {
                        var userDatabase = Bootstrapper.Container.ResolveNamed<LiteDatabase>(Bootstrapper.UserConfigurationDatabaseName);
                        userDatabase.Dispose();
                    }

                    Bootstrapper.Container.Dispose();
                }
            }
        }

        #region DupFinder Exclusion
        private static void AddAssemblyResolver()
        {
            _handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);

#if FORCE_CHOCOLATEY_OFFICIAL_KEY
                var chocolateyGuiPublicKey = Bootstrapper.OfficialChocolateyPublicKey;
#else
                var chocolateyGuiPublicKey = Bootstrapper.UnofficialChocolateyPublicKey;
#endif

                try
                {
                    if (requestedAssembly.get_public_key_token().is_equal_to(chocolateyGuiPublicKey)
                        && requestedAssembly.Name.is_equal_to(Bootstrapper.ChocolateyGuiCommonAssemblySimpleName))
                    {
                        return AssemblyResolution.resolve_or_load_assembly(
                            Bootstrapper.ChocolateyGuiCommonAssemblySimpleName,
                            requestedAssembly.get_public_key_token(),
                            Bootstrapper.ChocolateyGuiCommonAssemblyLocation).UnderlyingType;
                    }

                    if (requestedAssembly.get_public_key_token().is_equal_to(chocolateyGuiPublicKey)
                        && requestedAssembly.Name.is_equal_to(Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName))
                    {
                        return AssemblyResolution.resolve_or_load_assembly(
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName,
                            requestedAssembly.get_public_key_token(),
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblyLocation).UnderlyingType;
                    }
                }
                catch (Exception ex)
                {
                    Bootstrapper.Logger.Warning("Unable to load Chocolatey GUI assembly. {0}".format_with(ex.Message));
                }

                return null;
            };

            AppDomain.CurrentDomain.AssemblyResolve += _handler;
        }
        #endregion

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

                    Bootstrapper.Logger.Information(ChocolateyGui.Common.Properties.Resources.Command_CommandsListText.format_with("chocolateyguicli"));
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Warning(ChocolateyGui.Common.Properties.Resources.Command_CommandsTitle);
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Information("{0}".format_with(commandsLog.ToString()));
                    Bootstrapper.Logger.Information(ChocolateyGui.Common.Properties.Resources.Command_CommandsText.format_with("chocolateyguicli"));
                    Bootstrapper.Logger.Information(string.Empty);
                    Bootstrapper.Logger.Warning(ChocolateyGui.Common.Properties.Resources.Command_DefaultOptionsAndSwitches);
                });
        }

        private static void SetEnvironmentOptions(ChocolateyGuiConfiguration config)
        {
            var versionService = Bootstrapper.Container.Resolve<IVersionService>();
            config.Information.ChocolateyGuiVersion = versionService.Version;
            config.Information.ChocolateyGuiProductVersion = versionService.InformationalVersion;
            config.Information.DisplayVersion = versionService.DisplayVersion;
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