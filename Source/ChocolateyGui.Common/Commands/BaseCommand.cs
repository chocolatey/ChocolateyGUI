// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BaseCommand.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Utilities;
using Serilog;

namespace ChocolateyGui.Common.Commands
{
    public abstract class BaseCommand
    {
        private static readonly ILogger Logger = Log.ForContext<BaseCommand>();
        private static readonly TranslationSource TranslationSource = TranslationSource.Instance;

        protected static void PrintExitCodeInformation()
        {
            Logger.Warning(L(nameof(Resources.Command_ExitCodesTitle)));
            Logger.Information(string.Empty);
            Logger.Information(L(nameof(Resources.Command_ExitCodesText)));
            Logger.Information(string.Empty);
            Logger.Warning(L(nameof(Resources.Command_OptionsAndSwitches)));
        }

        protected static string L(string key)
        {
            return TranslationSource[key];
        }

        protected static string L(string key, params object[] parameters)
        {
            return TranslationSource[key, parameters];
        }
    }
}