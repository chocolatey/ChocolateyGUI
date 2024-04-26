// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hacks.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ChocolateyGui.Common
{
    public static class Hacks
    {
        public static bool IsElevated => (WindowsIdentity.GetCurrent().Owner?.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid)).GetValueOrDefault(false);

        // TODO: Replace this LockDirectory with calls to DotNetFileSystem's LockDirectory when https://github.com/chocolatey/ChocolateyGUI/issues/1046 is completed.
        /// <summary>
        /// Lock the given directory path to just Administrators being able to write. This method is copied from chocolatey.infrastructure.filesystem.DotNetFileSystem, and should be replaced with that call when the minimum Chocolatey.lib is bumped to 2.2.0 or greater.
        /// </summary>
        /// <param name="directoryPath">Directory path to lock down.</param>
        public static void LockDirectory(string directoryPath)
        {
            var permissions = Directory.GetAccessControl(directoryPath);
            var rules = permissions.GetAccessRules(includeExplicit: true, includeInherited: true, targetType: typeof(NTAccount));

            // We first need to remove all rules
            foreach (FileSystemAccessRule rule in rules)
            {
                permissions.RemoveAccessRuleAll(rule);
            }

            var bultinAdmins = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount));
            var localsystem = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null).Translate(typeof(NTAccount));
            var builtinUsers = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null).Translate(typeof(NTAccount));
            var inheritanceFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            permissions.SetAccessRule(new FileSystemAccessRule(bultinAdmins, FileSystemRights.FullControl, inheritanceFlags, PropagationFlags.None, AccessControlType.Allow));
            permissions.SetAccessRule(new FileSystemAccessRule(localsystem, FileSystemRights.FullControl, inheritanceFlags, PropagationFlags.None, AccessControlType.Allow));
            permissions.SetAccessRule(new FileSystemAccessRule(builtinUsers, FileSystemRights.ReadAndExecute, inheritanceFlags, PropagationFlags.None, AccessControlType.Allow));
            permissions.SetOwner(bultinAdmins);
            permissions.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
            Directory.SetAccessControl(directoryPath, permissions);
        }
    }
}