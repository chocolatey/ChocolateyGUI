// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LogExtensions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Serilog;

namespace ChocolateyGui.Utilities.Extensions
{
    public static class LogExtensions
    {
        public static ILogger GetLogger<T>(this T thing)
        {
            return Log.Logger.ForContext<T>();
        }

        public static ILogger GetLogger(this Type type)
        {
            return Log.Logger.ForContext(type);
        }
    }
}
