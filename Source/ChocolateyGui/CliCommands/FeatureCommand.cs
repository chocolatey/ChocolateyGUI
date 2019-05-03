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
    [LocalizedCommandFor("feature", "FeatureCommand_Description")]
    public class FeatureCommand : ICommand
    {
        private readonly IConfigService _configService;

        public FeatureCommand(IConfigService configService)
        {
            _configService = configService;
        }

        public void configure_argument_parser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
        {
            optionSet
                .Add(
                    "n=|name=",
                    Resources.FeatureCommand_NameOption,
                    option => configuration.FeatureCommand.Name = option.remove_surrounding_quotes());
        }

        public void handle_additional_argument_parsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            if (unparsedArguments.Count > 1)
            {
                Bootstrapper.Logger.Error(Resources.FeatureCommand_SingleFeatureError);
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }

            var command = FeatureCommandType.unknown;
            string unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault();
            Enum.TryParse(unparsedCommand, true, out command);
            if (command == FeatureCommandType.unknown)
            {
                if (!string.IsNullOrWhiteSpace(unparsedCommand))
                {
                    Bootstrapper.Logger.Warning(Resources.FeatureCommand_UnknownCommandError.format_with(unparsedCommand));
                }

                command = FeatureCommandType.list;
            }

            configuration.FeatureCommand.Command = command;
        }

        public void handle_validation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.FeatureCommand.Command != FeatureCommandType.list && string.IsNullOrWhiteSpace(configuration.FeatureCommand.Name))
            {
                Bootstrapper.Logger.Error(Resources.FeatureCommand_MissingNameOptionError.format_with(configuration.FeatureCommand.Command.to_string()));
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }
        }

        public void help_message(ChocolateyGuiConfiguration configuration)
        {
            Bootstrapper.Logger.Warning(Resources.FeatureCommand_Title);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Information(Resources.FeatureCommand_Help);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Warning(Resources.Command_Usage);
            Bootstrapper.Logger.Information(@"
    chocolateygui feature [list]|disable|enable <options/switches>]
");
            Bootstrapper.Logger.Warning(Resources.Command_Examples);
            Bootstrapper.Logger.Information(@"
    chocolateygui feature
    chocolateygui feature list
    chocolateygui feature disable -n=ShowConsoleOutput
    chocolateygui feature enable -n=ShowConsoleOutput
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

            switch (config.FeatureCommand.Command)
            {
                case FeatureCommandType.list:
                    _configService.ListFeatures(config);
                    break;
                case FeatureCommandType.disable:
                    _configService.DisableFeature(config);
                    break;
                case FeatureCommandType.enable:
                    _configService.EnableFeature(config);
                    break;
            }
        }
    }
}