using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chocolatey.Gui.Services
{
    public interface IChocolateyService
    {
        void InstallPackage(string id, string version = null, string source = null);
        void UninstallPackage(string id, string version = null, bool force = false);
        void UpdatePackage(string id);
        bool IsPackageInstalled(string id, string version);
    }
}
