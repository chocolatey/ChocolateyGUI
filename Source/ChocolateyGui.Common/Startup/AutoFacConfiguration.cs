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
        public static IContainer RegisterAutoFac(string chocolateyGuiAssemblySimpleName, string licensedGuiAssemblyLocation)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(System.Reflection.Assembly.GetCallingAssembly());

            var license = License.validate_license();
            if (license.IsValid)
            {
                if (File.Exists(licensedGuiAssemblyLocation))
                {
                    var licensedGuiAssembly = AssemblyResolution.resolve_or_load_assembly(
                        chocolateyGuiAssemblySimpleName,
                        chocolatey.infrastructure.app.ApplicationParameters.OfficialChocolateyPublicKey,
                        licensedGuiAssemblyLocation);

                    if (licensedGuiAssembly != null)
                    {
                        license.AssemblyLoaded = true;
                        license.Assembly = licensedGuiAssembly;
                        license.Version = VersionInformation.get_current_informational_version(licensedGuiAssembly);

                        builder.RegisterAssemblyModules(licensedGuiAssembly.UnderlyingType);
                    }
                }
            }

            return builder.Build();
        }
    }
}