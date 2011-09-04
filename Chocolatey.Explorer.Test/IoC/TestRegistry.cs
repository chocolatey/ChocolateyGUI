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
        public void IfIPackageManagerIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageManager>(), ObjectFactory.GetInstance<IPackageManager>());
        }

        [Test]
        public void IfIPackageServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageService>());
        }

        [Test]
        public void IfIPackageServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageService>(), ObjectFactory.GetInstance<IPackageService>());
        }

        [Test]
        public void IfIPackagesServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackagesService>());
        }

        [Test]
        public void IfIPackagesServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackagesService>(), ObjectFactory.GetInstance<IPackagesService>());
        }

        [Test]
        public void IfIPackageVersionServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IPackageVersionService>());
        }

        [Test]
        public void IfIPackageVersionServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IPackageVersionService>(), ObjectFactory.GetInstance<IPackageVersionService>());
        }

        [Test]
        public void IfIChocolateyServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IChocolateyService>());
        }

        [Test]
        public void IfIChocolateyServiceIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IChocolateyService>(), ObjectFactory.GetInstance<IChocolateyService>());
        }

        [Test]
        public void IfIRunCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<IRun>());
        }

        [Test]
        public void IfIRunIsNotSingleton()
        {
            Assert.AreNotEqual(ObjectFactory.GetInstance<IRun>(), ObjectFactory.GetInstance<IRun>());
        }

        [Test]
        public void IfIRunNamedSyncCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetNamedInstance<IRun>("sync"));
        }

        [Test]
        public void IfISourceServiceCanBeResolved()
        {
            Assert.IsNotNull(ObjectFactory.GetInstance<ISourceService>());
        }

        [Test]
        public void IfISourceServiceIsSingleton()
        {
            Assert.AreEqual(ObjectFactory.GetInstance<ISourceService>(), ObjectFactory.GetInstance<ISourceService>());
        }
    }
}