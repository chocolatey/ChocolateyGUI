using System;
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
            Assert.AreEqual("Name", package.ToString().Substring(0,4));
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
        public void IfTwoClassesWithOneClassNotAPackageVersionAreNotEqual()
        {
            var package1 = new PackageVersion();
            var package2 = new Object();
            package1.Name = "test";
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
            package.CurrentVersion = "n.a.";
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
        public void IfCanBeUpdatedIsFalseWhenCurrentVersionIsHigherThanServerVersionAndNotIsInstalled()
        {
            var package = new PackageVersion();
            package.CurrentVersion = "0.5.5";
            package.Serverversion = "0.5.2";
            Assert.IsFalse(package.CanBeUpdated);
        }

        [Test]
        public void IfCanBeUpdatedIsFalseWhenCurrentVersionNotEqualsServerVersionAndIsInstalled()
        {
            var package = new PackageVersion();
            package.CurrentVersion = "n.a.";
            package.Serverversion = "0.5.2";
            Assert.IsFalse(package.CanBeUpdated);
        }

        [Test]
        public void GivesHashCodeOfName()
        {
            var package1 = new PackageVersion();
            package1.Name = "test2";
            Assert.AreEqual("test2".GetHashCode(), package1.GetHashCode());
        }

        [Test]
        public void IfTwoClassesWithTheSameNameGiveCompareToOf0()
        {
            var package1 = new PackageVersion();
            var package2 = new PackageVersion();
            package1.Name = "test";
            package2.Name = "test";
            Assert.AreEqual(0, package1.CompareTo(package2));
        }

        [Test]
        public void IfTwoClassesWithPackage1IsBeforePackage2GiveCompareToOfMinus1()
        {
            var package1 = new PackageVersion();
            var package2 = new PackageVersion();
            package1.Name = "test1";
            package2.Name = "test2";
            Assert.AreEqual(-1, package1.CompareTo(package2));
        }

        [Test]
        public void IfTwoClassesWithPackage2IsBeforePackage1GiveCompareToOf1()
        {
            var package1 = new PackageVersion();
            var package2 = new PackageVersion();
            package1.Name = "test2";
            package2.Name = "test1";
            Assert.AreEqual(1, package1.CompareTo(package2));
        }
    }
}