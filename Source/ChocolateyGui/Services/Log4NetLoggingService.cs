// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Log4NetLoggingService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ChocolateyGui.Services
{
    using System;
    using System.Globalization;
    using System.Linq;
    using log4net;
    using log4net.Appender;
    using log4net.Repository.Hierarchy;
    using Mindscape.Raygun4Net;

    public class Log4NetLoggingService : ILogService
    {
        private readonly ILog _log;
#if !DEBUG
        private readonly RaygunClient _client = new RaygunClient("qDY2sdfKNrPcwaZJf3FELA==");
#endif

        public Log4NetLoggingService(Type logSourceType)
        {
            this._log = LogManager.GetLogger(logSourceType);
        }

        public void Debug(object message)
        {
            this._log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            this._log.Debug(message, exception);
        }

        public void DebugFormat(string message, object obj)
        {
            this._log.DebugFormat(message, obj);
        }

        public void DebugFormat(string message, object obj1, object obj2)
        {
            this._log.DebugFormat(message, obj1, obj2);
        }

        public void DebugFormat(string message, object obj1, object obj2, object obj3)
        {
            this._log.DebugFormat(message, obj1, obj2, obj3);
        }

        public void DebugFormat(string message, params object[] paramaters)
        {
            this._log.DebugFormat(CultureInfo.CurrentCulture, message, paramaters);
        }

        public void Error(object message)
        {
            this._log.Error(message);
            this.FlushBuffer();
        }

        public void Error(object message, Exception exception)
        {
            this._log.Error(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
            this.FlushBuffer();
        }

        public void ErrorFormat(string message, object obj)
        {
            this._log.ErrorFormat(message, obj);
            this.FlushBuffer();
        }

        public void ErrorFormat(string message, object obj1, object obj2)
        {
            this._log.ErrorFormat(message, obj1, obj2);
            this.FlushBuffer();
        }

        public void ErrorFormat(string message, object obj1, object obj2, object obj3)
        {
            this._log.ErrorFormat(message, obj1, obj2, obj3);
            this.FlushBuffer();
        }

        public void ErrorFormat(string message, params object[] paramaters)
        {
            this._log.ErrorFormat(CultureInfo.CurrentCulture, message, paramaters);
            this.FlushBuffer();
        }

        public void Fatal(object message)
        {
            this._log.Fatal(message);
            this.FlushBuffer();
        }

        public void Fatal(object message, Exception exception)
        {
            this._log.Fatal(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
            this.FlushBuffer();
        }

        public void FatalFormat(string message, object obj)
        {
            this._log.FatalFormat(message, obj);
            this.FlushBuffer();
        }

        public void FatalFormat(string message, object obj1, object obj2)
        {
            this._log.FatalFormat(message, obj1, obj2);
            this.FlushBuffer();
        }

        public void FatalFormat(string message, object obj1, object obj2, object obj3)
        {
            this._log.FatalFormat(message, obj1, obj2, obj3);
            this.FlushBuffer();
        }

        public void FatalFormat(string message, params object[] paramaters)
        {
            this._log.FatalFormat(CultureInfo.CurrentCulture, message, paramaters);
            this.FlushBuffer();
        }

        public void ForceFlush()
        {
            try
            {
                this.FlushBuffer();
            }
            catch (Exception ex)
            {
                this.Error("Error flushing buffer...", ex);
            }
        }

        public void Info(object message)
        {
            this._log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            this._log.Info(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
        }

        public void InfoFormat(string message, object obj)
        {
            this._log.InfoFormat(message, obj);
        }

        public void InfoFormat(string message, object obj1, object obj2)
        {
            this._log.InfoFormat(message, obj1, obj2);
        }

        public void InfoFormat(string message, object obj1, object obj2, object obj3)
        {
            this._log.InfoFormat(message, obj1, obj2, obj3);
        }

        public void InfoFormat(string message, params object[] paramaters)
        {
            this._log.InfoFormat(CultureInfo.CurrentCulture, message, paramaters);
        }

        public void Warn(object message)
        {
            this._log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            this._log.Warn(message, exception);
#if !DEBUG
            this._client.Send(exception);
#endif
        }

        public void WarnFormat(string message, object obj)
        {
            this._log.WarnFormat(message, obj);
        }

        public void WarnFormat(string message, object obj1, object obj2)
        {
            this._log.WarnFormat(message, obj1, obj2);
        }

        public void WarnFormat(string message, object obj1, object obj2, object obj3)
        {
            this._log.WarnFormat(message, obj1, obj2, obj3);
        }

        public void WarnFormat(string message, params object[] paramaters)
        {
            this._log.WarnFormat(CultureInfo.CurrentCulture, message, paramaters);
        }

        private void FlushBuffer()
        {
            var logger = this._log.Logger as Logger;
            if (logger == null)
            {
                return;
            }

            var buffered =
                logger.Appenders.Cast<IAppender>().AsQueryable().SingleOrDefault(app => app is BufferingAppenderSkeleton);
            if (buffered != null)
            {
                (buffered as BufferingAppenderSkeleton).Flush();
            }
        }
    }
}