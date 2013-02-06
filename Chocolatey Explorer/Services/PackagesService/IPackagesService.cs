namespace Chocolatey.Explorer.Services
{
    public interface IPackagesService
    {
        event PackagesService.FinishedDelegate RunFinshed;
        void ListOfPackages();
        void ListOfInstalledPackages();
    }
}