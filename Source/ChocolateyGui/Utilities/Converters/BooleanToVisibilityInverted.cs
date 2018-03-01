﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityInverted.cs" company="Chocolatey">
// Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChocolateyGui.Utilities.Converters
{
    public class BooleanToVisibilityInverted : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IsCollapsed(value, parameter)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static bool IsCollapsed(object value, object parameter)
        {
            var boolVal = (value != null && value != DependencyProperty.UnsetValue) && (bool)value;
            return boolVal == false ^ (parameter != null && bool.Parse((string)parameter));
        }
    }
}