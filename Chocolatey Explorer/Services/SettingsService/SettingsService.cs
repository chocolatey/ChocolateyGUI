using System;
using Chocolatey.Explorer.Properties;

namespace Chocolatey.Explorer.Services.SettingsService
{
    public class SettingsService:ISettingsService
    {
        private readonly Settings _settings;

        public SettingsService()
        {
            _settings = new Settings();
        }

        public String ChocolateyLibDirectory
        {
            get
            {
                return _settings.ChocolateyLibDirectory;
            }
            set 
            {
                _settings.ChocolateyLibDirectory = value;
                _settings.Save();
            }
        }
    }
}