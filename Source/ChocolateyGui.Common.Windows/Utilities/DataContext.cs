// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContext.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public static class DataContext
    {
        public static object GetDataContext(object element)
        {
            var fe = element as FrameworkElement;
            if (fe != null)
            {
                return fe.DataContext;
            }

            var fce = element as FrameworkContentElement;
            return fce == null ? null : fce.DataContext;
        }
    }
}