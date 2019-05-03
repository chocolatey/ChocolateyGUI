// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizedCommandForAttribute.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using ChocolateyGui.Properties;

namespace ChocolateyGui.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LocalizedCommandForAttribute : Attribute
    {
        public LocalizedCommandForAttribute(string commandName, string resourceKey)
        {
            CommandName = commandName;
            Description = Resources.ResourceManager.GetString(resourceKey);
        }

        public string CommandName { get; private set; }

        public string Description { get; private set; }
    }
}