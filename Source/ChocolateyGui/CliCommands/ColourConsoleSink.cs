using System;
using Serilog.Events;

namespace ChocolateyGui.CliCommands
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