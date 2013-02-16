namespace Chocolatey.Explorer.Services.PackageService
{
    public interface IPackageService
    {
        event PackageService.LineDelegate LineChanged;
        event PackageService.FinishedDelegate RunFinshed;
        void InstallPackage(string package);
        void UninstallPackage(string package);
        void UpdatePackage(string package);
    }
}