namespace Chocolatey.Explorer.Services.PackagesService
{
    public interface IPackagesService
    {
        event PackagesService.FinishedDelegate RunFinshed;
		event PackagesService.FailedDelegate RunFailed;
        void ListOfPackages();
        void ListOfInstalledPackages();
    }
}