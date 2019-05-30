// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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
using LiteDB;
using Serilog;

namespace ChocolateyGui.Common.Services
{
    public class ConfigService : IConfigService
    {
        private static readonly ILogger Logger = Serilog.Log.ForContext<ConfigService>();
        private readonly LiteDatabase _database;
        private AppConfiguration _appConfiguration;

        public ConfigService(LiteDatabase database)
        {
            _database = database;
            var settings = _database.GetCollection<AppConfiguration>(nameof(AppConfiguration));
            _appConfiguration = settings.FindById("Default") ?? new AppConfiguration() { Id = "Default", OutputPackagesCacheDurationInMinutes = "60" };
        }

        public event EventHandler SettingsChanged;

        public AppConfiguration GetAppConfiguration()
        {
            return _appConfiguration;
        }

        public void UpdateSettings(AppConfiguration settings)
        {
            var settingsCollection = _database.GetCollection<AppConfiguration>(nameof(AppConfiguration));
            if (settingsCollection.Exists(Query.EQ("_id", "Default")))
            {
                settingsCollection.Update("Default", settings);
            }
            else
            {
                settingsCollection.Insert(settings);
            }

            SettingsChanged?.Invoke(settings, EventArgs.Empty);
        }

        public IEnumerable<ChocolateyGuiFeature> GetFeatures()
        {
            var features = new List<ChocolateyGuiFeature>();

            var properties = typeof(AppConfiguration).GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;

                var featureAttributes = property.GetCustomAttributes(typeof(FeatureAttribute), true);
                if (property.Name != "Id" && featureAttributes.Length > 0)
                {
                    var propertyValue = (bool)property.GetValue(_appConfiguration);

                    features.Add(new ChocolateyGuiFeature { Description = GetDescriptionFromProperty(property), Enabled = propertyValue, Title = propertyName });
                }
            }

            return features;
        }

        public void ListFeatures(ChocolateyGuiConfiguration configuration)
        {
            foreach (var feature in GetFeatures())
            {
                if (configuration.RegularOutput)
                {
                    Logger.Information("{0} {1} - {2}".format_with(feature.Enabled ? "[x]" : "[ ]", feature.Title, feature.Description));
                }
                else
                {
                    Logger.Information("{0}|{1}|{2}".format_with(feature.Title, !feature.Enabled ? Resources.FeatureCommand_Disabled : Resources.FeatureCommand_Enabled, feature.Description));
                }
            }
        }

        public IEnumerable<ChocolateyGuiSetting> GetSettings()
        {
            var settings = new List<ChocolateyGuiSetting>();

            var properties = typeof(AppConfiguration).GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Name;

                var configAttributes = property.GetCustomAttributes(typeof(ConfigAttribute), true);
                if (property.Name != "Id" && configAttributes.Length > 0)
                {
                    var propertyValue = (string)property.GetValue(_appConfiguration);

                    settings.Add(new ChocolateyGuiSetting { Description = GetDescriptionFromProperty(property), Value = propertyValue, Key = propertyName });
                }
            }

            return settings;
        }

        public void ListSettings(ChocolateyGuiConfiguration configuration)
        {
            Logger.Warning(Resources.Command_SettingsTitle);
            Logger.Information(string.Empty);

            foreach (var setting in GetSettings())
            {
                Logger.Information("{0} = {1} | {2}".format_with(setting.Key, setting.Value, setting.Description));
            }

            Logger.Information(string.Empty);
            Logger.Warning(Resources.Command_FeaturesTitle);
            Logger.Information(string.Empty);
            ListFeatures(configuration);
            Logger.Information(string.Empty);
            Logger.Information(Resources.Command_UseFeatureCommandNote.format_with("chocolateyguicli feature"));
        }

        public void EnableFeature(ChocolateyGuiConfiguration configuration)
        {
            var featureProperty = GetProperty(configuration.FeatureCommand.Name, true);
            var featureValue = GetPropertyValue<bool>(_appConfiguration, featureProperty);

            if (!featureValue)
            {
                featureProperty.SetValue(_appConfiguration, true);
                UpdateSettings(_appConfiguration);
                Logger.Warning(Resources.FeatureCommand_EnabledWarning.format_with(configuration.FeatureCommand.Name));
            }
            else
            {
                Logger.Warning(Resources.FeatureCommand_NoChangeMessage);
            }
        }

        public void DisableFeature(ChocolateyGuiConfiguration configuration)
        {
            var featureProperty = GetProperty(configuration.FeatureCommand.Name, true);
            var featureValue = GetPropertyValue<bool>(_appConfiguration, featureProperty);

            if (featureValue)
            {
                featureProperty.SetValue(_appConfiguration, false);
                UpdateSettings(_appConfiguration);
                Logger.Warning(Resources.FeatureCommand_DisabledWarning.format_with(configuration.FeatureCommand.Name));
            }
            else
            {
                Logger.Warning(Resources.FeatureCommand_NoChangeMessage);
            }
        }

        public void GetConfigValue(ChocolateyGuiConfiguration configuration)
        {
            var configProperty = GetProperty(configuration.ConfigCommand.Name, false);
            var configValue = (string)configProperty.GetValue(_appConfiguration);

            Logger.Information("{0}".format_with(configValue ?? string.Empty));
        }

        public void SetConfigValue(ChocolateyGuiConfiguration configuration)
        {
            var configProperty = GetProperty(configuration.ConfigCommand.Name, false);
            configProperty.SetValue(_appConfiguration, configuration.ConfigCommand.ConfigValue);
            UpdateSettings(_appConfiguration);

            Logger.Information(Resources.ConfigCommand_Updated.format_with(configuration.ConfigCommand.Name, configuration.ConfigCommand.ConfigValue));
        }

        public void UnsetConfigValue(ChocolateyGuiConfiguration configuration)
        {
            var configProperty = GetProperty(configuration.ConfigCommand.Name, false);
            configProperty.SetValue(_appConfiguration, string.Empty);
            UpdateSettings(_appConfiguration);

            Logger.Information(Resources.ConfigCommand_Unset.format_with(configuration.ConfigCommand.Name));
        }

        private static PropertyInfo GetProperty(string propertyName, bool isFeature)
        {
            var featureProperty = typeof(AppConfiguration).GetProperties().FirstOrDefault(f => f.Name.ToLowerInvariant() == propertyName.ToLowerInvariant());

            if (featureProperty == null)
            {
                Logger.Error((isFeature ? Resources.FeatureCommand_FeatureNotFoundError : Resources.ConfigCommand_ConfigNotFoundError).format_with(propertyName));
                Environment.Exit(-1);
            }

            return featureProperty;
        }

        private static T GetPropertyValue<T>(AppConfiguration configuration, PropertyInfo property)
        {
            var propertyValue = (T)property.GetValue(configuration);
            return propertyValue;
        }

        private string GetDescriptionFromProperty(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), true);
            var attribute = attributes.Length > 0 ? (LocalizedDescriptionAttribute)attributes[0] : null;
            return attribute?.Description;
        }
    }
}