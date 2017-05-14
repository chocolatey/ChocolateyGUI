// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyExtensions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.ServiceModel;
using System.Threading.Tasks;
using chocolatey;
using chocolatey.infrastructure.results;
using ChocolateyGui.Models;

namespace ChocolateyGui.Subprocess
{
    public static class ChocolateyExtensions
    {
        public static Task RunAsync(this GetChocolatey chocolatey)
        {
            return Task.Run(() => chocolatey.Run());
        }

        public static Task<ICollection<T>> ListAsync<T>(this GetChocolatey chocolatey)
        {
            return Task.Run(() => (ICollection<T>)chocolatey.List<T>().ToList());
        }

        public static Task<ICollection<PackageResult>> ListPackagesAsync(this GetChocolatey chocolatey)
        {
            return Task.Run(() => (ICollection<PackageResult>)chocolatey.List<PackageResult>().ToList());
        }

        internal static GetChocolatey SetLoggerContext(this GetChocolatey chocolatey, OperationContext context)
        {
            return chocolatey.SetLoggerContext(context, out StreamingLogger _);
        }

        internal static GetChocolatey SetLoggerContext(this GetChocolatey chocolatey, OperationContext context, out StreamingLogger logger)
        {
            var callback = context.GetCallbackChannel<IIpcServiceCallbacks>();
            var logStream = new Subject<StreamingLogMessage>();
            logger = new StreamingLogger(logStream);
            logStream.Subscribe(message =>
            {
                callback.LogMessage(message);
            });
            return chocolatey.SetCustomLogging(logger);
        }
    }
}