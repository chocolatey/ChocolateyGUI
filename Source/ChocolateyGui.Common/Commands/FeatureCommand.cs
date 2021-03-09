// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="FeatureCommand.cs">
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
    [LocalizedCommandFor("feature", "FeatureCommand_Description")]
    public class FeatureCommand : BaseCommand, ICommand
    {
        private static readonly ILogger Logger = Log.ForContext<FeatureCommand>();
        private readonly IConfigService _configService;

        public FeatureCommand(IConfigService configService)
        {
            _configService = configService;
        }

        public virtual void ConfigureArgumentParser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
        {
            optionSet
                .Add(
                    "n=|name=",
                    Resources.FeatureCommand_NameOption,
                    option => configuration.FeatureCommand.Name = option.remove_surrounding_quotes())
                .Add(
                    "g|global",
                    Resources.GlobalOption,
                    option => configuration.Global = option != null);
        }

        public virtual void HandleAdditionalArgumentParsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            if (unparsedArguments.Count > 1)
            {
                Logger.Error(Resources.FeatureCommand_SingleFeatureError);
                Environment.Exit(-1);
            }

            var command = FeatureCommandType.Unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault();
            Enum.TryParse(unparsedCommand, true, out command);
            if (command == FeatureCommandType.Unknown)
            {
                if (!string.IsNullOrWhiteSpace(unparsedCommand))
                {
                    Logger.Warning(Resources.FeatureCommand_UnknownCommandError.format_with(unparsedCommand, "list"));
                }

                command = FeatureCommandType.List;
            }

            configuration.FeatureCommand.Command = command;
        }

        public virtual void HandleValidation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.FeatureCommand.Command != FeatureCommandType.List && string.IsNullOrWhiteSpace(configuration.FeatureCommand.Name))
            {
                Logger.Error(Resources.FeatureCommand_MissingNameOptionError.format_with(configuration.FeatureCommand.Command.to_string(), "--name"));
                Environment.Exit(-1);
            }
        }

        public virtual void HelpMessage(ChocolateyGuiConfiguration configuration)
        {
            Logger.Warning(Resources.FeatureCommand_Title);
            Logger.Information(string.Empty);
            Logger.Information(Resources.FeatureCommand_Help);
            Logger.Information(string.Empty);
            Logger.Warning(Resources.Command_Usage);
            Logger.Information(@"
    chocolateyguicli feature [list]|disable|enable [<options/switches>]
");
            Logger.Warning(Resources.Command_Examples);
            Logger.Information(@"
    chocolateyguicli feature
    chocolateyguicli feature list
    chocolateyguicli feature disable -n=ShowConsoleOutput
    chocolateyguicli feature enable -n=ShowConsoleOutput
");

            PrintExitCodeInformation();
        }

        public virtual void Run(ChocolateyGuiConfiguration config)
        {
            switch (config.FeatureCommand.Command)
            {
                case FeatureCommandType.List:
                    _configService.ListFeatures(config);
                    break;
                case FeatureCommandType.Disable:
                    _configService.ToggleFeature(config, false);
                    break;
                case FeatureCommandType.Enable:
                    _configService.ToggleFeature(config, true);
                    break;
            }
        }
    }
}