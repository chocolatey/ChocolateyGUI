using Chocolatey.Explorer.Model;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Model
{
    [TestFixture]
    public class TestPackageVersion
    {

        [Test]
        public void IfToStringReturnName()
        {
            var package = new PackageVersion();
            package.Name = "test";
            Assert.AreEqual("test", package.ToString());
        }

        [Test]
        public void IfTwoClassesWithTheSameNameAreEqual()
        {
            var package1 = new PackageVersion();
            var package2 = new PackageVersion();
            package1.Name = "test";
            package2.Name = "test";
            Assert.IsTrue(package1.Equals(package2));
        }

        [Test]
        public void IfTwoClassesWithDifferentNamesAreNotEqual()
        {
            var package1 = new PackageVersion();
            var package2 = new PackageVersion();
            package1.Name = "test";
            package2.Name = "test2";
            Assert.IsFalse(package1.Equals(package2));
        }

        [Test]
        public void IfIsInstalledReturnsTrueWhenCurrentVersionIsNotNoVersion()
        {
            var package = new PackageVersion();
            package.CurrentVersion = "test";
            Assert.IsTrue(package.IsInstalled);
        }

        [Test]
        public void IfIsInstalledReturnsFalseWhenCurrentVersionIsNoVersion()
        {
            var package = new PackageVersion();
            package.CurrentVersion = "no version";
            Assert.IsFalse(package.IsInstalled);
        }

        [Test]
        public void IfCanBeUpdatedIsTrueWhenCurrentVersionNotEqualsServerVersionAndNotIsInstalled()
        {
            var package = new PackageVersion();
            package.CurrentVersion = "0.5.2";
            package.Serverversion = "0.5.4";
            Assert.IsTrue(package.CanBeUpdated);
        }

        [Test]
        public void IfCanBeUpdatedIsFalseWhenCurrentVersionEqualsServerVersionAndNotIsInstalled()
        {
            var package = new PackageVersion();
            package.CurrentVersion = "0.5.2";
            package.Serverversion = "0.5.2";
            Assert.IsFalse(package.CanBeUpdated);
        }

        [Test]
        public void IfCanBeUpdatedIsFalseWhenCurrentVersionNotEqualsServerVersionAndIsInstalled()
        {
            var package = new PackageVersion();
            package.CurrentVersion = "no version";
            package.Serverversion = "0.5.2";
            Assert.IsFalse(package.CanBeUpdated);
        }
    }
}