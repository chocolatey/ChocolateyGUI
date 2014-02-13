using System;
using System.Configuration;
using System.Linq;
using Autofac;
using Chocolatey.Gui.ChocolateyFeedService;
using Chocolatey.Gui.IoC;
using Chocolatey.Gui.Models;
using Chocolatey.Gui.ViewModels.Items;

namespace Chocolatey.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal static IContainer Container { get; set; }

        static App()
        {
            Container = AutoFacConfiguration.RegisterAutoFac();

            AutoMapper.Mapper.CreateMap<V2FeedPackage, PackageViewModel>();
            AutoMapper.Mapper.CreateMap<PackageMetadata, PackageViewModel>();

            var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var config = (ChocoConfigurationSection)appConfig.GetSection(ChocoConfigurationSection.SectionName);
            config.SectionInformation.ForceSave = true;

            if (string.IsNullOrWhiteSpace(config.ChocolateyInstall.Path))
            {
                var chocoDirectoryPath = Environment.GetEnvironmentVariable("ChocolateyInstall");
                if (string.IsNullOrWhiteSpace(chocoDirectoryPath))
                {
                    var pathVar = Environment.GetEnvironmentVariable("PATH");
                    if (!string.IsNullOrWhiteSpace(pathVar))
                    {
                        chocoDirectoryPath = pathVar.Split(';').SingleOrDefault(path => path.IndexOf("Chocolatey", StringComparison.OrdinalIgnoreCase) > -1);
                    }
                }

                if (!string.IsNullOrWhiteSpace(chocoDirectoryPath))
                {
                    config.ChocolateyInstall.Path = chocoDirectoryPath;
                    appConfig.Save(ConfigurationSaveMode.Full);
                }
            }
        }
    }
}
