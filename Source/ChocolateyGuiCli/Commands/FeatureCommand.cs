// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FeatureCommand.cs">
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
    [LocalizedCommandFor("feature", "FeatureCommand_Description")]
    public class FeatureCommand : BaseCommand, ICommand
    {
        private readonly IConfigService _configService;

        public FeatureCommand(IConfigService configService)
        {
            _configService = configService;
        }

        public void ConfigureArgumentParser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
        {
            optionSet
                .Add(
                    "n=|name=",
                    Resources.FeatureCommand_NameOption,
                    option => configuration.FeatureCommand.Name = option.remove_surrounding_quotes());
        }

        public void HandleAdditionalArgumentParsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            if (unparsedArguments.Count > 1)
            {
                Bootstrapper.Logger.Error(Resources.FeatureCommand_SingleFeatureError);
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }

            var command = FeatureCommandType.Unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault();
            Enum.TryParse(unparsedCommand, true, out command);
            if (command == FeatureCommandType.Unknown)
            {
                if (!string.IsNullOrWhiteSpace(unparsedCommand))
                {
                    Bootstrapper.Logger.Warning(Resources.FeatureCommand_UnknownCommandError.format_with(unparsedCommand));
                }

                command = FeatureCommandType.List;
            }

            configuration.FeatureCommand.Command = command;
        }

        public void HandleValidation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.FeatureCommand.Command != FeatureCommandType.List && string.IsNullOrWhiteSpace(configuration.FeatureCommand.Name))
            {
                Bootstrapper.Logger.Error(Resources.FeatureCommand_MissingNameOptionError.format_with(configuration.FeatureCommand.Command.to_string()));
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }
        }

        public void HelpMessage(ChocolateyGuiConfiguration configuration)
        {
            Bootstrapper.Logger.Warning(Resources.FeatureCommand_Title);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Information(Resources.FeatureCommand_Help);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Warning(Resources.Command_Usage);
            Bootstrapper.Logger.Information(@"
    chocolateygui feature [list]|disable|enable [<options/switches>]
");
            Bootstrapper.Logger.Warning(Resources.Command_Examples);
            Bootstrapper.Logger.Information(@"
    chocolateygui feature
    chocolateygui feature list
    chocolateygui feature disable -n=ShowConsoleOutput
    chocolateygui feature enable -n=ShowConsoleOutput
");

            PrintExitCodeInformation();
        }

        public void Run(ChocolateyGuiConfiguration config)
        {
            switch (config.FeatureCommand.Command)
            {
                case FeatureCommandType.List:
                    _configService.ListFeatures(config);
                    break;
                case FeatureCommandType.Disable:
                    _configService.DisableFeature(config);
                    break;
                case FeatureCommandType.Enable:
                    _configService.EnableFeature(config);
                    break;
            }
        }
    }
}