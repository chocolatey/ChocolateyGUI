// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Internationalization.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ChocolateyGui.Common.Utilities;

namespace ChocolateyGui.Common.Windows.Startup
{
    public static class Internationalization
    {
        private static readonly HashSet<CultureInfo> _cachedCultures = new HashSet<CultureInfo>();
        private static readonly Lazy<CultureInfo> _fallbackCulture = new Lazy<CultureInfo>(() => new CultureInfo("en"));

        public static event EventHandler<CultureInfo> LanguageChanged;

        public static IEnumerable<CultureInfo> GetAllSupportedCultures()
        {
            if (_cachedCultures.Count > 0)
            {
                return _cachedCultures;
            }

            _cachedCultures.Add(GetFallbackCulture());

            var installLocation = Bootstrapper.ApplicationFilesPath;

            foreach (var directory in Directory.EnumerateDirectories(installLocation))
            {
                var resourceAssemblyPath = Path.Combine(directory, "ChocolateyGui.Common.resources.dll");
                if (!File.Exists(resourceAssemblyPath))
                {
                    continue;
                }

                try
                {
                    var directoryName = Path.GetFileName(directory);

                    var culture = new CultureInfo(directoryName);

                    if (!_cachedCultures.Contains(culture))
                    {
                        _cachedCultures.Add(culture);
                    }
                }
                catch (Exception)
                {
                    // Ignored on purpose.
                }
            }

            return _cachedCultures;
        }

        public static CultureInfo GetFallbackCulture()
        {
            return _fallbackCulture.Value;
        }

        public static CultureInfo GetSupportedCultureInfo(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                return GetFallbackCulture();
            }

            var cultureInfo = new CultureInfo(languageCode);

            var foundCulture = GetAllSupportedCultures().FirstOrDefault(c =>
                string.Equals(c.Name, cultureInfo.Name, StringComparison.OrdinalIgnoreCase));

            if (foundCulture != null)
            {
                return foundCulture;
            }

            if (!string.IsNullOrEmpty(cultureInfo.Parent.Name))
            {
                return GetSupportedCultureInfo(cultureInfo.Parent.Name);
            }

            return GetFallbackCulture();
        }

        public static void Initialize()
        {
            UpdateLanguage(CultureInfo.CurrentCulture.Name);
        }

        public static void UpdateLanguage(string languageCode)
        {
            var existingLanguage = TranslationSource.Instance.CurrentCulture ?? CultureInfo.CurrentCulture;

            var culture = GetSupportedCultureInfo(languageCode);

            if (culture != existingLanguage)
            {
                TranslationSource.Instance.CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;

                LanguageChanged?.Invoke(typeof(Internationalization), culture);
            }
        }
    }
}