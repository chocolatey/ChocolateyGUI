// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BaseCommand.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Properties;

namespace ChocolateyGuiCli.Commands
{
    public abstract class BaseCommand
    {
        protected static void PrintExitCodeInformation()
        {
            Bootstrapper.Logger.Warning(Resources.Command_ExitCodesTitle);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Information(Resources.Command_ExitCodesText);
            Bootstrapper.Logger.Information(string.Empty);
            Bootstrapper.Logger.Warning(Resources.Command_OptionsAndSwitches);
        }
    }
}