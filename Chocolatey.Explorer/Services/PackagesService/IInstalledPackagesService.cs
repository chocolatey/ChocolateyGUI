namespace Chocolatey.Explorer.Services.PackagesService
{
    public interface IInstalledPackagesService
    {
        event Delegates.StartedDelegate RunStarted;
        event Delegates.FinishedDelegate RunFinshed;
        event Delegates.FailedDelegate RunFailed;
        void ListOfIntalledPackages();
        void ListOfDistinctHighestInstalledPackages();
    }
}