namespace Chocolatey.Explorer.Services
{
    public interface IPackageVersionService
    {
        event PackageVersionService.VersionResult VersionChanged;
        void PackageVersion(string package);
    }
}