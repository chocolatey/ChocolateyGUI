// <copyright file="AssemblyResolver.cs" company="Chocolatey">
// Copyright 2017 - Present Chocolatey Software, LLC
// Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Serilog;

namespace ChocolateyGui.Common.Startup
{
    public class AssemblyResolver
    {
        private const int LOCKRESOLUTIONTIMEOUTSECONDS = 5;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Resolves or loads an assembly. If an assembly is already loaded, no need to reload it.
        /// </summary>
        /// <param name="assemblySimpleName">Simple Name of the assembly, such as "chocolatey"</param>
        /// <param name="publicKeyToken">The public key token.</param>
        /// <param name="assemblyFileLocation">The assembly file location. Typically the path to the DLL on disk.</param>
        /// <returns>An assembly</returns>
        /// <exception cref="Exception">Unable to enter synchronized code to determine assembly loading</exception>
        public static Assembly ResolveOrLoadAssembly(string assemblySimpleName, string publicKeyToken, string assemblyFileLocation)
        {
            return ResolveOrLoadAssemblyInternal(
                assemblySimpleName,
                publicKeyToken,
                assemblyFileLocation,
                (assembly) => string.Equals(assembly.GetName().Name, assemblySimpleName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool DoesPublicKeyTokenMatch(AssemblyName assemblyName, string expectedKeyToken)
        {
            var publicKey = GetPublicKeyToken(assemblyName);

            return string.Equals(publicKey, expectedKeyToken, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetPublicKeyToken(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                return string.Empty;
            }

            var publicKeyToken = assemblyName.GetPublicKeyToken();

            if (publicKeyToken == null || publicKeyToken.Length == 0)
            {
                return string.Empty;
            }

            return publicKeyToken.Select(x => x.ToString("x2")).Aggregate((x, y) => x + y);
        }

        private static Assembly ResolveOrLoadAssemblyInternal(string assemblySimpleName, string publicKeyToken, string assemblyFileLocation, Func<Assembly, bool> assemblyPredicate)
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(_lockObject, TimeSpan.FromSeconds(LOCKRESOLUTIONTIMEOUTSECONDS), ref lockTaken);
            }
            catch (Exception)
            {
                throw new Exception("Unable to enter synchronized code to determine assembly loading");
            }

            Assembly resolvedAssembly = null;
            if (lockTaken)
            {
                try
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assemblyPredicate))
                    {
                        if (string.IsNullOrWhiteSpace(publicKeyToken) || string.Equals(GetPublicKeyToken(assembly.GetName()), publicKeyToken, StringComparison.OrdinalIgnoreCase))
                        {
                            Log.Debug("Returning loaded assembly type for '{0}'", assemblySimpleName);

                            resolvedAssembly = assembly;
                            break;
                        }
                    }

                    if (resolvedAssembly == null)
                    {
                        Log.Debug("Loading up '{0}' assembly type from '{1}'", assemblySimpleName, assemblyFileLocation);

                        // Reading the raw bytes and calling 'Load' causes an exception, as such we use LoadFrom instead.
                        resolvedAssembly = Assembly.LoadFrom(assemblyFileLocation);
                    }
                }
                finally
                {
                    Monitor.Pulse(_lockObject);
                    Monitor.Exit(_lockObject);
                }
            }

            return resolvedAssembly;
        }
    }
}