// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hacks.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Principal;

namespace ChocolateyGui.Utilities
{
    public static class Hacks
    {
        public static bool IsElevated => (WindowsIdentity.GetCurrent().Owner?.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid)).GetValueOrDefault(false);
    }
}