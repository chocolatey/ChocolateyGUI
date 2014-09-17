using System;
using Autofac;
using ChocolateyGui.Services;

namespace ChocolateyGui.Utilities.Extensions
{
    public static class LogExtensions
    {
        public static ILogService GetLogger(this Type sourceType)
        {
            return App.Container.Resolve<ILogService>(new TypedParameter(typeof (Type), sourceType));
        }
    }
}
