using System;
using System.Reactive.Subjects;
using chocolatey.infrastructure.logging;
using ChocolateyGui.Models;
using Serilog;

namespace ChocolateyGui.Subprocess
{
    internal class StreamingLogger : ILog
    {
        private static readonly ILogger Logger = Serilog.Log.ForContext<StreamingLogger>();
        protected ISubject<StreamingLogMessage> Subject;
        private string _context;
        private Action<StreamingLogMessage> _interceptor;

        public StreamingLogger(ISubject<StreamingLogMessage> subject)
        {
            Subject = subject;
        }

        public void InitializeFor(string loggerName)
        {
            _context = loggerName;
        }

        public IDisposable Intercept(Action<StreamingLogMessage> interceptor)
        {
            return new InterceptMessages(this, interceptor);
        }

        public void Debug(string message, params object[] formatting)
        {
            Logger.Debug(message, formatting);
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Debug,
                Message = string.Format(message, formatting)
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Debug(Func<string> message)
        {
            Logger.Debug(message());
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Debug,
                Message = message()
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Info(string message, params object[] formatting)
        {
            Logger.Information(message, formatting);
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Info,
                Message = string.Format(message, formatting)
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Info(Func<string> message)
        {
            Logger.Information(message());
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Info,
                Message = message()
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Warn(string message, params object[] formatting)
        {
            Logger.Warning(message, formatting);
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Warn,
                Message = string.Format(message, formatting)
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Warn(Func<string> message)
        {
            Logger.Warning(message());
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Warn,
                Message = message()
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Error(string message, params object[] formatting)
        {
            Logger.Error(message, formatting);
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Error,
                Message = string.Format(message, formatting)
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Error(Func<string> message)
        {
            Logger.Error(message());
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Error,
                Message = message()
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Fatal(string message, params object[] formatting)
        {
            Logger.Fatal(message, formatting);
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Fatal,
                Message = string.Format(message, formatting)
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public void Fatal(Func<string> message)
        {
            Logger.Fatal(message());
            var logMessage = new StreamingLogMessage
            {
                Context = _context,
                LogLevel = StreamingLogLevel.Fatal,
                Message = message()
            };
            _interceptor?.Invoke(logMessage);
            Subject.OnNext(logMessage);
        }

        public class InterceptMessages : IDisposable
        {
            private readonly StreamingLogger _logger;

            public InterceptMessages(StreamingLogger logger, Action<StreamingLogMessage> interceptor)
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
