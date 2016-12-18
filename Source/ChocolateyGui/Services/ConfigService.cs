// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigService.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ChocolateyGui.Models;
using LiteDB;

namespace ChocolateyGui.Services
{
    public class ConfigService : IConfigService
    {
        private readonly LiteDatabase _database;

        public ConfigService(LiteDatabase database)
        {
            _database = database;
        }

        public AppConfiguration GetSettings()
        {
            var settings = _database.GetCollection<AppConfiguration>(nameof(AppConfiguration));
            return settings.FindById("Default") ?? new AppConfiguration() { Id = "Default" };
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
        }
    }
}