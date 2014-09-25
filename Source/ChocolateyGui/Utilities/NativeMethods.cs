// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NativeMethods.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities
{
    using System.Runtime.InteropServices;
    using System.Text;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "PIvoke magic is happening here...")]
    internal class NativeMethods
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        internal static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer, int bufferSize);
    }
}