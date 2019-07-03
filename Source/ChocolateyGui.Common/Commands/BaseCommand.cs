// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BaseCommand.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Properties;
using Serilog;

namespace ChocolateyGui.Common.Commands
{
    public abstract class BaseCommand
    {
        private static readonly ILogger Logger = Log.ForContext<BaseCommand>();

        protected static void PrintExitCodeInformation()
        {
            Logger.Warning(Resources.Command_ExitCodesTitle);
            Logger.Information(string.Empty);
            Logger.Information(Resources.Command_ExitCodesText);
            Logger.Information(string.Empty);
            Logger.Warning(Resources.Command_OptionsAndSwitches);
        }
    }
}