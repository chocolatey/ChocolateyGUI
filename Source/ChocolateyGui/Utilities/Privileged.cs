// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Privileged.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace ChocolateyGui.Utilities
{
    public static class Privileged
    {
        private static readonly Lazy<bool> IsElevatedLazy = 
            new Lazy<bool>(() => WindowsIdentity.GetCurrent().Owner?.
                IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) ?? false);

        public static bool IsElevated => IsElevatedLazy.Value;

        public static bool Elevate(string arguments = null)
        {
            var proc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = ExecutablePath,
                Verb = "runas"
            };

            if (arguments != null)
            {
                proc.Arguments = arguments;
            }

            try
            {
                Process.Start(proc);
            }
            catch (Win32Exception ex)
            {
                if (ex.Message == "The operation was canceled by the user")
                {
                    return false;
                }

                throw;
            }

            return true;
        }

        private static readonly Lazy<string> ExecutablePathLazy = 
            new Lazy<string>(() => new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

        public static string ExecutablePath
            => ExecutablePathLazy.Value;
    }
}
