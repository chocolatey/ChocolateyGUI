// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigCommand.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using chocolatey;
using chocolatey.infrastructure.commandline;
using ChocolateyGui.Common.Attributes;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Services;

namespace ChocolateyGuiCli.Commands
{
    [LocalizedCommandFor("config", "ConfigCommand_Description")]
    public class ConfigCommand : BaseCommand, ICommand
    {
        private readonly IConfigService _configService;

        public ConfigCommand(IConfigService configService)
        {
            _configService = configService;
        }

        public void ConfigureArgumentParser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
        {
            optionSet
                .Add(
                    "name=",
                    Resources.ConfigCommand_NameOption,
                    option => configuration.ConfigCommand.Name = option.remove_surrounding_quotes())
                .Add(
                    "value=",
                    Resources.ConfigCommand_ValueOption,
                    option => configuration.ConfigCommand.ConfigValue = option.remove_surrounding_quotes());
        }

        public void HandleAdditionalArgumentParsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            var command = ConfigCommandType.Unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault().to_string().Replace("-", string.Empty);
            Enum.TryParse(unparsedCommand, true, out command);
            if (command == ConfigCommandType.Unknown)
            {
                if (!string.IsNullOrWhiteSpace(unparsedCommand))
                {
                    Bootstrapper.Logger.Warning(Resources.ConfigCommand_UnknownCommandError.format_with(unparsedCommand));
                }

                command = ConfigCommandType.List;
            }

            configuration.ConfigCommand.Command = command;

            if ((configuration.ConfigCommand.Command == ConfigCommandType.List || !string.IsNullOrWhiteSpace(configuration.ConfigCommand.Name)) && unparsedArguments.Count > 1)
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

        public void HandleValidation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.ConfigCommand.Command != ConfigCommandType.List &&
                string.IsNullOrWhiteSpace(configuration.ConfigCommand.Name))
            {
                Bootstrapper.Logger.Error(Resources.ConfigCommand_MissingNameOptionError.format_with(configuration.ConfigCommand.Command.to_string()));
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }

            if (configuration.ConfigCommand.Command == ConfigCommandType.Set &&
                string.IsNullOrWhiteSpace(configuration.ConfigCommand.ConfigValue))
            {
                Bootstrapper.Logger.Error(Resources.ConfigCommand_MissingValueOptionError.format_with(configuration.ConfigCommand.Command.to_string()));
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }
        }

        public void HelpMessage(ChocolateyGuiConfiguration configuration)
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
            PrintExitCodeInformation();
        }

        public void Run(ChocolateyGuiConfiguration config)
        {
            switch (config.ConfigCommand.Command)
            {
                case ConfigCommandType.List:
                    _configService.ListSettings(config);
                    break;
                case ConfigCommandType.Get:
                    _configService.GetConfigValue(config);
                    break;
                case ConfigCommandType.Set:
                    _configService.SetConfigValue(config);
                    break;
                case ConfigCommandType.Unset:
                    _configService.UnsetConfigValue(config);
                    break;
            }
        }
    }
}