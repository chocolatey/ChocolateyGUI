using System.Configuration;

namespace Chocolatey.Gui
{
    public class ChocoConfigurationSection : ConfigurationSection 
    {
        [ConfigurationProperty("packageSources", IsRequired = true, IsDefaultCollection = true)]
        public SourcesCollection Sources
        {
            get { return (SourcesCollection)this["packageSources"]; }
            set { this["packageSources"] = value; }
        }

        [ConfigurationProperty("currentSource", IsRequired = true)]
        public CurrentSourceElement CurrentSource
        {
            get { return (CurrentSourceElement) this["currentSource"]; }
            set { this["currentSource"] = value; }
        }
    }

    public class SourcesCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new SourceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SourceElement) element).Name;
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
}
