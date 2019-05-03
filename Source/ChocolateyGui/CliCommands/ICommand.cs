using System.Collections.Generic;
using chocolatey.infrastructure.commandline;

namespace ChocolateyGui.CliCommands
{
    /// <summary>
    ///   Commands that can be configured and run
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///   Configure the argument parser.
        /// </summary>
        /// <param name="optionSet">The option set.</param>
        /// <param name="configuration">The configuration.</param>
        void configure_argument_parser(OptionSet optionSet, ChocolateyGuiConfiguration configuration);

        /// <summary>
        ///   Handle the arguments that were not parsed by the argument parser and/or do additional parsing work
        /// </summary>
        /// <param name="unparsedArguments">The unparsed arguments.</param>
        /// <param name="configuration">The configuration.</param>
        void handle_additional_argument_parsing(IList<string> unparsedArguments, ChocolateyGuiConfiguration configuration);

        void handle_validation(ChocolateyGuiConfiguration configuration);

        /// <summary>
        ///   The specific help message for a particular command.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        void help_message(ChocolateyGuiConfiguration configuration);

        /// <summary>
        ///   Runs the command.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void run(ChocolateyGuiConfiguration config);
    }
}
