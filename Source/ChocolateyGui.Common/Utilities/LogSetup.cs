// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogSetup.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;

namespace ChocolateyGui.Common.Utilities
{
    public static class LogSetup
    {
        private static string _localAppDataPath;
        private static string _logsFolderPath;
        private static string _appDataPath;

        public static void Execute()
        {
            if (!Directory.Exists(_localAppDataPath))
            {
                Directory.CreateDirectory(_localAppDataPath);
            }

            if (!Directory.Exists(_logsFolderPath))
            {
                Directory.CreateDirectory(_logsFolderPath);
            }
        }

        public static string GetLogsFolderPath(string folderName)
        {
            _logsFolderPath = Path.Combine(_appDataPath, folderName);

            return _logsFolderPath;
        }

        public static string GetLocalAppDataPath(string applicationName)
        {
            _localAppDataPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                applicationName);

            return _localAppDataPath;
        }

        public static string GetAppDataPath(string applicationName)
        {
            _appDataPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData,
                    Environment.SpecialFolderOption.DoNotVerify),
                applicationName);

            return _appDataPath;
        }
    }
}