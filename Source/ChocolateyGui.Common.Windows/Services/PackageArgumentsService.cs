// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageArgumentsService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using chocolatey;
using chocolatey.infrastructure.adapters;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Providers;
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

        public PackageArgumentsService(IEncryptionUtility encryptionUtility, IFileSystem fileSystem, IChocolateyConfigurationProvider chocolateyConfigurationProvider)
        {
            _encryptionUtility = encryptionUtility;
            _fileSystem = fileSystem;
            _chocolateyConfigurationProvider = chocolateyConfigurationProvider;
        }

        public string DecryptPackageArgumentsFile(string id, string version)
        {
            var argumentsPath = _fileSystem.combine_paths(_chocolateyConfigurationProvider.ChocolateyInstall, ".chocolatey", "{0}.{1}".format_with(id, version));
            var argumentsFile = _fileSystem.combine_paths(argumentsPath, ".arguments");

            // Get the arguments decrypted in here and return them
            try
            {
                if (_fileSystem.file_exists(argumentsFile))
                {
                    return _encryptionUtility.decrypt_string(_fileSystem.read_file(argumentsFile));
                }

                return Resources.PackageView_UnableToFindArgumentsFile.format_with(version, id);
            }
            catch (Exception ex)
            {
                var message = Resources.Application_PackageArgumentsError.format_with(version, id);
                Logger.Error(ex, message);
                return message;
            }
        }
    }
}