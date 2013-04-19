namespace Chocolatey.Explorer.Services.PackageService
{
    public interface IPackageService
    {
        event Delegates.LineDelegate LineChanged;
        event Delegates.FinishedPackageDelegate RunFinshed;
        event Delegates.StartedDelegate RunStarted;
        void InstallPackage(string package);
        void UninstallPackage(string package);
        void UpdatePackage(string package);
    }
}