// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Internationalization.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Threading;
using ChocolateyGui.Common.Windows.Utilities;

namespace ChocolateyGui.Common.Windows.Startup
{
    public static class Internationalization
    {
        public static void Initialize()
        {
            TranslationSource.Instance.CurrentCulture = CultureInfo.CurrentCulture;
        }

        public static void UpdateLanguage(string languageCode)
        {
            var culture = CultureInfo.GetCultureInfo(languageCode);
            TranslationSource.Instance.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }
}