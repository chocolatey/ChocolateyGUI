// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PersistenceService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using Microsoft.Win32;

namespace ChocolateyGui.Services
{
    public class PersistenceService : IPersistenceService
    {
        public Stream OpenFile(string defaultExtension, string filter)
        {
            var fd = new OpenFileDialog {DefaultExt = defaultExtension, Filter = filter};

            var result = fd.ShowDialog();

            return result != null && result.Value ? fd.OpenFile() : null;
        }

        public Stream SaveFile(string defaultExtension, string filter)
        {
            var fd = new SaveFileDialog {DefaultExt = defaultExtension, Filter = filter};

            var result = fd.ShowDialog();

            return result != null && result.Value ? fd.OpenFile() : null;
        }
    }
}