// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IConfigService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using ChocolateyGui.Common.Models;

namespace ChocolateyGui.Common.Services
{
    public interface IConfigService
    {
        AppConfiguration GetEffectiveConfiguration();

        AppConfiguration GetGlobalConfiguration();

        void UpdateSettings(AppConfiguration settings, bool global);

        IEnumerable<ChocolateyGuiFeature> GetFeatures(bool global);

        void ListFeatures(ChocolateyGuiConfiguration configuration);

        IEnumerable<ChocolateyGuiSetting> GetSettings(bool global);

        void ListSettings(ChocolateyGuiConfiguration configuration);

        void ToggleFeature(ChocolateyGuiConfiguration configuration, bool requiredValue);

        void GetConfigValue(ChocolateyGuiConfiguration configuration);

        void SetConfigValue(ChocolateyGuiConfiguration configuration);

        void UnsetConfigValue(ChocolateyGuiConfiguration configuration);
    }
}