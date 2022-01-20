// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizedCommandForAttribute.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using chocolatey.infrastructure.app.attributes;
using ChocolateyGui.Common.Utilities;

namespace ChocolateyGui.Common.Attributes
{
    public class LocalizedCommandForAttribute : CommandForAttribute
    {
        public string Key { get; set; }

        public LocalizedCommandForAttribute(string commandName, string key)
        : base(commandName, Localize(key))
        {
        }

        private static string Localize(string key)
        {
            return TranslationSource.Instance[key];
        }
    }
}