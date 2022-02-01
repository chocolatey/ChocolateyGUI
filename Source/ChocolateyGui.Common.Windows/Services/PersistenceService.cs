// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PersistenceService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using ChocolateyGui.Common.Services;
using Microsoft.Win32;
using DialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace ChocolateyGui.Common.Windows.Services
{
    public class PersistenceService : IPersistenceService
    {
        public Stream OpenFile(string defaultExtension, string filter)
        {
            var fd = new OpenFileDialog { DefaultExt = defaultExtension, Filter = filter };

            var result = fd.ShowDialog();

            return result != null && result.Value ? fd.OpenFile() : null;
        }

        public Stream SaveFile(string defaultExtension, string filter)
        {
            var fd = new SaveFileDialog { DefaultExt = defaultExtension, Filter = filter };

            var result = fd.ShowDialog();

            return result != null && result.Value ? fd.OpenFile() : null;
        }

        public string GetFolderPath(string defaultLocation, string description = null)
        {
            var fd = new FolderBrowserDialog();
            fd.SelectedPath = defaultLocation;
            fd.Description = description;

            if (fd.ShowDialog() == DialogResult.OK)
            {
                var path = fd.SelectedPath;

                return path;
            }

            return null;
        }

        public string GetFilePath(string defaultExtension, string filter)
        {
            var fd = new SaveFileDialog { DefaultExt = defaultExtension, Filter = filter };

            var result = fd.ShowDialog();

            return result != null && result.Value ? fd.FileName : null;
        }
    }
}