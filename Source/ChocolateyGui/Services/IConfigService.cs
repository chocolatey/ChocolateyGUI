// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IConfigService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using ChocolateyGui.CliCommands;
using ChocolateyGui.Models;

namespace ChocolateyGui.Services
{
    public interface IConfigService
    {
        AppConfiguration GetAppConfiguration();

        void UpdateSettings(AppConfiguration settings);

        IEnumerable<ChocolateyGuiFeature> GetFeatures();

        void ListFeatures(ChocolateyGuiConfiguration configuration);

        IEnumerable<ChocolateyGuiSetting> GetSettings();

        void ListSettings(ChocolateyGuiConfiguration configuration);

        void EnableFeature(ChocolateyGuiConfiguration configuration);

        void DisableFeature(ChocolateyGuiConfiguration configuration);

        void GetConfigValue(ChocolateyGuiConfiguration configuration);

        void SetConfigValue(ChocolateyGuiConfiguration configuration);

        void UnsetConfigValue(ChocolateyGuiConfiguration configuration);
    }
}