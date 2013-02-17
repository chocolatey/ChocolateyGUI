namespace Chocolatey.Explorer.Services.PackagesService
{
    public interface IAvailablePackagesService
    {
        event AvailablePackagesService.FinishedDelegate RunFinshed;
		event AvailablePackagesService.FailedDelegate RunFailed;
        void ListOfAvalablePackages();
    }
}