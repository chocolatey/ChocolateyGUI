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
            var argumentsPath = _fileSystem.CombinePaths(_chocolateyConfigurationProvider.ChocolateyInstall, ".chocolatey", "{0}.{1}".FormatWith(id, version));
            var argumentsFile = _fileSystem.CombinePaths(argumentsPath, ".arguments");

            string arguments = string.Empty;

            // Get the arguments decrypted in here and return them
            try
            {
                if (_fileSystem.FileExists(argumentsFile))
                {
                    arguments = _fileSystem.ReadFile(argumentsFile);
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
            var packageArgumentsUnencrypted = arguments.Contains(" --") && arguments.ToStringSafe().Length > 4
                ? arguments
                : _encryptionUtility.DecryptString(arguments);

            // Lets do a global check first to see if there are any sensitive arguments
            // before we filter out the values used later.
            var sensitiveArgs = ArgumentsUtility.SensitiveArgumentsProvided(packageArgumentsUnencrypted);

            var packageArgumentsSplit =
                packageArgumentsUnencrypted.Split(new[] { " --" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var packageArgument in packageArgumentsSplit.OrEmpty())
            {
                var isSensitiveArgument = sensitiveArgs && ArgumentsUtility.SensitiveArgumentsProvided(string.Concat("--", packageArgument));

                var packageArgumentSplit =
                    packageArgument.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                var optionName = packageArgumentSplit[0].ToStringSafe();
                var optionValue = string.Empty;

                if (packageArgumentSplit.Length == 2 && isSensitiveArgument)
                {
                    optionValue = L(nameof(Resources.PackageArgumentService_RedactedArgument));
                }
                else if (packageArgumentSplit.Length == 2)
                {
                    optionValue = packageArgumentSplit[1].ToStringSafe().UnquoteSafe();
                    if (optionValue.StartsWith("'"))
                    {
                        optionValue.UnquoteSafe();
                    }
                }

                yield return "--{0}{1}".FormatWith(
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