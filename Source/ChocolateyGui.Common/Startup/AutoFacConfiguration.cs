// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AutoFacConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
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
        public static IContainer RegisterAutoFac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(System.Reflection.Assembly.GetEntryAssembly());

            var license = License.validate_license();
            if (license.IsValid)
            {
                if (File.Exists(ApplicationParameters.LicensedGuiAssemblyLocation))
                {
                    var licensedGuiAssembly = AssemblyResolution.resolve_or_load_assembly(
                        ApplicationParameters.LicensedChocolateyGuiAssemblySimpleName,
                        chocolatey.infrastructure.app.ApplicationParameters.OfficialChocolateyPublicKey,
                        ApplicationParameters.LicensedGuiAssemblyLocation);

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