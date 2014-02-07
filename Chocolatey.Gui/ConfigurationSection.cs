using System.Configuration;

namespace Chocolatey.Gui
{
    public class ChocoConfigurationSection : ConfigurationSection 
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public SourcesCollection Sources
        {
            get { return (SourcesCollection) this[""]; }
            set { this[""] = value; }
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
}
