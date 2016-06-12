// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Log4NetLoggingService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;

[assembly: XmlConfigurator(Watch = true)]

namespace ChocolateyGui.Services
{
#if !DEBUG
    using Mindscape.Raygun4Net;
#endif

    public class Log4NetLoggingService : ILogService
    {
        private readonly ILog _log;
#if !DEBUG
        private readonly RaygunClient _client = new RaygunClient("qDY2sdfKNrPcwaZJf3FELA==");
#endif

        public Log4NetLoggingService(Type logSourceType)
        {
            _log = LogManager.GetLogger(logSourceType);
        }

        public Log4NetLoggingService(string logSourceName)
        {
            _log = LogManager.GetLogger(logSourceName);
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void DebugFormat(string message, object obj)
        {
            _log.DebugFormat(message, obj);
        }

        public void DebugFormat(string message, object obj1, object obj2)
        {
            _log.DebugFormat(message, obj1, obj2);
        }

        public void DebugFormat(string message, object obj1, object obj2, object obj3)
        {
            _log.DebugFormat(message, obj1, obj2, obj3);
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            _log.DebugFormat(CultureInfo.CurrentCulture, message, parameters);
        }

        public void Error(object message)
        {
            _log.Error(message);
            FlushBuffer();
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
            FlushBuffer();
        }

        public void ErrorFormat(string message, object obj)
        {
            _log.ErrorFormat(message, obj);
            FlushBuffer();
        }

        public void ErrorFormat(string message, object obj1, object obj2)
        {
            _log.ErrorFormat(message, obj1, obj2);
            FlushBuffer();
        }

        public void ErrorFormat(string message, object obj1, object obj2, object obj3)
        {
            _log.ErrorFormat(message, obj1, obj2, obj3);
            FlushBuffer();
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
            _log.ErrorFormat(CultureInfo.CurrentCulture, message, parameters);
            FlushBuffer();
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
            FlushBuffer();
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
            FlushBuffer();
        }

        public void FatalFormat(string message, object obj)
        {
            _log.FatalFormat(message, obj);
            FlushBuffer();
        }

        public void FatalFormat(string message, object obj1, object obj2)
        {
            _log.FatalFormat(message, obj1, obj2);
            FlushBuffer();
        }

        public void FatalFormat(string message, object obj1, object obj2, object obj3)
        {
            _log.FatalFormat(message, obj1, obj2, obj3);
            FlushBuffer();
        }

        public void FatalFormat(string message, params object[] parameters)
        {
            _log.FatalFormat(CultureInfo.CurrentCulture, message, parameters);
            FlushBuffer();
        }

        public void ForceFlush()
        {
            try
            {
                FlushBuffer();
            }
            catch (Exception ex)
            {
                Error("Error flushing buffer...", ex);
                throw;
            }
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
        }

        public void InfoFormat(string message, object obj)
        {
            _log.InfoFormat(message, obj);
        }

        public void InfoFormat(string message, object obj1, object obj2)
        {
            _log.InfoFormat(message, obj1, obj2);
        }

        public void InfoFormat(string message, object obj1, object obj2, object obj3)
        {
            _log.InfoFormat(message, obj1, obj2, obj3);
        }

        public void InfoFormat(string message, params object[] parameters)
        {
            _log.InfoFormat(CultureInfo.CurrentCulture, message, parameters);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
        }

        public void WarnFormat(string message, object obj)
        {
            _log.WarnFormat(message, obj);
        }

        public void WarnFormat(string message, object obj1, object obj2)
        {
            _log.WarnFormat(message, obj1, obj2);
        }

        public void WarnFormat(string message, object obj1, object obj2, object obj3)
        {
            _log.WarnFormat(message, obj1, obj2, obj3);
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            _log.WarnFormat(CultureInfo.CurrentCulture, message, parameters);
        }

        private void FlushBuffer()
        {
            var logger = _log.Logger as Logger;
            if (logger == null)
            {
                return;
            }

            var buffered =
                logger.Appenders.Cast<IAppender>()
                    .AsQueryable()
                    .SingleOrDefault(app => app is BufferingAppenderSkeleton);
            if (buffered != null)
            {
                var bufferingAppenderSkeleton = buffered as BufferingAppenderSkeleton;

                if (bufferingAppenderSkeleton != null)
                {
                    bufferingAppenderSkeleton.Flush();
                }
            }
        }
    }
}