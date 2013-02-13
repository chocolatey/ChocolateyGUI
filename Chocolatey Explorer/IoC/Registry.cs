using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.View;

namespace Chocolatey.Explorer.IoC
{
    public class Registry: StructureMap.Configuration.DSL.Registry
    {
         public Registry()
         {
             
             For<IPackageManager>().Use<PackageManager>();
             For<IPackageService>().Use<PackageService>();
             For<IPackagesService>().Use<CachedPackagesService>();
             For<IPackageVersionService>().Use<ODataPackageVersionService>();
             For<IChocolateyService>().Use<ChocolateyService>();
             For<IPackageVersionXMLParser>().Use<PackageVersionXMLParser>();
             For<IRunAsync>().Use<RunAsync>();
             For<IRunSync>().Use<RunSync>();
             For<ISourceService>().Singleton().Use<SourceService>();
			 For<IFileStorageService>().Use<LocalFileSystemStorageService>();
         }
    }
}