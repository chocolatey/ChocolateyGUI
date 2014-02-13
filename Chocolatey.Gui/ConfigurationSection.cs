using System;
using System.Collections.Generic;
using System.Configuration;

namespace Chocolatey.Gui
{
    public class ChocoConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "chocolatey";

        public static ChocoConfigurationSection Current
        {
            get { return (ChocoConfigurationSection)ConfigurationManager.GetSection(SectionName); }
        }

        [ConfigurationProperty("packageSources", IsRequired = true, IsDefaultCollection = true)]
        public SourcesCollection Sources
        {
            get { return (SourcesCollection)this["packageSources"]; }
            set { this["packageSources"] = value; }
        }

        [ConfigurationProperty("currentSource", IsRequired = false)]
        public CurrentSourceElement CurrentSource
        {
            get { return (CurrentSourceElement) this["currentSource"]; }
        }

        [ConfigurationProperty("chocolateyInstall", IsRequired = true)]
        public ChocolateyInstall ChocolateyInstall
        {
            get { return (ChocolateyInstall)this["chocolateyInstall"]; }
            set { this["chocolateyInstall"] = value; }
        }
    }

    public class SourcesCollection : ConfigurationElementCollection, IEnumerable<SourceElement>
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new SourceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SourceElement) element).Name;
        }
        public new IEnumerator<SourceElement> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return BaseGet(i) as SourceElement;
        }
}

    public class SourceElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["Name"]; }
            set { base["Name"] = value; }
        }

        [ConfigurationProperty("Url", IsKey = true, IsRequired = true)]
        public string Url
        {
            get { return (string)base["Url"]; }
            set { base["Url"] = value; }
        }
    }

    public class CurrentSourceElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["Name"]; }
            set { base["Name"] = value; }
        }
    }

    public class ChocolateyInstall : ConfigurationElement
    {
        public override bool IsReadOnly()
        {
            return false;
        }

        [ConfigurationProperty("Path", DefaultValue = "")]
        public string Path
        {
            get { return (string)base["Path"]; }
            set { base["Path"] = value; }
        }
    }
}
