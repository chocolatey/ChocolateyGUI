using System;

namespace ChocolateyGui.CliCommands
{
    [Serializable]
    public class ChocolateyGuiConfiguration
    {
        public ChocolateyGuiConfiguration()
        {
            RegularOutput = true;
            FeatureCommand = new FeatureCommandConfiguration();
            ConfigCommand = new ConfigCommandConfiguration();
        }

        public string CommandName { get; set; }

        public bool HelpRequested { get; set; }

        public bool UnsuccessfulParsing { get; set; }

        public bool RegularOutput { get; set; }

        public string Input { get; set; }

        public FeatureCommandConfiguration FeatureCommand { get;  set; }

        public ConfigCommandConfiguration ConfigCommand { get; set; }
    }

    public sealed class FeatureCommandConfiguration
    {
        public string Name { get; set; }

        public FeatureCommandType Command { get; set; }
    }

    [Serializable]
    public sealed class ConfigCommandConfiguration
    {
        public string Name { get; set; }

        public string ConfigValue { get; set; }

        public ConfigCommandType Command { get; set; }
    }
}