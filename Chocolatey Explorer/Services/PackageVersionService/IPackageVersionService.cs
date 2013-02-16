namespace Chocolatey.Explorer.Services.PackageVersionService
{
    public interface IPackageVersionService
    {
        event PackageVersionService.VersionResult VersionChanged;
        void PackageVersion(string package);
    }
}