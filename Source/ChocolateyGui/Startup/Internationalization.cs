// <copyright file="Internationalization.cs" company="Chocolatey">
// Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ChocolateyGui.Utilities;

namespace ChocolateyGui.Startup
{
    public static class Internationalization
    {
        public static void Initialize()
        {
            TranslationSource.Instance.CurrentCulture = CultureInfo.CurrentCulture;
        }

        public static void UpdateLanguage(string languageCode)
        {
            TranslationSource.Instance.CurrentCulture = CultureInfo.GetCultureInfo(languageCode);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = TranslationSource.Instance.CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
        }
    }
}