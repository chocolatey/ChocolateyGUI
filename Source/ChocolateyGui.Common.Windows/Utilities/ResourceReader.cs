// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ResourceReader.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public static class ResourceReader
    {
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times",
            Justification = "Based on this blog post: http://blogs.msdn.com/b/tilovell/archive/2014/02/12/the-worst-code-analysis-rule-that-s-recommended-ca2202.aspx")]
        internal static string GetFromResources(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return string.Empty;
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}