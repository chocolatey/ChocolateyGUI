// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LogExtensions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Extensions
{
    using Autofac;
    using ChocolateyGui.Services;
    using System;

    public static class LogExtensions
    {
        public static ILogService GetLogger(this Type sourceType)
        {
            return App.Container.Resolve<ILogService>(new TypedParameter(typeof(Type), sourceType));
        }
    }
}