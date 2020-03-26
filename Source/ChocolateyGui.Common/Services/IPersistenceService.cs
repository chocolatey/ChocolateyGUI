// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IPersistenceService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;

namespace ChocolateyGui.Common.Services
{
    public interface IPersistenceService
    {
        Stream OpenFile(string defaultExtension, string filter);

        Stream SaveFile(string defaultExtension, string filter);
    }
}