// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigCommand.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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
using Serilog;

namespace ChocolateyGui.Common.Commands
{
    [LocalizedCommandFor("config", "ConfigCommand_Description")]
    public class ConfigCommand : BaseCommand, ICommand
    {
        private static readonly ILogger Logger = Log.ForContext<ConfigCommand>();
        private readonly IConfigService _configService;

        public ConfigCommand(IConfigService configService)
        {
            _configService = configService;
        }

        public virtual void ConfigureArgumentParser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
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
                .Add(
                    "g|global",
                    Resources.GlobalOption,
                    option => configuration.Global = option != null);
        }

        public virtual void HandleAdditionalArgumentParsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            var command = ConfigCommandType.Unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault().to_string().Replace("-", string.Empty);
            Enum.TryParse(unparsedCommand, true, out command);
            if (command == ConfigCommandType.Unknown)
            {
                if (!string.IsNullOrWhiteSpace(unparsedCommand))
                {
                    Logger.Warning(Resources.ConfigCommand_UnknownCommandError.format_with(unparsedCommand, "list"));
                }

                command = ConfigCommandType.List;
            }

            configuration.ConfigCommand.Command = command;

            if ((configuration.ConfigCommand.Command == ConfigCommandType.List || !string.IsNullOrWhiteSpace(configuration.ConfigCommand.Name)) && unparsedArguments.Count > 1)
            {
                Logger.Error(Resources.ConfigCommand_SingleConfigError);
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

        public virtual void HandleValidation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.ConfigCommand.Command != ConfigCommandType.List &&
                string.IsNullOrWhiteSpace(configuration.ConfigCommand.Name))
            {
                Logger.Error(Resources.ConfigCommand_MissingNameOptionError.format_with(configuration.ConfigCommand.Command.to_string(), "--name"));
                Environment.Exit(-1);
            }

            if (configuration.ConfigCommand.Command == ConfigCommandType.Set &&
                string.IsNullOrWhiteSpace(configuration.ConfigCommand.ConfigValue))
            {
                Logger.Error(Resources.ConfigCommand_MissingValueOptionError.format_with(configuration.ConfigCommand.Command.to_string(), "--value"));
                Environment.Exit(-1);
            }
        }

        public virtual void HelpMessage(ChocolateyGuiConfiguration configuration)
        {
            Logger.Warning(Resources.ConfigCommand_Title);
            Logger.Information(string.Empty);
            Logger.Information(Resources.ConfigCommand_Help);
            Logger.Information(string.Empty);
            Logger.Warning(Resources.Command_Usage);
            Logger.Information(@"
    chocolateyguicli config [list]|get|set|unset [<options/switches>]
");

            Logger.Warning(Resources.Command_Examples);
            Logger.Information(@"
    chocolateyguicli config
    chocolateyguicli config list
    chocolateyguicli config get outdatedPackagesCacheDurationInMinutes
    chocolateyguicli config get --name outdatedPackagesCacheDurationInMinutes
    chocolateyguicli config set outdatedPackagesCacheDurationInMinutes 60
    chocolateyguicli config set --name outdatedPackagesCacheDurationInMinutes --value 60
    chocolateyguicli config unset outdatedPackagesCacheDurationInMinutes
    chocolateyguicli config unset --name outdatedPackagesCacheDurationInMinutes
");
            PrintExitCodeInformation();
        }

        public virtual void Run(ChocolateyGuiConfiguration config)
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