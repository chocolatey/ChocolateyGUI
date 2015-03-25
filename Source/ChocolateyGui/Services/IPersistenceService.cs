// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IPersistenceService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Services
{
    using System.IO;

    public interface IPersistenceService
    {
        Stream OpenFile(string defaultExtension, string filter);

        Stream SaveFile(string defaultExtension, string filter);
    }
}