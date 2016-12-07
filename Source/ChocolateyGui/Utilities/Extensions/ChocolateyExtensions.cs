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
using System.Linq.Expressions;
using System.Threading.Tasks;
using chocolatey;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.registration;
using chocolatey.infrastructure.results;
using ChocolateyGui.Models;
using ChocolateyGui.Services;
using Serilog;

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

        public static IChocolateyPackageInformationService GetPackageInformationService()
        {
            var containerType = SimpleInjectorContainer.Container.GetType();
            var containerParam = Expression.Constant(SimpleInjectorContainer.Container);
            var myMethod = containerType
                          .GetMethods()
                          .Where(m => m.Name == "GetInstance")
                          .Select(m => new
                          {
                              Method = m,
                              Params = m.GetParameters(),
                              Args = m.GetGenericArguments()
                          })
                          .Where(x => x.Params.Length == 0
                                      && x.Args.Length == 1)
                          .Select(x => x.Method)
                          .First();

            var block = Expression.Block(
                Expression.Call(containerParam, myMethod.MakeGenericMethod(typeof(IChocolateyPackageInformationService))));

            var func = Expression.Lambda<Func<IChocolateyPackageInformationService>>(block).Compile();
            return func();
        }

        public class ProgressWrapper : chocolatey.infrastructure.logging.ILog
        {
            private static readonly ILogger Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(Bootstrapper.AppDataPath, "Logs", "Chocolatey.{Date}.log"))
                .CreateLogger();

            private readonly IProgressService _progressService;
            private readonly Action<string> _errorListener;
            private ILogger _log = Logger;

            public ProgressWrapper(IProgressService progressService, Action<string> errorListener = null)
            {
                _progressService = progressService;
                _errorListener = errorListener;
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
            }

            public void Debug(string message, params object[] formatting)
            {
                _log.Debug(message, formatting);
                _progressService.WriteMessage(string.Format(CultureInfo.CurrentCulture, message, formatting), PowerShellLineType.Debug);
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
                _errorListener?.Invoke(messageString);
            }

            public void Error(string message, params object[] formatting)
            {
                _log.Error(message, formatting);
                var messageString = string.Format(CultureInfo.CurrentCulture, message, formatting);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
                _errorListener?.Invoke(messageString);
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
                _errorListener?.Invoke(messageString);
            }

            public void Fatal(string message, params object[] formatting)
            {
                _log.Fatal(message, formatting);
                var messageString = string.Format(CultureInfo.CurrentCulture, message, formatting);
                _progressService.WriteMessage(messageString, PowerShellLineType.Error);
                _errorListener?.Invoke(messageString);
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
            }

            public void Info(string message, params object[] formatting)
            {
                _log.Information(message, formatting);
                _progressService.WriteMessage(string.Format(CultureInfo.CurrentCulture, message, formatting));
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
            }

            public void Warn(string message, params object[] formatting)
            {
                _log.Warning(message, formatting);
                _progressService.WriteMessage(string.Format(CultureInfo.CurrentCulture, message, formatting), PowerShellLineType.Warning);
            }
        }
    }
}