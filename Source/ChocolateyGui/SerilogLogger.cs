// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SerilogLogger.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui
{
    using System;
    using chocolatey.infrastructure.logging;
    using Serilog;

    public class SerilogLogger : ILog
    {
        private readonly ILogger _logger;

        public SerilogLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void InitializeFor(string loggerName)
        {
            //skip for now
        }

        public void Debug(string message, params object[] formatting)
        {
            _logger.Debug(message, formatting);
        }

        public void Debug(Func<string> message)
        {
            _logger.Debug(message());
        }

        public void Info(string message, params object[] formatting)
        {
            _logger.Information(message, formatting);
        }

        public void Info(Func<string> message)
        {
            _logger.Information(message());
        }

        public void Warn(string message, params object[] formatting)
        {
            _logger.Warning(message, formatting);
        }

        public void Warn(Func<string> message)
        {
            _logger.Warning(message());
        }

        public void Error(string message, params object[] formatting)
        {
            _logger.Error(message, formatting);
        }

        public void Error(Func<string> message)
        {
            _logger.Error(message());
        }

        public void Fatal(string message, params object[] formatting)
        {
            _logger.Fatal(message, formatting);
        }

        public void Fatal(Func<string> message)
        {
            _logger.Fatal(message());
        }
    }
}
