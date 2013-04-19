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
                this.Log().Debug("Returing lib: {0}", _settings.ChocolateyLibDirectory);
                return _settings.ChocolateyLibDirectory;
            }
            set 
            {
                this.Log().Debug("Saving lib: {0}", value);
                _settings.ChocolateyLibDirectory = value;
                _settings.Save();
            }
        }
    }
}