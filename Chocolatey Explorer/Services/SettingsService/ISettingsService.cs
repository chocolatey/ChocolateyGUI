using System;
using Chocolatey.Explorer.Properties;

namespace Chocolatey.Explorer.Services.SettingsService
{
    public interface ISettingsService
    {
        String ChocolateyLibDirectory { get; set; }
    }
}