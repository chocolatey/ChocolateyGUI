using Serilog;
using Serilog.Events;
#if !DEBUG
using System;
#endif

namespace ChocolateyGui.Utilities
{
    public static class DefaultsExtensions
    {
        public static LoggerConfiguration SetDefaultLevel(this LoggerConfiguration loggerConfig, LogEventLevel defaultLevel = LogEventLevel.Information)
        {
#if DEBUG
            return loggerConfig.MinimumLevel.Debug();
#else

            var logLevel = Environment.GetEnvironmentVariable("CHOCOLATEYGUI__LOGLEVEL");
            LogEventLevel logEventLevel;
            if (string.IsNullOrWhiteSpace(logLevel) || !Enum.TryParse(logLevel, true, out logEventLevel))
            {
                loggerConfig.MinimumLevel.Is(defaultLevel);
            }
            else
            {
                loggerConfig.MinimumLevel.Is(logEventLevel);
            }
            return loggerConfig;
#endif
        }
    }
}
