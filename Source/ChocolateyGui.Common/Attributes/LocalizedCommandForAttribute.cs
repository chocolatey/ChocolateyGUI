// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizedCommandForAttribute.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using chocolatey.infrastructure.app.attributes;
using ChocolateyGui.Common.Properties;

namespace ChocolateyGui.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LocalizedCommandForAttribute : CommandForAttribute
    {
        public LocalizedCommandForAttribute(string commandName, string key)
        : base(commandName, Localize(key))
        {
        }

        private static string Localize(string key)
        {
            return Resources.ResourceManager.GetString(key);
        }
    }
}