namespace Chocolatey.Explorer.Services
{
    public interface IPackageService
    {
        event PackageService.LineDelegate LineChanged;
        event PackageService.FinishedDelegate RunFinshed;
        void InstallPackage(string package);
        void UpdatePackage(string package);
    }
}