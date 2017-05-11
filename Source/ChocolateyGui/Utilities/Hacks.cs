// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Hacks.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using chocolatey;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.registration;

namespace ChocolateyGui.Utilities
{
    public static class Hacks
    {
        public static bool IsElevated => (WindowsIdentity.GetCurrent().Owner?.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid)).GetValueOrDefault(false);
    }
}