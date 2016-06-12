// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LogExtensions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Autofac;
using ChocolateyGui.Services;

namespace ChocolateyGui.Utilities.Extensions
{
    public static class LogExtensions
    {
        public static ILogService GetLogger(this Type sourceType)
        {
            return Bootstrapper.Container.Resolve<ILogService>(new TypedParameter(typeof(Type), sourceType));
        }
    }
}