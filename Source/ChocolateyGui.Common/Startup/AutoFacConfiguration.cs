// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AutoFacConfiguration.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac;
using chocolatey.infrastructure.information;
using chocolatey.infrastructure.licensing;
using chocolatey.infrastructure.registration;

namespace ChocolateyGui.Common.Startup
{
    public static class AutoFacConfiguration
    {
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1506:AvoidExcessiveClassCoupling",
            Justification = "This is really a requirement due to required registrations.")]
        public static IContainer RegisterAutoFac(string chocolateyGuiAssemblySimpleName, string licensedGuiAssemblyLocation, string publicKey)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(System.Reflection.Assembly.GetCallingAssembly());

            var license = License.ValidateLicense();
            if (license.IsValid)
            {
                if (File.Exists(licensedGuiAssemblyLocation))
                {
                    var licensedGuiAssembly = AssemblyResolution.ResolveOrLoadAssembly(
                        chocolateyGuiAssemblySimpleName,
                        publicKey,
                        licensedGuiAssemblyLocation);

                    if (licensedGuiAssembly != null)
                    {
                        license.AssemblyLoaded = true;
                        license.Assembly = licensedGuiAssembly;
                        license.Version = VersionInformation.GetCurrentInformationalVersion(licensedGuiAssembly);

                        builder.RegisterAssemblyModules(licensedGuiAssembly.UnderlyingType);
                    }
                }
            }

            return builder.Build();
        }
    }
}