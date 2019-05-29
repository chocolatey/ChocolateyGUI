// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChocolateyGuiCliModule.cs" company="Chocolatey">
//  Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using Autofac;
using chocolatey;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.filesystem;
using chocolatey.infrastructure.services;
using ChocolateyGui.Common.Providers;
using ChocolateyGui.Common.Services;
using ChocolateyGuiCli.Commands;
using LiteDB;

namespace ChocolateyGuiCli.Startup
{
    internal class ChocolateyGuiCliModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register Providers
            builder.RegisterType<VersionNumberProvider>().As<IVersionNumberProvider>().SingleInstance();
            builder.RegisterType<ChocolateyConfigurationProvider>().As<IChocolateyConfigurationProvider>().SingleInstance();
            builder.RegisterType<ChocolateyService>().As<IChocolateyService>().SingleInstance();
            builder.RegisterType<DotNetFileSystem>().As<chocolatey.infrastructure.filesystem.IFileSystem>().SingleInstance();

            var choco = Lets.GetChocolatey();
            builder.RegisterInstance(choco.Container().GetInstance<IChocolateyConfigSettingsService>())
                .As<IChocolateyConfigSettingsService>().SingleInstance();
            builder.RegisterInstance(choco.Container().GetInstance<IXmlService>())
                .As<IXmlService>().SingleInstance();

            builder.RegisterType<PersistenceService>().As<IPersistenceService>().SingleInstance();
            builder.RegisterType<LiteDBFileStorageService>().As<IFileStorageService>().SingleInstance();
            builder.RegisterType<ConfigService>().As<IConfigService>().SingleInstance();
            builder.RegisterType<ChocolateyGuiCacheService>().As<IChocolateyGuiCacheService>().SingleInstance();

            var database = new LiteDatabase($"filename={Path.Combine(Bootstrapper.LocalAppDataPath, "data.db")};upgrade=true");
            builder.Register(c => database).SingleInstance();

            // Commands
            builder.RegisterType<FeatureCommand>().As<ICommand>().SingleInstance();
            builder.RegisterType<ConfigCommand>().As<ICommand>().SingleInstance();
            builder.RegisterType<PurgeCommand>().As<ICommand>().SingleInstance();
        }
    }
}