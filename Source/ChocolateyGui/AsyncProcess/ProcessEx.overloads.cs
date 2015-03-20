// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ProcessEx.overloads.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.AsyncProcess
{
    using System.Diagnostics;
    using System.Security;
    using System.Threading.Tasks;
  
    /// <summary>
    /// Overloads for ProcessEx
    /// </summary>
    /// <remarks>
    /// these overloads match the ones in Process.Start to make it a simpler transition for callers
    /// see http://msdn.microsoft.com/en-us/library/system.diagnostics.process.start.aspx
    /// </remarks>
    public partial class ProcessEx
    {
        public static Task<ProcessResults> RunAsync(string fileName)
        {
            return RunAsync(new ProcessStartInfo(fileName));
        }

        public static Task<ProcessResults> RunAsync(string fileName, string arguments)
        {
            return RunAsync(new ProcessStartInfo(fileName, arguments));
        }

        public static Task<ProcessResults> RunAsync(string fileName, string userName, SecureString password, string domain)
        {
            return RunAsync(new ProcessStartInfo(fileName)
            {
                UserName = userName,
                Password = password,
                Domain = domain,
                UseShellExecute = false
            });
        }

        public static Task<ProcessResults> RunAsync(string fileName, string arguments, string userName, SecureString password, string domain)
        {
            return RunAsync(new ProcessStartInfo(fileName, arguments)
            {
                UserName = userName,
                Password = password,
                Domain = domain,
                UseShellExecute = false
            });
        }
    }
}