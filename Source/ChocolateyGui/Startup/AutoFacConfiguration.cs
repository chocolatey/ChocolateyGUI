// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AutoFacConfiguration.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace ChocolateyGui.Startup
{
    public static class AutoFacConfiguration
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "This is really a requirement due to required registrations.")]
        public static IContainer RegisterAutoFac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(AutoFacConfiguration).Assembly);
            return builder.Build();
        }
    }
}