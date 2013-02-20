namespace Chocolatey.Explorer.Services.PackagesService
{
    public interface IAvailablePackagesService
    {
        event Delegates.StartedDelegate RunStarted;
        event Delegates.FinishedDelegate RunFinshed;
		event Delegates.FailedDelegate RunFailed;
        void ListOfAvalablePackages();
    }
}