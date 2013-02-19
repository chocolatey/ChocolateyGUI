namespace Chocolatey.Explorer.Services.PackageVersionService
{
    public interface IPackageVersionService
    {
        event Delegates.VersionResult VersionChanged;
        event Delegates.StartedDelegate Started;
        void PackageVersion(string package);
    }
}