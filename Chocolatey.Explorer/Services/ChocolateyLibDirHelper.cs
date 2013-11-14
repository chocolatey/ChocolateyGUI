using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Services.ChocolateyService;
using Chocolatey.Explorer.Services.FileStorageService;
using Chocolatey.Explorer.Services.SettingsService;

namespace Chocolatey.Explorer.Services
{
    /// <summary>
    /// Loads information about installed packages from
    /// chocolatey lib directory, specified by the setting
    /// "ChocolateyLibDirectory".
    /// </summary>
    public class ChocolateyLibDirHelper : IChocolateyLibDirHelper
    {
        private readonly Regex _packageVersionRegexp = new Regex(@"((?:\.\d+){1,4})(-[^\.]+)?$");
        private readonly char[] _segmentDelim = "\\".ToCharArray();
        private List<Package> _instaledPackages;
        private readonly IChocolateyService _chocolateyService;
		private readonly IFileStorageService _fileStorageService;
        private readonly ISettingsService _settingsService;
        private string _chocoVersion;

		public ChocolateyLibDirHelper(IChocolateyService chocolateyService, IFileStorageService fileStorageService, ISettingsService settingsService)
        {
			_chocolateyService = chocolateyService;
			_fileStorageService = fileStorageService;
            _settingsService = settingsService;
            _chocolateyService.OutputChanged += VersionChangeFinished;
        }

        /// <summary>
        /// Reloads information about installed packages from
        /// chocolatey lib directory.
        /// </summary>
        public IList<Package> ReloadFromDir()
        {
            this.Log().Debug("Reloadfromdir");
            _instaledPackages = new List<Package>();
            var expandedLibDirectory = Environment.ExpandEnvironmentVariables(_settingsService.ChocolateyLibDirectory);
			var directories = _fileStorageService.GetDirectories(expandedLibDirectory);

            foreach (var directoryPath in directories)
            {
                this.Log().Debug("Found directory: {0}", directoryPath);
                var directoryPathSegments = directoryPath.Split(_segmentDelim);
                var directoryName = directoryPathSegments.Last();

                var package = GetPackageFromDirectoryName(directoryName);
                _instaledPackages.Add(package);
            }
            //add chocolatey by default because else this won't work anyway
            _chocolateyService.Help();
            var chocoPackage = new Package { Name = "chocolatey", InstalledVersion = _chocoVersion };
            this.Log().Debug("Chocolatey package added: {0}", chocoPackage);
            _instaledPackages.Add(chocoPackage);
            return _instaledPackages;
        }

        private void VersionChangeFinished(string version)
        {
            this.Log().Debug("VersionChangeFinished for version: {0}", version);

            // Possible matches incclude things like the following
            // Version: '0.9.8.21'
            // Version: '0.9.8.21-alpha3'
            // The most typical is the four version numbers, but to account for the format string used by
            // Chocolatey itself, the additional optionals were added to the regular expression
            var match = Regex.Match(version, @"Version:\s+'([0-9\\.]+)(?:-[a-z]{1,5}?\d?)?'");

            if (!match.Success)
            {
                this.Log().Debug("Chocolatey version not found");
                throw new ChocolateyVersionUnknownException(version);
            }

            _chocoVersion = match.Groups[1].Value;
        }

        /// <summary>
        /// Searches the chocolatey install directory to look up 
        /// the installed version of the given package.
        /// </summary>
        public Package GetHighestInstalledVersion(string packageName, bool reload=true)
        {
            this.Log().Debug("Find highest version for: {0}", packageName);
            if (reload || _instaledPackages == null)
            {
                this.Log().Debug("With reload");
                ReloadFromDir();
            }
            var versionQuery =
                from package in _instaledPackages
                where package.Name == packageName
                select package;
            var query = versionQuery as Package[] ?? versionQuery.ToArray();
            return !query.Any() ? null : query.OrderBy(x=> x.InstalledVersion, new PackagesSorter()).Last();
        }

        public Package GetPackageFromDirectoryName(string directoryName)
        {
            this.Log().Debug("Get packagename for directory: {0}", directoryName);
            var package = new Package();
            var versionMatch = _packageVersionRegexp.Match(directoryName);
            package.Name = directoryName.Substring(0, versionMatch.Index);
			package.InstalledVersion = versionMatch.Groups[1].Value.TrimStart('.');
			package.IsPreRelease = versionMatch.Groups.Count == 3 && !string.IsNullOrWhiteSpace(versionMatch.Groups[2].Value);
            return package;
        }
    }
}
