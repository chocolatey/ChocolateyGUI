using System.Collections.Generic;
using Chocolatey.Explorer.Model;

namespace Chocolatey.Explorer.Services
{
    public interface IChocolateyLibDirHelper
    {
        /// <summary>
        /// Reloads information about installed packages from
        /// chocolatey lib directory.
        /// </summary>
        IList<Package> ReloadFromDir();

        /// <summary>
        /// Searches the chocolatey install directory to look up 
        /// the installed version of the given package.
        /// </summary>
        Package GetHighestInstalledVersion(string packageName, bool reload=true);

        Package GetPackageFromDirectoryName(string directoryName);
    }
}