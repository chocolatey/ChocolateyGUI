// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ILogService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System;

    public interface ILogService
    {
        void Debug(object message);
        
        void Debug(object message, Exception exception);
        
        void DebugFormat(string message, object obj);

        void DebugFormat(string message, object obj1, object obj2);
        
        void DebugFormat(string message, object obj1, object obj2, object obj3);
        
        void DebugFormat(string message, params object[] paramaters);
        
        void Info(object message);
        
        void Info(object message, Exception exception);
        
        void InfoFormat(string message, object obj);
        
        void InfoFormat(string message, object obj1, object obj2);
        
        void InfoFormat(string message, object obj1, object obj2, object obj3);
        
        void InfoFormat(string message, params object[] paramaters);
        
        void Warn(object message);
        
        void Warn(object message, Exception exception);
        
        void WarnFormat(string message, object obj);
        
        void WarnFormat(string message, object obj1, object obj2);
        
        void WarnFormat(string message, object obj1, object obj2, object obj3);
        
        void WarnFormat(string message, params object[] paramaters);
        
        void Error(object message);
        
        void Error(object message, Exception exception);
        
        void ErrorFormat(string message, object obj);
        
        void ErrorFormat(string message, object obj1, object obj2);
        
        void ErrorFormat(string message, object obj1, object obj2, object obj3);
        
        void ErrorFormat(string message, params object[] paramaters);
        
        void Fatal(object message);
        
        void Fatal(object message, Exception exception);
        
        void FatalFormat(string message, object obj);
        
        void FatalFormat(string message, object obj1, object obj2);

        void FatalFormat(string message, object obj1, object obj2, object obj3);
        
        void FatalFormat(string message, params object[] paramaters);

        void ForceFlush();
    }
}