// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ConfigFileWatcher.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using Caliburn.Micro;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.services;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Services;
using IFileSystem = chocolatey.infrastructure.filesystem.IFileSystem;

namespace ChocolateyGui.Common.Windows.Services
{
    public class ConfigFileWatcher : IConfigFileWatcher
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEventAggregator _eventAggregator;
        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly IXmlService _xmlService;
        private readonly string _configFilePath = chocolatey.infrastructure.app.ApplicationParameters.GlobalConfigFileLocation;
        private int _lastKnownConfigFileHash;

        public ConfigFileWatcher(IFileSystem fileSystem, IEventAggregator eventAggregator, IXmlService xmlService)
        {
            _fileSystem = fileSystem;
            _eventAggregator = eventAggregator;
            _xmlService = xmlService;
            _fileSystemWatcher = new FileSystemWatcher
            {
                Path = _fileSystem.get_directory_name(_configFilePath),
                Filter = _fileSystem.get_file_name(_configFilePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
            };
            _fileSystemWatcher.Changed += OnConfigFileChanged;
            _fileSystemWatcher.EnableRaisingEvents = true;
            _lastKnownConfigFileHash = _xmlService.deserialize<ConfigFileSettings>(_configFilePath).GetHashCode();
        }

        public void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            var currentSettingsHash = _xmlService.deserialize<ConfigFileSettings>(_configFilePath).GetHashCode();
            if (currentSettingsHash != _lastKnownConfigFileHash)
            {
                _eventAggregator.PublishOnUIThread(new SourcesUpdatedMessage());
                _lastKnownConfigFileHash = currentSettingsHash;
            }
        }
    }
}