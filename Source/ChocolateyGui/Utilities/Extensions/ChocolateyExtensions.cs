// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChocolateyExtensions.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using chocolatey;
    using chocolatey.infrastructure.results;
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;

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

        public static GetChocolatey Init(this GetChocolatey chocolatey, IProgressService progressService, Func<string, ILogService> logService)
        {
            if (chocolatey == null)
            {
                throw new ArgumentNullException(nameof(chocolatey));
            }

            chocolatey.SetCustomLogging(new ProgressWrapper(progressService, logService));
            return chocolatey;
        }

        private class ProgressWrapper : chocolatey.infrastructure.logging.ILog
        {
            private readonly IProgressService _progressService;
            private readonly Func<string, ILogService> _logSerivceFactory;
            private ILogService _logService;

            public ProgressWrapper(IProgressService progressService, Func<string, ILogService> logSerivceFactory)
            {
                _progressService = progressService;
                _logSerivceFactory = logSerivceFactory;
                _logService = _logSerivceFactory(typeof(ProgressWrapper).Name);
            }

            public void Debug(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _logService.Debug(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Debug);
            }

            public void Debug(string message, params object[] formatting)
            {
                _logService.DebugFormat(message, formatting);
            }

            public void Error(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _logService.Error(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
            }

            public void Error(string message, params object[] formatting)
            {
                _logService.ErrorFormat(message, formatting);
            }

            public void Fatal(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _logService.Fatal(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
            }

            public void Fatal(string message, params object[] formatting)
            {
                _logService.FatalFormat(message, formatting);
            }

            public void Info(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _logService.Info(messageString);
                _progressService.WriteMessage(messageString);
            }

            public void Info(string message, params object[] formatting)
            {
                _logService.InfoFormat(message, formatting);
                _progressService.WriteMessage(string.Format(CultureInfo.CurrentCulture, message, formatting));
            }

            public void InitializeFor(string loggerName)
            {
                _logService = _logSerivceFactory(loggerName);
            }

            public void Warn(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _logService.Warn(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Warning);
            }

            public void Warn(string message, params object[] formatting)
            {
                _logService.WarnFormat(message, formatting);
            }
        }
    }
}
