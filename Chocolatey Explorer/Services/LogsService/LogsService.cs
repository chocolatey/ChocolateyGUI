using System;
using System.Collections.Generic;
using System.IO;
using Chocolatey.Explorer.Services.FileStorageService;

namespace Chocolatey.Explorer.Services.LogsService
{
    public class LogsService:ILogsService
    {
        public event Delegates.StartedDelegate RunStarted;
        public event Delegates.FinishedLogsDelegate RunFinished;

        private readonly IFileStorageService _fileStorageService;

        public LogsService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public void GetLogs()
        {
            OnRunStarted();
            OnRunFinshed(_fileStorageService.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChocolateyGUI", "Logs")));
        }

        private void OnRunFinshed(IList<string> logs)
        {
            this.Log().Debug("Run finished found {0} logs", logs !=null?logs.Count:0);
            var handler = RunFinished;
            if (handler != null) handler(logs);
        }

        private void OnRunStarted()
        {
            this.Log().Debug("Run started");
            var handler = RunStarted;
            if (handler != null) handler("Getting list of available logs.");
        }
    }
}