namespace Chocolatey.Explorer.Services.PackagesService
{
    public interface IAvailablePackagesService
    {
        event Delegates.FinishedDelegate RunFinshed;
		event Delegates.FailedDelegate RunFailed;
        void ListOfAvalablePackages();
    }
}