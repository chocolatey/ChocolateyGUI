// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChocolateyGuiCliModule.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using Autofac;
using chocolatey;
using chocolatey.infrastructure.app.services;
using chocolatey.infrastructure.filesystem;
using chocolatey.infrastructure.services;
using ChocolateyGui.Common;
using ChocolateyGui.Common.Commands;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Providers;
using ChocolateyGui.Common.Services;
using LiteDB;

namespace ChocolateyGuiCli.Startup
{
    internal class ChocolateyGuiCliModule : Module
    {
        private static readonly string FeatureCommandName = "Feature";
        private static readonly string ConfigCommandName = "Config";
        private static readonly string PurgeCommandName = "Purge";

        protected override void Load(ContainerBuilder builder)
        {
            // Register Providers
            builder.RegisterType<VersionNumberProvider>().As<IVersionNumberProvider>().SingleInstance();
            builder.RegisterType<ChocolateyConfigurationProvider>().As<IChocolateyConfigurationProvider>().SingleInstance();
            builder.RegisterType<DotNetFileSystem>().As<chocolatey.infrastructure.filesystem.IFileSystem>().SingleInstance();

            var choco = Lets.GetChocolatey();
            builder.RegisterInstance(choco.Container().GetInstance<IChocolateyConfigSettingsService>())
                .As<IChocolateyConfigSettingsService>().SingleInstance();
            builder.RegisterInstance(choco.Container().GetInstance<IXmlService>())
                .As<IXmlService>().SingleInstance();

            builder.RegisterType<ChocolateyGuiCacheService>().As<IChocolateyGuiCacheService>().SingleInstance();

            try
            {
                var userDatabase = new LiteDatabase($"filename={Path.Combine(Bootstrapper.LocalAppDataPath, "data.db")};upgrade=true");

                LiteDatabase globalDatabase;
                if (Hacks.IsElevated)
                {
                    globalDatabase = new LiteDatabase($"filename={Path.Combine(Bootstrapper.AppDataPath, "Config", "data.db")};upgrade=true");
                }
                else
                {
                    if (!File.Exists(Path.Combine(Bootstrapper.AppDataPath, "Config", "data.db")))
                    {
                        // Since the global configuration database file doesn't exist, we must be running in a state where an administrator user
                        // has never run Chocolatey GUI. In this case, use null, which will mean attempts to use the global database will be ignored.
                        globalDatabase = null;
                    }
                    else
                    {
                        // Since this is a non-administrator user, they should only have read permissions to this database
                        globalDatabase = new LiteDatabase($"filename={Path.Combine(Bootstrapper.AppDataPath, "Config", "data.db")};readonly=true");
                    }
                }

                if (globalDatabase != null)
                {
                    builder.RegisterInstance(globalDatabase).As<LiteDatabase>().SingleInstance().Named<LiteDatabase>(Bootstrapper.GlobalConfigurationDatabaseName);
                }

                var configService = new ConfigService(globalDatabase, userDatabase);
                configService.SetEffectiveConfiguration();

                builder.RegisterInstance(configService).As<IConfigService>().SingleInstance();
                builder.RegisterInstance(new LiteDBFileStorageService(userDatabase)).As<IFileStorageService>().SingleInstance();

                // Since there are two instances of LiteDB, they are added as named instances, so that they can be retrieved when required
                builder.RegisterInstance(userDatabase).As<LiteDatabase>().SingleInstance().Named<LiteDatabase>(Bootstrapper.UserConfigurationDatabaseName);
            }
            catch (IOException ex)
            {
                Bootstrapper.Logger.Error(ex, Resources.Error_DatabaseAccessCli);
                Environment.Exit(-1);
            }

            // Services
            builder.RegisterType<VersionService>().As<IVersionService>().SingleInstance();

            // Commands
            // These are using Named registrations to aid with the "finding" of these components
            // within the Container.  As suggested in this Stack Overflow question:
            // https://stackoverflow.com/questions/4999000/replace-registration-in-autofac
            builder.RegisterType<FeatureCommand>().As<ICommand>().SingleInstance().Named<ICommand>(FeatureCommandName);
            builder.RegisterType<ConfigCommand>().As<ICommand>().SingleInstance().Named<ICommand>(ConfigCommandName);
            builder.RegisterType<PurgeCommand>().As<ICommand>().SingleInstance().Named<ICommand>(PurgeCommandName);
        }
    }
}