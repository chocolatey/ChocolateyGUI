// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PurgeCommand.cs">
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
    [LocalizedCommandFor("purge", "PurgeCommand_Description")]
    public class PurgeCommand : BaseCommand, ICommand
    {
        private static readonly ILogger Logger = Log.ForContext<PurgeCommand>();
        private readonly IChocolateyGuiCacheService _chocolateyGuiCacheService;

        public PurgeCommand(IChocolateyGuiCacheService chocolateyGuiCacheService)
        {
            _chocolateyGuiCacheService = chocolateyGuiCacheService;
        }

        public virtual void ConfigureArgumentParser(OptionSet optionSet, ChocolateyGuiConfiguration configuration)
        {
            // There are no additional options for this command currently
        }

        public virtual void HandleAdditionalArgumentParsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration)
        {
            configuration.Input = string.Join(" ", unparsedArguments);

            if (unparsedArguments.Count > 1)
            {
                Environment.Exit(-1);
            }

            var command = PurgeCommandType.Unknown;
            var unparsedCommand = unparsedArguments.DefaultIfEmpty(string.Empty).FirstOrDefault();
            Enum.TryParse(unparsedCommand, true, out command);
            configuration.PurgeCommand.Command = command;
        }

        public virtual void HandleValidation(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.PurgeCommand.Command == PurgeCommandType.Unknown)
            {
                Logger.Error(Resources.PurgeCommand_UnknownCommandError.format_with(configuration.Input, "icons", "outdated"));
                Environment.Exit(-1);
            }
        }

        public virtual void HelpMessage(ChocolateyGuiConfiguration configuration)
        {
            Logger.Warning(Resources.PurgeCommand_Title);
            Logger.Information(string.Empty);
            Logger.Information(Resources.PurgeCommand_Help);
            Logger.Information(string.Empty);
            Logger.Warning(Resources.Command_Usage);
            Logger.Information(@"
    chocolateyguicli pruge icons|outdated [<options/switches>]
");
            Logger.Warning(Resources.Command_Examples);
            Logger.Information(@"
    chocolateyguicli purge icons
    chocolateyguicli purge outdated
");

            PrintExitCodeInformation();
        }

        public virtual void Run(ChocolateyGuiConfiguration config)
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