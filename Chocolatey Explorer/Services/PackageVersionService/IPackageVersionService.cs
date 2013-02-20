namespace Chocolatey.Explorer.Services.PackageVersionService
{
    public interface IPackageVersionService
    {
        event Delegates.VersionResult VersionChanged;
        event Delegates.StartedDelegate RunStarted;
        void PackageVersion(string package);
    }
}