// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignMode.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows;

namespace ChocolateyGui
{
    internal static class DesignMode
    {
        private static bool? isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!isInDesignMode.HasValue)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    isInDesignMode
                        = (bool)DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(FrameworkElement))
                        .Metadata.DefaultValue;
                }

                return isInDesignMode.Value;
            }
        }
    }
}