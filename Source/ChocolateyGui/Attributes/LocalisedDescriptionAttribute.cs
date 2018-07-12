using System.ComponentModel;
using ChocolateyGui.Properties;

namespace ChocolateyGui.Attributes
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        static string Localize(string key)
        {
            return Resources.ResourceManager.GetString(key);
        }

        public LocalizedDescriptionAttribute(string key)
            : base(Localize(key))
        {
        }
    }
}