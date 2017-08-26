// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AssemblyInfo.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("ChocolateyGui.Subprocess")]
[assembly: AssemblyDescription("Subprocess for handling delegated Chocolatey commands.")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("20510788-dd8d-42e0-a2d9-430c6c2ced44")]