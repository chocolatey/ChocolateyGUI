// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChocolateyGuiCliModule.cs" company="Chocolatey">
//  Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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

            builder.RegisterType<LiteDBFileStorageService>().As<IFileStorageService>().SingleInstance();
            builder.RegisterType<ConfigService>().As<IConfigService>().SingleInstance();
            builder.RegisterType<ChocolateyGuiCacheService>().As<IChocolateyGuiCacheService>().SingleInstance();

            try
            {
                var database = new LiteDatabase($"filename={Path.Combine(Bootstrapper.LocalAppDataPath, "data.db")};upgrade=true");
                builder.Register(c => database).SingleInstance();
            }
            catch (IOException ex)
            {
                Bootstrapper.Logger.Error(ex, Resources.Error_DatabaseAccessCli);
                Environment.Exit(-1);
            }

            // Commands
            // These are using Named registrations to aid with the "finding" of these components
            // within the Container.  As suggested in this Stack Overflow question:
            // https://stackoverflow.com/questions/4999000/replace-registration-in-autofac
            builder.RegisterType<FeatureCommand>().As<ICommand>().SingleInstance().Named<ICommand>(ApplicationParameters.FeatureCommandName);
            builder.RegisterType<ConfigCommand>().As<ICommand>().SingleInstance().Named<ICommand>(ApplicationParameters.ConfigCommandName);
            builder.RegisterType<PurgeCommand>().As<ICommand>().SingleInstance().Named<ICommand>(ApplicationParameters.PurgeCommandName);
        }
    }
}