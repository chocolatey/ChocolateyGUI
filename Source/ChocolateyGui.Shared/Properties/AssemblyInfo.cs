// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AssemblyInfo.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

[assembly: AssemblyTitle("ChocolateyGui.Shared")]
[assembly: AssemblyDescription("Contains shared code used by ChocolateyGUI.")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]