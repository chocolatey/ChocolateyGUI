// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NativeMethods.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ChocolateyGui.Utilities
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "PIvoke magic is happening here...")]
    internal class NativeMethods
    {
        public static float GetScaleFactor()
        {
            var desktop = GetDCCE(IntPtr.Zero);
            int Xdpi = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSX);
            int Ydpi = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, desktop);
            return Xdpi / 96f;
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        internal static extern long StrFormatByteSize(long fileSize,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer, int bufferSize);

        [DllImport("User32.dll", EntryPoint = "GetDC", SetLastError = true)]
        private static extern IntPtr GetDCCE(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ReleaseDC", SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        private enum DeviceCap
        {
            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,

            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90

            // Other constants may be founded on pinvoke.net
        }
    }
}