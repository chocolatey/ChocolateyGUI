// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChocolateyExtensions.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using chocolatey;
using chocolatey.infrastructure.results;
using ChocolateyGui.Models;
using ChocolateyGui.Services;
using Serilog;
using Serilog.Events;

namespace ChocolateyGui.Utilities.Extensions
{
    public static class ChocolateyExtensions
    {
        public static Task RunAsync(this GetChocolatey chocolatey)
        {
            return Task.Run(() => chocolatey.Run());
        }

        public static Task<ICollection<T>> ListAsync<T>(this GetChocolatey chocolatey)
        {
            return Task.Run(() => (ICollection<T>) chocolatey.List<T>().ToList());
        }

        public static Task<ICollection<PackageResult>> ListPackagesAsync(this GetChocolatey chocolatey)
        {
            return Task.Run(() => (ICollection<PackageResult>) chocolatey.List<PackageResult>().ToList());
        }

        public static GetChocolatey Init(this GetChocolatey chocolatey, IProgressService progressService, Action<string> errorListener = null)
        {
            if (chocolatey == null)
            {
                throw new ArgumentNullException(nameof(chocolatey));
            }

            chocolatey.SetCustomLogging(new ProgressWrapper(progressService, errorListener));
            return chocolatey;
        }

        public static GetChocolatey Init(this GetChocolatey chocolatey, IProgressService progressService, Action<LogEventLevel, string, object[]> messageInterceptor)
        {
            if (chocolatey == null)
            {
                throw new ArgumentNullException(nameof(chocolatey));
            }

            chocolatey.SetCustomLogging(new ProgressWrapper(progressService, messageInterceptor));
            return chocolatey;
        }

        public class ProgressWrapper : chocolatey.infrastructure.logging.ILog
        {
            private static readonly ILogger Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(Bootstrapper.AppDataPath, "Logs", "Chocolatey.{Date}.log"), retainedFileCountLimit: 10, fileSizeLimitBytes: 150 * 1000 * 1000)
                .CreateLogger();

            private readonly object[] _empty = new object[0];

            private readonly IProgressService _progressService;
            private readonly Action<LogEventLevel, string, object[]> _messageInterceptor;
            private ILogger _log = Logger;

            public ProgressWrapper(IProgressService progressService, Action<string> errorListener = null)
            {
                _progressService = progressService;
                if (errorListener != null)
                {
                    _messageInterceptor = (level, message, formatting) =>
                    {
                        switch (level)
                        {
                            case LogEventLevel.Error:
                            case LogEventLevel.Fatal:
                                errorListener.Invoke(message);
                                break;
                            default:
                                break;
                        }
                    };
                }
            }

            public ProgressWrapper(IProgressService progressService, Action<LogEventLevel, string, object[]> messageInterceptor)
            {
                _progressService = progressService;
                _messageInterceptor = messageInterceptor;
            }

            public void Debug(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _log.Debug(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Debug);
                _messageInterceptor?.Invoke(LogEventLevel.Debug, messageString, _empty);
            }

            public void Debug(string message, params object[] formatting)
            {
                _log.Debug(message, formatting);
                _progressService.WriteMessage(string.Format(CultureInfo.CurrentCulture, message, formatting), PowerShellLineType.Debug);
                _messageInterceptor?.Invoke(LogEventLevel.Debug, message, formatting);
            }

            public void Error(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _log.Error(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
                _messageInterceptor?.Invoke(LogEventLevel.Error, messageString, _empty);
            }

            public void Error(string message, params object[] formatting)
            {
                _log.Error(message, formatting);
                var messageString = string.Format(CultureInfo.CurrentCulture, message, formatting);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
                _messageInterceptor?.Invoke(LogEventLevel.Error, message, formatting);
            }

            public void Fatal(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _log.Fatal(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
                _messageInterceptor?.Invoke(LogEventLevel.Fatal, messageString, _empty);
            }

            public void Fatal(string message, params object[] formatting)
            {
                _log.Fatal(message, formatting);
                var messageString = string.Format(CultureInfo.CurrentCulture, message, formatting);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
                _messageInterceptor?.Invoke(LogEventLevel.Fatal, message, formatting);
            }

            public void Info(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _log.Information(messageString);
                _progressService.WriteMessage(messageString);
                _messageInterceptor?.Invoke(LogEventLevel.Information, messageString, _empty);
            }

            public void Info(string message, params object[] formatting)
            {
                _log.Information(message, formatting);
                _progressService.WriteMessage(string.Format(CultureInfo.CurrentCulture, message, formatting));
                _messageInterceptor?.Invoke(LogEventLevel.Information, message, formatting);
            }

            public void InitializeFor(string loggerName)
            {
                _log = _log.ForContext("ChocolateyLogger", loggerName);
            }

            public void Warn(Func<string> message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var messageString = message();
                _log.Warning(messageString);
                _progressService.WriteMessage(messageString, PowerShellLineType.Warning);
                _messageInterceptor?.Invoke(LogEventLevel.Warning, messageString, _empty);
            }

            public void Warn(string message, params object[] formatting)
            {
                _log.Warning(message, formatting);
                _progressService.WriteMessage(string.Format(CultureInfo.CurrentCulture, message, formatting), PowerShellLineType.Warning);
                _messageInterceptor?.Invoke(LogEventLevel.Warning, message, formatting);
            }
        }
    }
}