// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SerilogLogger.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui
{
    using System;
    using chocolatey.infrastructure.logging;
    using Models;
    using Serilog;

    public class SerilogLogger : ILog
    {
        private readonly ILogger _logger;
        private Action<LogMessage> _interceptor;

        public SerilogLogger(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable Intercept(Action<LogMessage> interceptor)
        {
            return new InterceptMessages(this, interceptor);
        }

        public void InitializeFor(string loggerName)
        {
            // skip for now
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

        public class InterceptMessages : IDisposable
        {
            private readonly SerilogLogger _logger;

            public InterceptMessages(SerilogLogger logger, Action<LogMessage> interceptor)
            {
                _logger = logger;
                logger._interceptor = interceptor;
            }

            public void Dispose()
            {
                _logger._interceptor = null;
            }
        }
    }
}