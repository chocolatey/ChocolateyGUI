// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyGuiCacheService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using chocolatey.infrastructure.filesystem;
using ChocolateyGui.Common.Models;

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

            _localAppDataPath = _fileSystem.CombinePaths(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Chocolatey GUI");
        }

        public void PurgeIcons()
        {
            _fileStorageService.DeleteAllFiles();
        }

        public void PurgeOutdatedPackages()
        {
            PurgeOutdatedPackages(source: null, includePrerelease: false);
        }

        public void PurgeOutdatedPackages(ChocolateySource source, bool includePrerelease)
        {
            if (source == null)
            {
                var outdatedFiles = Directory.GetFiles(_localAppDataPath, "outdatedPackages*.xml");

                foreach (var outdatedFile in outdatedFiles)
                {
                    _fileSystem.DeleteFile(outdatedFile);
                }
            }
            else
            {
                var outdatedPackageFileName = "outdatedPackages";

                if (source != null)
                {
                    outdatedPackageFileName = $"{outdatedPackageFileName}-{source.Id}";
                }

                if (includePrerelease)
                {
                    outdatedPackageFileName = $"{outdatedPackageFileName}-pre.xml";
                }
                else
                {
                    outdatedPackageFileName = $"{outdatedPackageFileName}.xml";
                }

                if (_fileSystem.FileExists(outdatedPackageFileName))
                {
                    _fileSystem.DeleteFile(outdatedPackageFileName);
                }
            }
        }
    }
}