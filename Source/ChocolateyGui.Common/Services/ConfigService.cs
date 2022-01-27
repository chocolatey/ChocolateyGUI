// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigService.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using chocolatey;
using ChocolateyGui.Common.Attributes;
using ChocolateyGui.Common.Models;
using ChocolateyGui.Common.Properties;
using ChocolateyGui.Common.Utilities;
using LiteDB;
using Serilog;

namespace ChocolateyGui.Common.Services
{
    public class ConfigService : IConfigService
    {
        private static readonly TranslationSource TranslationSource = TranslationSource.Instance;

        public ConfigService(LiteDatabase globalDatabase, LiteDatabase userDatabase)
        {
            var defaultGlobalSettings = new AppConfiguration()
            {
                Id = "v0.18.0",
                OutdatedPackagesCacheDurationInMinutes = "60",
                UseKeyboardBindings = true,
                DefaultToTileViewForLocalSource = true,
                DefaultToTileViewForRemoteSource = true,
                NumberOfPackageVersionsForSelection = "25"
            };

            var defaultUserSettings = new AppConfiguration()
            {
                Id = "v0.18.0"
            };

            // If the global database is null, the assumption has to be that we are running as a non-administrator
            // user, as such, we should proceed with default settings
            if (globalDatabase == null)
            {
                GlobalCollection = null;
                GlobalAppConfiguration = defaultGlobalSettings;
            }
            else
            {
                GlobalCollection = globalDatabase.GetCollection<AppConfiguration>(nameof(AppConfiguration));
                GlobalAppConfiguration = GlobalCollection.FindById("v0.18.0") ?? defaultGlobalSettings;
            }

            UserCollection = userDatabase.GetCollection<AppConfiguration>(nameof(AppConfiguration));
            UserAppConfiguration = UserCollection.FindById("v0.18.0") ?? defaultUserSettings;
        }

        public event EventHandler SettingsChanged;

        public static ILogger Logger
        {
            get
            {
                return Serilog.Log.ForContext<ConfigService>();
            }
        }

        public AppConfiguration EffectiveAppConfiguration { get; set; }

        public AppConfiguration UserAppConfiguration { get; set; }

        public AppConfiguration GlobalAppConfiguration { get; set; }

        public ILiteCollection<AppConfiguration> GlobalCollection { get; set; }

        public ILiteCollection<AppConfiguration> UserCollection { get; set; }

        public virtual void SetEffectiveConfiguration()
        {
            EffectiveAppConfiguration = new AppConfiguration();

            var properties = typeof(AppConfiguration).GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == "Id")
                {
                    continue;
                }

                var featureAttributes = property.GetCustomAttributes(typeof(FeatureAttribute), true);

                object globalPropertyValue;
                object userPropertyValue;

                if (featureAttributes.Length > 0)
                {
                    globalPropertyValue = GetPropertyValue<bool?>(GlobalAppConfiguration, property);
                    userPropertyValue = GetPropertyValue<bool?>(UserAppConfiguration, property);
                }
                else
                {
                    globalPropertyValue = (string)property.GetValue(GlobalAppConfiguration);
                    userPropertyValue = (string)property.GetValue(UserAppConfiguration);
                }

                // Neither the user or global values have been set, so do nothing
                if (userPropertyValue == null && globalPropertyValue == null)
                {
                    continue;
                }

                // If the user hasn't explicitly set a value, take the global value
                if (userPropertyValue == null)
                {
                    property.SetValue(EffectiveAppConfiguration, globalPropertyValue);
                    continue;
                }

                // If we get here, we know userPropertyValue is not null, so if globalPropertyValue
                // hasn't been set, take the userPropertyValue
                if (globalPropertyValue == null)
                {
                    property.SetValue(EffectiveAppConfiguration, userPropertyValue);
                    continue;
                }

                // At this point, both aren't null, so if they are the same, use global
                if (globalPropertyValue.Equals(userPropertyValue))
                {
                    property.SetValue(EffectiveAppConfiguration, globalPropertyValue);
                    continue;
                }

                // If they aren't the same, use user, since user can override
                property.SetValue(EffectiveAppConfiguration, userPropertyValue);
            }

            Logger.Debug("GlobalAppConfiguration Settings");
            Logger.Debug(GlobalAppConfiguration.ToString());
            Logger.Debug("EffectiveAppConfiguration Settings");
            Logger.Debug(EffectiveAppConfiguration.ToString());
            Logger.Debug("UserAppConfiguration settings");
            Logger.Debug(UserAppConfiguration.ToString());
        }

        public AppConfiguration GetEffectiveConfiguration()
        {
            if (EffectiveAppConfiguration == null)
            {
                Logger.Information("Calling SettingEffectiveConfiguration from OSS");
                SetEffectiveConfiguration();
            }

            return EffectiveAppConfiguration;
        }

        public AppConfiguration GetGlobalConfiguration()
        {
            return GlobalAppConfiguration;
        }

        public void UpdateSettings(AppConfiguration settings, bool global)
        {
            if (global && GlobalCollection == null)
            {
                // This is very much an edge case, and we shouldn't ever get to here, but it does need to be handled
                Logger.Warning("An attempt has been made to save a configuration change globally, when the global configuration database hasn't been created.");
                Logger.Warning("No action will be taken, please check with your System Administrator.");
                return;
            }

            var settingsCollection = global ? GlobalCollection : UserCollection;

            if (settingsCollection.Exists(Query.EQ("_id", "v0.18.0")))
            {
                settingsCollection.Update("v0.18.0", settings);
            }
            else
            {
                try
                {
                    settingsCollection.Insert(settings);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            SettingsChanged?.Invoke(GetEffectiveConfiguration(), EventArgs.Empty);
        }

        public IEnumerable<ChocolateyGuiFeature> GetFeatures(bool global)
        {
            return GetFeatures(global, useResourceKeys: false);
        }

        public IEnumerable<ChocolateyGuiFeature> GetFeatures(bool global, bool useResourceKeys)
        {
            var features = new List<ChocolateyGuiFeature>();

            var properties = typeof(AppConfiguration).GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;

                var featureAttributes = property.GetCustomAttributes(typeof(FeatureAttribute), true);
                if (property.Name != "Id" && featureAttributes.Length > 0)
                {
                    var propertyValue = (bool?)property.GetValue(global ? GlobalAppConfiguration : EffectiveAppConfiguration);

                    features.Add(new ChocolateyGuiFeature { Description = GetDescriptionFromProperty(property, useResourceKeys), Enabled = propertyValue ?? false, Title = propertyName });
                }
            }

            return features.OrderBy(f => f.Title);
        }

        public void ListFeatures(ChocolateyGuiConfiguration configuration)
        {
            foreach (var feature in GetFeatures(configuration.Global))
            {
                if (configuration.RegularOutput)
                {
                    Logger.Information("{0} {1} - {2}".format_with(feature.Enabled ? "[x]" : "[ ]", feature.Title, feature.Description));
                }
                else
                {
                    Logger.Information("{0}|{1}|{2}".format_with(feature.Title, L(!feature.Enabled ? nameof(Resources.FeatureCommand_Disabled) : nameof(Resources.FeatureCommand_Enabled)), feature.Description));
                }
            }
        }

        public IEnumerable<ChocolateyGuiSetting> GetSettings(bool global)
        {
            return GetSettings(global, useResourceKeys: false);
        }

        public IEnumerable<ChocolateyGuiSetting> GetSettings(bool global, bool useResourceKeys)
        {
            var settings = new List<ChocolateyGuiSetting>();

            var properties = typeof(AppConfiguration).GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;

                var configAttributes = property.GetCustomAttributes(typeof(ConfigAttribute), true);
                if (property.Name != "Id" && configAttributes.Length > 0)
                {
                    var propertyValue = (string)property.GetValue(global ? GlobalAppConfiguration : EffectiveAppConfiguration);

                    settings.Add(new ChocolateyGuiSetting { Description = GetDescriptionFromProperty(property, useResourceKeys), Value = propertyValue, Key = propertyName });
                }
            }

            return settings.OrderBy(s => s.Key);
        }

        public void ListSettings(ChocolateyGuiConfiguration configuration)
        {
            foreach (var setting in GetSettings(configuration.Global))
            {
                if (configuration.RegularOutput)
                {
                    Logger.Information("{0} = {1} - {2}".format_with(setting.Key, setting.Value, setting.Description));
                }
                else
                {
                    Logger.Information("{0}|{1}|{2}".format_with(setting.Key, setting.Value, setting.Description));
                }
            }
        }

        public void ToggleFeature(ChocolateyGuiConfiguration configuration, bool requiredValue)
        {
            if (configuration.Global && !Hacks.IsElevated)
            {
                // This is not allowed!
                Logger.Error(L(nameof(Resources.FeatureCommand_ElevatedPermissionsError)));
                return;
            }

            var chosenAppConfiguration = GetChosenAppConfiguration(configuration.Global);
            var featureProperty = GetProperty(configuration.FeatureCommand.Name, true);
            var featureValue = GetPropertyValue<bool?>(chosenAppConfiguration, featureProperty);

            if (featureValue == null || (featureValue.HasValue && requiredValue != featureValue))
            {
                featureProperty.SetValue(chosenAppConfiguration, requiredValue);
                UpdateSettings(chosenAppConfiguration, configuration.Global);

                // since the update happened successfully, update the effective configuration
                featureProperty.SetValue(EffectiveAppConfiguration, requiredValue);

                Logger.Warning(L(
                    requiredValue
                        ? nameof(Resources.FeatureCommand_EnabledWarning)
                        : nameof(Resources.FeatureCommand_DisabledWarning),
                    configuration.FeatureCommand.Name));
            }
            else
            {
                Logger.Warning(L(nameof(Resources.FeatureCommand_NoChangeMessage)));
            }
        }

        public void GetConfigValue(ChocolateyGuiConfiguration configuration)
        {
            var chosenAppConfiguration = GetChosenAppConfiguration(configuration.Global);
            var configProperty = GetProperty(configuration.ConfigCommand.Name, false);
            var configValue = (string)configProperty.GetValue(chosenAppConfiguration);

            Logger.Information("{0}".format_with(configValue ?? string.Empty));
        }

        public void SetConfigValue(string key, string value)
        {
            var configuration = new ChocolateyGuiConfiguration
            {
                CommandName = "config",
                ConfigCommand =
                {
                    Name = key,
                    ConfigValue = value
                }
            };

            SetConfigValue(configuration);
        }

        public void SetConfigValue(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.Global && !Hacks.IsElevated)
            {
                // This is not allowed!
                Logger.Error(L(nameof(Resources.ConfigCommand_ElevatedPermissionsError)));
                return;
            }

            var chosenAppConfiguration = GetChosenAppConfiguration(configuration.Global);
            var configProperty = GetProperty(configuration.ConfigCommand.Name, false);
            configProperty.SetValue(chosenAppConfiguration, configuration.ConfigCommand.ConfigValue);
            UpdateSettings(chosenAppConfiguration, configuration.Global);

            // since the update happened successfully, update the effective configuration
            configProperty.SetValue(EffectiveAppConfiguration, configuration.ConfigCommand.ConfigValue);

            Logger.Warning(L(nameof(Resources.ConfigCommand_Updated), configuration.ConfigCommand.Name, configuration.ConfigCommand.ConfigValue));
        }

        public void UnsetConfigValue(ChocolateyGuiConfiguration configuration)
        {
            if (configuration.Global && !Hacks.IsElevated)
            {
                // This is not allowed!
                Logger.Error(L(nameof(Resources.ConfigCommand_Unset_ElevatedPermissionsError)));
                return;
            }

            var chosenAppConfiguration = GetChosenAppConfiguration(configuration.Global);
            var configProperty = GetProperty(configuration.ConfigCommand.Name, false);
            configProperty.SetValue(chosenAppConfiguration, string.Empty);
            UpdateSettings(chosenAppConfiguration, configuration.Global);

            // since the update happened successfully, update the effective configuration
            configProperty.SetValue(EffectiveAppConfiguration, string.Empty);

            Logger.Warning(L(nameof(Resources.ConfigCommand_Unset), configuration.ConfigCommand.Name));
        }

        private static PropertyInfo GetProperty(string propertyName, bool isFeature)
        {
            var featureProperty = typeof(AppConfiguration).GetProperties().FirstOrDefault(f => f.Name.ToLowerInvariant() == propertyName.ToLowerInvariant());

            if (featureProperty == null)
            {
                var key = isFeature
                    ? nameof(Resources.FeatureCommand_FeatureNotFoundError)
                    : nameof(Resources.ConfigCommand_ConfigNotFoundError);

                Logger.Error(L(key, propertyName));
                Environment.Exit(-1);
            }

            return featureProperty;
        }

        private static T GetPropertyValue<T>(AppConfiguration configuration, PropertyInfo property)
        {
            var propertyValue = (T)property.GetValue(configuration);
            return propertyValue;
        }

        private static string L(string key)
        {
            return TranslationSource[key];
        }

        private static string L(string key, params object[] parameters)
        {
            return TranslationSource[key, parameters];
        }

        private string GetDescriptionFromProperty(PropertyInfo property, bool useResourceKey = false)
        {
            var attributes = property.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), true);
            var attribute = attributes.Length > 0 ? (LocalizedDescriptionAttribute)attributes[0] : null;
            return useResourceKey ? attribute?.Key : attribute?.Description;
        }

        private AppConfiguration GetChosenAppConfiguration(bool global)
        {
            return global ? GlobalAppConfiguration : UserAppConfiguration;
        }
    }
}