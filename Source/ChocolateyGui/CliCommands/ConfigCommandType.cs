// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigCommandType.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.CliCommands
{
    public enum ConfigCommandType
    {
        Unknown,
        List,
        Get,
        Set,
        Unset,
    }
}