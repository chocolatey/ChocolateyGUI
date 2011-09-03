using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.View;

namespace Chocolatey.Explorer.IoC
{
    public class Registry: StructureMap.Configuration.DSL.Registry
    {
         public Registry()
         {
             this.For<IPackageManager>().Use<PackageManager>();
             this.For<IPackageService>().Use<PackageService>();
             this.For<IPackagesService>().Use<PackagesService>();
             this.For<IPackageVersionService>().Use<PackageVersionService>();
             this.For<IChocolateyService>().Use<ChocolateyService>();
             this.For<IRun>().Use<RunAsync>();
             this.For<IRun>().Use<RunSync>().Named("sync");
         }
    }
}