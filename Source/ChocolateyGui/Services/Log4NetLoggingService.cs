using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Mindscape.Raygun4Net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ChocolateyGui.Services
{
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

        public void DebugFormat(string message, params object[] paramaters)
        {
            _log.DebugFormat(message, paramaters);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
#if !DEBUG
            _client.Send(exception);
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

        public void InfoFormat(string message, params object[] paramaters)
        {
            _log.InfoFormat(message, paramaters);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
#if !DEBUG
            _client.Send(exception);
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

        public void WarnFormat(string message, params object[] paramaters)
        {
            _log.WarnFormat(message, paramaters);
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
            _client.Send(exception);
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

        public void ErrorFormat(string message, params object[] paramaters)
        {
            _log.ErrorFormat(message, paramaters);
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
            _client.Send(exception);
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

        public void FatalFormat(string message, params object[] paramaters)
        {
            _log.FatalFormat(message, paramaters);
            FlushBuffer();
        }

        private void FlushBuffer()
        {
            var logger = (_log.Logger as Logger);
            if (logger == null)
                return;
            var buffered =
                logger.Appenders.Cast<IAppender>().AsQueryable().SingleOrDefault(app => app is BufferingAppenderSkeleton);
            if (buffered != null)
                (buffered as BufferingAppenderSkeleton).Flush();
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
            }
        }
    }
}
