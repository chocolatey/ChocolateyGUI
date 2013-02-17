namespace Chocolatey.Explorer.Services.PackagesService
{
    public interface IInstalledPackagesService
    {
        event AvailablePackagesService.FinishedDelegate RunFinshed;
        event AvailablePackagesService.FailedDelegate RunFailed;
        void ListOfIntalledPackages(); 
    }
}