// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AssemblyInfo.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle("Chocolatey GUI")]
[assembly: AssemblyDescription("GUI for Chocolatey")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: CLSCompliant(false)]

[assembly: ComVisible(false)]
[assembly: ThemeInfo(

    // where theme specific resource dictionaries are located
    // (used if a resource is not found in the page,
    // or application resource dictionaries)
#pragma warning disable SA1114 // Parameter list must follow declaration
    ResourceDictionaryLocation.None,
#pragma warning restore SA1114 // Parameter list must follow declaration

    // where the generic resource dictionary is located
    // (used if a resource is not found in the page,
    // app, or any theme specific resource dictionaries)
#pragma warning disable SA1115 // Parameter must follow comma
    ResourceDictionaryLocation.SourceAssembly)]
#pragma warning restore SA1115 // Parameter must follow comma