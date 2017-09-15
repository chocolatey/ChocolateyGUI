// <copyright file="ServiceCallbackHandler.cs" company="Chocolatey">
//      Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using ChocolateyGui.Models;

namespace ChocolateyGui.Services
{
    public class ServiceCallbackHandler : IChocolateyServiceCallbacks
    {
        private readonly IProgressService _progressService;

        public ServiceCallbackHandler(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public void LogMessage(LogMessage message)
        {
            PowerShellLineType powerShellLineType;
            switch (message.LogLevel)
            {
                case LogLevel.Debug:
                    powerShellLineType = PowerShellLineType.Debug;
                    break;
                case LogLevel.Verbose:
                    powerShellLineType = PowerShellLineType.Verbose;
                    break;
                case LogLevel.Info:
                    powerShellLineType = PowerShellLineType.Output;
                    break;
                case LogLevel.Warn:
                    powerShellLineType = PowerShellLineType.Warning;
                    break;
                case LogLevel.Error:
                    powerShellLineType = PowerShellLineType.Error;
                    break;
                case LogLevel.Fatal:
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