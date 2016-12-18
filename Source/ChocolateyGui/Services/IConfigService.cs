// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IConfigService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Models;

namespace ChocolateyGui.Services
{
    public interface IConfigService
    {
        AppConfiguration GetSettings();

        void UpdateSettings(AppConfiguration settings);
    }
}
