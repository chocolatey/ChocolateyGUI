using Chocolatey.Explorer.CommandPattern;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.Services.ChocolateyService;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.Services.PackageService;
using Chocolatey.Explorer.Services.PackageVersionService;
using Chocolatey.Explorer.Services.PackagesService;
using Chocolatey.Explorer.Services.SourceService;
using Chocolatey.Explorer.View;
using Chocolatey.Explorer.View.Forms;

namespace Chocolatey.Explorer.IoC
{
    public class Registry: StructureMap.Configuration.DSL.Registry
    {
         public Registry()
         {
             // Services
             For<IPackageService>().Use<PackageService>();
             For<IPackagesService>().Use<CachedPackagesService>();
             For<IPackageVersionService>().Use<ODataPackageVersionService>();
             For<IChocolateyService>().Use<ChocolateyService>();
             For<IPackageVersionXMLParser>().Use<PackageVersionXMLParser>();
             For<IRunAsync>().Use<RunAsync>();
             For<IRunSync>().Use<RunSync>();
             For<ISourceService>().Singleton().Use<SourceService>();
			 For<IFileStorageService>().Use<LocalFileSystemStorageService>();
             For<ICommandExecuter>().Use<CommandExecuter>();

             // Forms
             For<IPackageManager>().Use<PackageManager>();
             For<IAbout>().Use<About>();
             For<IHelp>().Use<Help>();
             For<ISettings>().Use<Settings>();
             SetAllProperties(x => x.WithAnyTypeFromNamespace("Chocolatey.Explorer.View.Forms"));
         }
    }
}