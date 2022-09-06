// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IConfigFileWatcher.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;

namespace ChocolateyGui.Common.Services
{
    public interface IConfigFileWatcher
    {
        void OnConfigFileChanged(object sender, FileSystemEventArgs e);
    }
}