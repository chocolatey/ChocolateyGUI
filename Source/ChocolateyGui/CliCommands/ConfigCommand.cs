using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using chocolatey;
using chocolatey.infrastructure.commandline;
using ChocolateyGui.Attributes;
using ChocolateyGui.Models;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;
using LiteDB;

namespace ChocolateyGui.CliCommands
{
    [LocalizedCommandFor("config", "ConfigCommand_Description")]
    public class ConfigCommand : ICommand
    {
        private readonly IConfigService _configService;

        public ConfigCommand(IConfigService configService)
        {
            _configService = configService;
        }

        public void configure_argument_parser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
        {
            optionSet
                .Add(
                    "name=",
                    Resources.ConfigCommand_NameOption,
                    option => configuration.ConfigCommand.Name = option.remove_surrounding_quotes())
                .Add(
                    "value=",
                    Resources.ConfigCommand_ValueOption,
                    option => configuration.ConfigCommand.ConfigValue = option.remove_surrounding_quotes())
                ;
        }

        public void handle_additional_argument_parsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            var command = ConfigCommandType.unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault().to_string().Replace("-",string.Empty);
            Enum.TryParse(unparsedCommand, true, out command);
            if (command == ConfigCommandType.unknown)
            {
                if (!string.IsNullOrWhiteSpace(unparsedCommand))
                {
                    Bootstrapper.Logger.Warning(Resources.ConfigCommand_UnknownCommandError.format_with(unparsedCommand));
                }

                command = ConfigCommandType.list;
            }

            configuration.ConfigCommand.Command = command;

            if ((configuration.ConfigCommand.Command == ConfigCommandType.list || !string.IsNullOrWhiteSpace(configuration.ConfigCommand.Name)) && unparsedArguments.Count > 1)
            {
                Bootstrapper.Logger.Error(Resources.ConfigCommand_SingleConfigError);
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }

            if (string.IsNullOrWhiteSpace(configuration.ConfigCommand.Name) && unparsedArguments.Count >= 2)
            {
                configuration.ConfigCommand.Name = unparsedArguments[1];
            }

            if (string.IsNullOrWhiteSpace(configuration.ConfigCommand.ConfigValue) && unparsedArguments.Count >= 3)
            {
                configuration.ConfigCommand.ConfigValue = unparsedArguments[2];
            }
        }

        public void handle_validation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.ConfigCommand.Command != ConfigCommandType.list &&
                string.IsNullOrWhiteSpace(configuration.ConfigCommand.Name))
            {
                Bootstrapper.Logger.Error(Resources.ConfigCommand_MissingNameOptionError.format_with(configuration.ConfigCommand.Command.to_string()));
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }

            if (configuration.ConfigCommand.Command == ConfigCommandType.set &&
                string.IsNullOrWhiteSpace(configuration.ConfigCommand.ConfigValue))
            {
                Bootstrapper.Logger.Error(Resources.ConfigCommand_MissingValueOptionError.format_with(configuration.ConfigCommand.Command.to_string()));
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }
        }

        public void help_message(ChocolateyGuiConfiguration configuration)
        {
            Bootstrapper.Logger.Warning(Resources.ConfigCommand_Title);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Information(Resources.ConfigCommand_Help);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Warning(Resources.Command_Usage);
            Bootstrapper.Logger.Information(@"
    chocolateygui config [list]|get|set|unset [<options/switches>]
");

            Bootstrapper.Logger.Warning(Resources.Command_Examples);
            Bootstrapper.Logger.Information(@"
    chocolateygui config
    chocolateygui config list
    chocolateygui config get outdatedPackagesCacheDurationInMinutes
    chocolateygui config get --name outdatedPackagesCacheDurationInMinutes
    chocolateygui config set outdatedPackagesCacheDurationInMinutes 60
    chocolateygui config set --name outdatedPackagesCacheDurationInMinutes --value 60
    chocolateygui config unset outdatedPackagesCacheDurationInMinutes
    chocolateygui config unset --name outdatedPackagesCacheDurationInMinutes
");

            Bootstrapper.Logger.Warning(Resources.Command_ExitCodesTitle);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Information(Resources.Command_ExitCodesText);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Warning(Resources.Command_OptionsAndSwitches);
        }

        public void run(ChocolateyGuiConfiguration config)
        {
            // let's grab the current configuration database
            var localAppDataPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                App.ApplicationName);

            if (!Directory.Exists(localAppDataPath))
            {
                Directory.CreateDirectory(localAppDataPath);
            }

            switch (config.ConfigCommand.Command)
            {
                case ConfigCommandType.list:
                    _configService.ListSettings(config);
                    break;
                case ConfigCommandType.get:
                    _configService.GetConfigValue(config);
                    break;
                case ConfigCommandType.set:
                    _configService.SetConfigValue(config);
                    break;
                case ConfigCommandType.unset:
                    _configService.UnsetConfigValue(config);
                    break;
            }
        }
    }
}