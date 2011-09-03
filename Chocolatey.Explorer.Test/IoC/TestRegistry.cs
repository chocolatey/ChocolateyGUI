using Chocolatey.Explorer.IoC;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Services;
using Chocolatey.Explorer.View;
using NUnit.Framework;
using StructureMap;

namespace Chocolatey.Explorer.Test.IoC
{
    [TestFixture]
    public class TestRegistry
    {

        [SetUp]
        public void Setup()
        {
            ObjectFactory.Configure(configure => configure.AddRegistry<Registry>());
        }

        [Test]
        public void IfIPackageManagerCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageManager>());
        }

        [Test]
        public void IfIPackageServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageService>());
        }

        [Test]
        public void IfIPackagesServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackagesService>());
        }

        [Test]
        public void IfIPackageVersionServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageVersionService>());
        }

        [Test]
        public void IfIChocolateyServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IChocolateyService>());
        }

        [Test]
        public void IfIRunCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IRun>());
        }

        [Test]
        public void IfIRunNamedSyncCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetNamedInstance<IRun>("sync"));
        }
         
    }
}