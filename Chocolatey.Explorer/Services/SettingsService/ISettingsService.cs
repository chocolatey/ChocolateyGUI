using System;

namespace Chocolatey.Explorer.Services.SettingsService
{
    public interface ISettingsService
    {
        String ChocolateyLibDirectory { get; set; }
    }
}