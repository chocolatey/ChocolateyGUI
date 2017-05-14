using ChocolateyGui.Models;

namespace ChocolateyGui.Services
{
    public class ServiceCallbackHandler : IIpcServiceCallbacks
    {
        private readonly IProgressService _progressService;

        public ServiceCallbackHandler(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public void LogMessage(StreamingLogMessage message)
        {
            PowerShellLineType powerShellLineType;
            switch (message.LogLevel)
            {
                case StreamingLogLevel.Debug:
                    powerShellLineType = PowerShellLineType.Debug;
                    break;
                case StreamingLogLevel.Verbose:
                    powerShellLineType = PowerShellLineType.Verbose;
                    break;
                case StreamingLogLevel.Info:
                    powerShellLineType = PowerShellLineType.Output;
                    break;
                case StreamingLogLevel.Warn:
                    powerShellLineType = PowerShellLineType.Warning;
                    break;
                case StreamingLogLevel.Error:
                    powerShellLineType = PowerShellLineType.Error;
                    break;
                case StreamingLogLevel.Fatal:
                    powerShellLineType = PowerShellLineType.Error;
                    break;
                default:
                    powerShellLineType = PowerShellLineType.Output;
                    break;
            }

            _progressService.WriteMessage(message.Message, powerShellLineType);
        }
    }
}