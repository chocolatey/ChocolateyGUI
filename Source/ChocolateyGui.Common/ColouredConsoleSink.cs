// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ColouredConsoleSink.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Serilog.Events;

namespace ChocolateyGui.Common
{
    public sealed class ColouredConsoleSink : Serilog.Core.ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            try
            {
                switch (logEvent.Level)
                {
                    case LogEventLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogEventLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    default:
                        break;
                }

                logEvent.RenderMessage(Console.Out);
                Console.WriteLine();
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}