using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.View;

namespace Chocolatey.Explorer.IoC
{
    public class Registry: StructureMap.Configuration.DSL.Registry
    {
         public Registry()
         {
             For<IPackageManager>().Use<PackageManager>();
             For<IPackageService>().Use<PackageService>();
             For<IPackagesService>().Use<PackagesService>();
             For<IPackageVersionService>().Use<PackageVersionService>();
             For<IChocolateyService>().Use<ChocolateyService>();
             For<IRun>().Use<RunAsync>();
             For<IRun>().Use<RunSync>().Named("sync");
             For<ISourceService>().Singleton().Use<SourceService>();
         }
    }
}