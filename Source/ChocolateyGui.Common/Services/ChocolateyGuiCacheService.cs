// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyGuiCacheService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using chocolatey.infrastructure.filesystem;

namespace ChocolateyGui.Common.Services
{
    public class ChocolateyGuiCacheService : IChocolateyGuiCacheService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IFileSystem _fileSystem;

        private string _localAppDataPath = string.Empty;

        public ChocolateyGuiCacheService(IFileStorageService fileStorageService, IFileSystem fileSystem)
        {
            _fileStorageService = fileStorageService;
            _fileSystem = fileSystem;

            _localAppDataPath = _fileSystem.combine_paths(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Chocolatey GUI");
        }

        public void PurgeIcons()
        {
            _fileStorageService.DeleteAllFiles();
        }

        public void PurgeOutdatedPackages()
        {
            var outdatedPackagesFile = _fileSystem.combine_paths(_localAppDataPath, "outdatedPackages.xml");
            var outdatedPackagesBackupFile = _fileSystem.combine_paths(_localAppDataPath, "outdatedPackages.xml.backup");

            if (_fileSystem.file_exists(outdatedPackagesFile))
            {
                _fileSystem.delete_file(outdatedPackagesFile);
            }

            if (_fileSystem.file_exists(outdatedPackagesBackupFile))
            {
                _fileSystem.delete_file(outdatedPackagesBackupFile);
            }
        }
    }
}