// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="Windows7PlatformProvider.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;

namespace ChocolateyGui.Common.Windows.Providers.PlatformProvider
{
    public class Windows7PlatformProvider : IPlatformProvider
    {
        public Tuple<float, float> GetDpiScaleFactor()
        {
            float dpiX, dpiY;
            var hwnd = IntPtr.Zero;
            if (Application.Current?.MainWindow != null)
            {
                hwnd = new WindowInteropHelper(Application.Current.MainWindow).EnsureHandle();
            }

            using (var graphics = Graphics.FromHwnd(hwnd))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            return Tuple.Create(dpiX / 96.0f, dpiY / 96.0f);
        }
    }
}