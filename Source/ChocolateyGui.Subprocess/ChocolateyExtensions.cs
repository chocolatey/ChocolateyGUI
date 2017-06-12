// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyExtensions.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using chocolatey;
using chocolatey.infrastructure.results;
using ChocolateyGui.Models;

namespace ChocolateyGui.Subprocess
{
    public static class ChocolateyExtensions
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cancellationTokenSources =
            new ConcurrentDictionary<string, CancellationTokenSource>(StringComparer.OrdinalIgnoreCase);

        public static Task RunAsync(this GetChocolatey chocolatey, CancellationToken cancellationToken)
        {
            return Task.Run(() => chocolatey.Run(), cancellationToken);
        }

        public static Task<ICollection<T>> ListAsync<T>(this GetChocolatey chocolatey, CancellationToken cancellationToken)
        {
            return Task.Run(() => (ICollection<T>)chocolatey.List<T>().ToList(), cancellationToken);
        }

        public static Task<ICollection<PackageResult>> ListPackagesAsync(this GetChocolatey chocolatey)
        {
            return Task.Run(() => (ICollection<PackageResult>)chocolatey.List<PackageResult>().ToList());
        }

        internal static GetChocolatey SetLoggerContext(this GetChocolatey chocolatey, OperationContext context)
        {
            StreamingLogger ignored;
            return chocolatey.SetLoggerContext(context, out ignored);
        }

        internal static GetChocolatey SetLoggerContext(this GetChocolatey chocolatey, OperationContext context, out StreamingLogger logger)
        {
            var callback = context.GetCallbackChannel<IIpcServiceCallbacks>();
            var logStream = new Subject<StreamingLogMessage>();
            logger = new StreamingLogger(logStream, context.GetCancellationToken());
            logStream.Subscribe(message =>
            {
                callback.LogMessage(message);
            });
            return chocolatey.SetCustomLogging(logger);
        }

        internal static CancellationToken GetCancellationToken(this OperationContext context)
        {
            var sessionId = context.SessionId;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new InvalidOperationException("Missing SessionId");
            }

            var cts = _cancellationTokenSources.GetOrAdd(sessionId, id => new CancellationTokenSource());
            return cts.Token;
        }

        internal static void EndSession(this OperationContext context)
        {
            var sessionId = context.SessionId;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new InvalidOperationException("Missing SessionId");
            }

            CancellationTokenSource cts;
            if (_cancellationTokenSources.TryGetValue(sessionId, out cts))
            {
                cts.Cancel();
            }
        }
    }
}