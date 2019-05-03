// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="GenericRunner.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using chocolatey;
using chocolatey.infrastructure.app;
using ChocolateyGui.Attributes;
using ChocolateyGui.Properties;

namespace ChocolateyGui.CliCommands
{
    public sealed class GenericRunner
    {
        public void Run(ChocolateyGuiConfiguration configuration, IContainer container, Action<ICommand> parseArgs)
        {
            var command = FindCommand(configuration, container, parseArgs);

            if (configuration.HelpRequested || configuration.UnsuccessfulParsing)
            {
                Bootstrapper.Container.Dispose();
                Environment.Exit(configuration.UnsuccessfulParsing ? 1 : 0);
            }

            if (command != null)
            {
                Bootstrapper.Logger.Debug("_ {0}:{1} - Normal Run Mode _".format_with(ApplicationParameters.Name, command.GetType().Name));
                command.Run(configuration);
            }
        }

        private ICommand FindCommand(ChocolateyGuiConfiguration configuration, IContainer container, Action<ICommand> parseArgs)
        {
            var commands = container.Resolve<IEnumerable<ICommand>>();
            var command = commands.Where((c) =>
            {
                var attributes = c.GetType().GetCustomAttributes(typeof(LocalizedCommandForAttribute), false);
                return attributes.Cast<LocalizedCommandForAttribute>().Any(attribute => attribute.CommandName.is_equal_to(configuration.CommandName));
            }).FirstOrDefault();

            if (command == null)
            {
                if (!string.IsNullOrWhiteSpace(configuration.CommandName))
                {
                    Bootstrapper.Logger.Error(Resources.Command_NotFoundError.format_with(configuration.CommandName));
                    Bootstrapper.Container.Dispose();
                    Environment.Exit(-1);
                }
            }
            else
            {
                if (parseArgs != null)
                {
                    parseArgs.Invoke(command);
                }
            }

            return command;
        }
    }
}