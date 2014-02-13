using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Chocolatey.Gui.Services;

namespace Chocolatey.Gui.Utilities.Extensions
{
    public static class LogExtensions
    {
        public static ILogService GetLogger(this Type sourceType)
        {
            return App.Container.Resolve<ILogService>(new TypedParameter(typeof (Type), sourceType));
        }
    }
}
