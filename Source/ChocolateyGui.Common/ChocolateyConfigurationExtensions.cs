namespace ChocolateyGui.Common
{
    using System;
    using chocolatey;
    using chocolatey.infrastructure.app.configuration;

    public static class ChocolateyConfigurationExtensions
    {
        [Obsolete("This is used only for backwards compatibility, should be removed when updating Chocolatey CLI reference in the nuspec.")]
        public static bool HasCacheExpirationInMinutes()
        {
            var configType = typeof(ChocolateyConfiguration);

            return configType.GetProperty("CacheExpirationInMinutes") != null;
        }

        [Obsolete("This is used only for backwards compatibility, should be removed when updating Chocolatey CLI reference in the nuspec.")]
        public static void SetCacheExpirationInMinutes(this ChocolateyConfiguration config, int cacheExpirationInMinutes)
        {
            var configType = config.GetType();

            var property = configType.GetProperty("CacheExpirationInMinutes");

            if (property != null)
            {
                property.SetValue(config, cacheExpirationInMinutes);
            }
            else
            {
                "chocolatey".Log().Warn("CacheExpirationInMinutes property is not available. Unable to ignore existing cached items!");
            }
        }
    }
}