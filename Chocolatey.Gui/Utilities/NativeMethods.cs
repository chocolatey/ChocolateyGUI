using System.Runtime.InteropServices;
using System.Text;

namespace Chocolatey.Gui.Utilities
{
    internal class NativeMethods
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        internal static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer, int bufferSize);
    }
}
