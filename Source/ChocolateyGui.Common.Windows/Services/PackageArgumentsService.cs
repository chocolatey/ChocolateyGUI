// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageArgumentsService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using chocolatey;
using chocolatey.infrastructure.adapters;
using chocolatey.infrastructure.app.utility;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Providers;
using ChocolateyGui.Common.Utilities;
using IFileSystem = chocolatey.infrastructure.filesystem.IFileSystem;
using ILogger = Serilog.ILogger;

namespace ChocolateyGui.Common.Windows.Services
{
    public class PackageArgumentsService : IPackageArgumentsService
    {
        private static readonly ILogger Logger = Serilog.Log.ForContext<PackageArgumentsService>();
        private readonly IEncryptionUtility _encryptionUtility;
        private readonly IFileSystem _fileSystem;
        private readonly IChocolateyConfigurationProvider _chocolateyConfigurationProvider;
        private readonly IDialogService _dialogService;

        public PackageArgumentsService(
            IEncryptionUtility encryptionUtility,
            IFileSystem fileSystem,
            IChocolateyConfigurationProvider chocolateyConfigurationProvider,
            IDialogService dialogService)
        {
            _encryptionUtility = encryptionUtility;
            _fileSystem = fileSystem;
            _chocolateyConfigurationProvider = chocolateyConfigurationProvider;
            _dialogService = dialogService;
        }

        public IEnumerable<string> DecryptPackageArgumentsFile(string id, string version)
        {
            var argumentsPath = _fileSystem.combine_paths(_chocolateyConfigurationProvider.ChocolateyInstall, ".chocolatey", "{0}.{1}".format_with(id, version));
            var argumentsFile = _fileSystem.combine_paths(argumentsPath, ".arguments");

            string arguments = string.Empty;

            // Get the arguments decrypted in here and return them
            try
            {
                if (_fileSystem.file_exists(argumentsFile))
                {
                    arguments = _fileSystem.read_file(argumentsFile);
                }
            }
            catch (Exception ex)
            {
                var message = L(nameof(Resources.Application_PackageArgumentsError), version, id);
                Logger.Error(ex, message);
            }

            if (string.IsNullOrEmpty(arguments))
            {
                Logger.Debug(
                    string.Empty,
                    L(nameof(Resources.PackageView_UnableToFindArgumentsFile), version, id));
                yield break;
            }

            // The following code is borrowed from the Chocolatey codebase, should
            // be extracted to a separate location in choco executable so we can re-use it.
            var packageArgumentsUnencrypted = arguments.contains(" --") && arguments.to_string().Length > 4
                ? arguments
                : _encryptionUtility.decrypt_string(arguments);

            // Lets do a global check first to see if there are any sensitive arguments
            // before we filter out the values used later.
            var sensitiveArgs = ArgumentsUtility.arguments_contain_sensitive_information(packageArgumentsUnencrypted);

            var packageArgumentsSplit =
                packageArgumentsUnencrypted.Split(new[] { " --" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var packageArgument in packageArgumentsSplit.or_empty_list_if_null())
            {
                var isSensitiveArgument = sensitiveArgs && ArgumentsUtility.arguments_contain_sensitive_information(string.Concat("--", packageArgument));

                var packageArgumentSplit =
                    packageArgument.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                var optionName = packageArgumentSplit[0].to_string();
                var optionValue = string.Empty;

                if (packageArgumentSplit.Length == 2 && isSensitiveArgument)
                {
                    optionValue = L(nameof(Resources.PackageArgumentService_RedactedArgument));
                }
                else if (packageArgumentSplit.Length == 2)
                {
                    optionValue = packageArgumentSplit[1].to_string().remove_surrounding_quotes();
                    if (optionValue.StartsWith("'"))
                    {
                        optionValue.remove_surrounding_quotes();
                    }
                }

                yield return "--{0}{1}".format_with(
                    optionName,
                    string.IsNullOrWhiteSpace(optionValue) ? string.Empty : "=" + optionValue);
            }
        }

        private static string L(string key)
        {
            return TranslationSource.Instance[key];
        }

        private static string L(string key, params object[] parameters)
        {
            return TranslationSource.Instance[key, parameters];
        }
    }
}