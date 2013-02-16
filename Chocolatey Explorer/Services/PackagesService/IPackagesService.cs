namespace Chocolatey.Explorer.Services
{
    public interface IPackagesService
    {
        event PackagesService.FinishedDelegate RunFinshed;
		event PackagesService.FailedDelegate RunFailed;
        void ListOfPackages();
        void ListOfInstalledPackages();
    }
}