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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "thing", Justification = "N/A")]
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