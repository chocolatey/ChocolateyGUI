// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="NupkgReader.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities.Nuspec
{
    using System;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using Autofac;
    using ChocolateyGui.Models;
    using ChocolateyGui.ViewModels.Items;

    public static class NupkgReader
    {
        public static async Task<IPackageViewModel> GetPackageInformation(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException(@"Invalid file path.", "filePath");
            }

            using (var package = Package.Open(filePath))
            {
                var nuspecPart = package.GetParts().SingleOrDefault(part => part.Uri.OriginalString.Contains("nuspec"));
                if (nuspecPart == null)
                {
                    throw new InvalidDataException("Package does not contain nuspec");
                }

                using (var partStream = nuspecPart.GetStream())
                {
                    var data = new byte[partStream.Length];
                    await partStream.ReadAsync(data, 0, (int)partStream.Length);

                    using (var partMemoryStream = new MemoryStream(data))
                    {
                        var packageViewModel = App.Container.Resolve<IPackageViewModel>();

                        var serializer = new XmlSerializer(typeof(NuspecPackage));
                        var packageInformation = (NuspecPackage)serializer.Deserialize(new NamespaceIgnorantXmlTextReader(partMemoryStream));

                        AutoMapper.Mapper.Map(packageInformation.Metadata, packageViewModel);
                        return packageViewModel;
                    }
                }
            }
        }

        private class NamespaceIgnorantXmlTextReader : XmlTextReader
        {
            public NamespaceIgnorantXmlTextReader(Stream reader)
                : base(reader)
            {
            }

            public override string NamespaceURI
            {
                get { return string.Empty; }
            }
        }
    }
}