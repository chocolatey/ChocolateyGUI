// <copyright file="Program.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System;
using System.IO;
using System.Reflection;
using ChocolateyGui.Common.Startup;

namespace ChocolateyGuiCli
{
    public class Program
    {
        private static ResolveEventHandler _handler = null;

        public static void Main(string[] args)
        {
            AddAssemblyResolver();

            Runner.Run(args);
        }

        #region DupFinder Exclusion

        private static void AddAssemblyResolver()
        {
            _handler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);

                try
                {
                    if (string.Equals(requestedAssembly.Name, "chocolatey", StringComparison.OrdinalIgnoreCase))
                    {
                        var installDir = Environment.GetEnvironmentVariable("ChocolateyInstall");
                        if (string.IsNullOrEmpty(installDir))
                        {
                            var rootDrive = Path.GetPathRoot(Assembly.GetExecutingAssembly().Location);
                            if (string.IsNullOrEmpty(rootDrive))
                            {
                                return null; // TODO: Maybe return the chocolatey.dll file instead?
                            }

                            installDir = Path.Combine(rootDrive, "ProgramData", "chocolatey");
                        }

                        var assemblyLocation = Path.Combine(installDir, "choco.exe");

                        return AssemblyResolver.ResolveOrLoadAssembly("choco", string.Empty, assemblyLocation);
                    }

#if FORCE_CHOCOLATEY_OFFICIAL_KEY
                    var chocolateyGuiPublicKey = Bootstrapper.OfficialChocolateyPublicKey;
#else
                    var chocolateyGuiPublicKey = Bootstrapper.UnofficialChocolateyPublicKey;
#endif

                    if (AssemblyResolver.DoesPublicKeyTokenMatch(requestedAssembly, chocolateyGuiPublicKey)
                        && string.Equals(requestedAssembly.Name, Bootstrapper.ChocolateyGuiCommonAssemblySimpleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return AssemblyResolver.ResolveOrLoadAssembly(
                            Bootstrapper.ChocolateyGuiCommonAssemblySimpleName,
                            AssemblyResolver.GetPublicKeyToken(requestedAssembly),
                            Bootstrapper.ChocolateyGuiCommonAssemblyLocation);
                    }

                    if (AssemblyResolver.DoesPublicKeyTokenMatch(requestedAssembly, chocolateyGuiPublicKey)
                        && string.Equals(requestedAssembly.Name, Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return AssemblyResolver.ResolveOrLoadAssembly(
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblySimpleName,
                            AssemblyResolver.GetPublicKeyToken(requestedAssembly),
                            Bootstrapper.ChocolateyGuiCommonWindowsAssemblyLocation);
                    }
                }
                catch (Exception ex)
                {
                    Bootstrapper.Logger.Warning("Unable to load Chocolatey GUI assembly. {0}", ex.Message);
                }

                return null;
            };

            AppDomain.CurrentDomain.AssemblyResolve += _handler;
        }

        #endregion DupFinder Exclusion
    }
}