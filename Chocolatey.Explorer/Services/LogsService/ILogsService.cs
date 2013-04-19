namespace Chocolatey.Explorer.Services.LogsService
{
    public interface ILogsService
    {
        event Delegates.StartedDelegate RunStarted;
        event Delegates.FinishedLogsDelegate RunFinished;
        void GetLogs(); 
    }
}