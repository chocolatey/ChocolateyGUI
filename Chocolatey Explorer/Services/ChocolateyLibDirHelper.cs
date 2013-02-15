using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chocolatey.Explorer.Model;
using Chocolatey.Explorer.Properties;
using log4net;
using Chocolatey.Explorer.Services.FileStorageService;

namespace Chocolatey.Explorer.Services
{
    /// <summary>
    /// Loads information about installed packages from
    /// chocolatey lib directory, specified by the setting
    /// "ChocolateyLibDirectory".
    /// </summary>
    public class ChocolateyLibDirHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ChocolateyLibDirHelper));

        private readonly Regex _packageVersionRegexp = new Regex(@"((\.\d+)+)$");
        private readonly char[] _segmentDelim = "\\".ToCharArray();
        private readonly Settings _settings = new Settings();
        private List<Package> _instaledPackages;
        private readonly IChocolateyService _chocolateyService;
		private readonly IFileStorageService _fileStorageService;
        private string _chocoVersion;

		public ChocolateyLibDirHelper() : this(new ChocolateyService(), new LocalFileSystemStorageService()) { }

        public ChocolateyLibDirHelper(IChocolateyService chocolateyService, IFileStorageService fileStorageService)
        {
			_chocolateyService = chocolateyService;
			_fileStorageService = fileStorageService;
            _chocolateyService.OutputChanged += VersionChangeFinished;
        }

        /// <summary>
        /// Reloads information about installed packages from
        /// chocolatey lib directory.
        /// </summary>
        public IList<Package> ReloadFromDir()
        {
            _instaledPackages = new List<Package>();
            var expandedLibDirectory = Environment.ExpandEnvironmentVariables(_settings.ChocolateyLibDirectory);
			var directories = _fileStorageService.GetDirectories(expandedLibDirectory);

            foreach (string directoryPath in directories)
            {
                string[] directoryPathSegments = directoryPath.Split(_segmentDelim);
                string directoryName = directoryPathSegments.Last();

                Package package = GetPackageFromDirectoryName(directoryName);
                _instaledPackages.Add(package);
            }
            //add chocolatey by default because else this won't work anyway
            _chocolateyService.Help();
            var chocoPackage = new Package { Name = "chocolatey", InstalledVersion = _chocoVersion };
            _instaledPackages.Add(chocoPackage);
            return _instaledPackages;
        }

        private void VersionChangeFinished(string version)
        {
			var match = Regex.Match(version, "Version:[ ]+'([0-9\\.]+)'");

			if (!match.Success)
				throw new ChocolateyVersionUnknownException(version);

			_chocoVersion = match.Groups[1].Value;
        }

        /// <summary>
        /// Searches the chocolatey install directory to look up 
        /// the installed version of the given package.
        /// </summary>
        public string GetHighestInstalledVersion(string packageName, bool reload=true)
        {
            if (reload || _instaledPackages == null)
            {
                ReloadFromDir();
            }
            var versionQuery =
                from package in _instaledPackages
                where package.Name == packageName
                select package.InstalledVersion;

            var query = versionQuery as string[] ?? versionQuery.ToArray();
            return !query.Any() ? strings.not_available : query.OrderBy(x=> x, new PackagesSorter()).Last();
        }

        private Package GetPackageFromDirectoryName(string directoryName)
        {
            var package = new Package();
            var versionMatch = _packageVersionRegexp.Match(directoryName);
            package.Name = directoryName.Substring(0, versionMatch.Index);
            package.InstalledVersion = directoryName.Substring(versionMatch.Index + 1);
            return package;
        }
    }
}
