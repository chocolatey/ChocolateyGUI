// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PurgeCommand.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using chocolatey;
using chocolatey.infrastructure.commandline;
using ChocolateyGui.Attributes;
using ChocolateyGui.Properties;
using ChocolateyGui.Services;

namespace ChocolateyGui.CliCommands
{
    [LocalizedCommandFor("purge", "PurgeCommand_Description")]
    public class PurgeCommand : BaseCommand, ICommand
    {
        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;

        public PurgeCommand(IChocolateyGuiCacheService chocolateyGuiCacheService)
        {
            _chocolateyGuiCacheService = chocolateyGuiCacheService;
        }

        public void ConfigureArgumentParser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
        {
            // There are no additional options for this command currently
        }

        public void HandleAdditionalArgumentParsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            if (unparsedArguments.Count > 1)
            {
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }

            var command = PurgeCommandType.Unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault();
            Enum.TryParse(unparsedCommand, true, out command);
            configuration.PurgeCommand.Command = command;
        }

        public void HandleValidation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.PurgeCommand.Command == PurgeCommandType.Unknown)
            {
                Bootstrapper.Logger.Error(Resources.PurgeCommand_UnknownCommandError.format_with(configuration.Input, "icons", "outdated"));
                Bootstrapper.Container.Dispose();
                Environment.Exit(-1);
            }
        }

        public void HelpMessage(ChocolateyGuiConfiguration configuration)
        {
            Bootstrapper.Logger.Warning(Resources.PurgeCommand_Title);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Information(Resources.PurgeCommand_Help);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Warning(Resources.Command_Usage);
            Bootstrapper.Logger.Information(@"
    chocolateygui pruge icons|outdated [<options/switches>]
");
            Bootstrapper.Logger.Warning(Resources.Command_Examples);
            Bootstrapper.Logger.Information(@"
    chocolateygui purge icons
    chocolateygui purge outdated
");

            PrintExitCodeInformation();
        }

        public void Run(ChocolateyGuiConfiguration config)
        {
            switch (config.PurgeCommand.Command)
            {
                case PurgeCommandType.Icons:
                    _chocolateyGuiCacheService.PurgeIcons();
                    break;
                case PurgeCommandType.Outdated:
                    _chocolateyGuiCacheService.PurgeOutdatedPackages();
                    break;
            }
        }
    }
}