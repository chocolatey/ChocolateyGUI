using Chocolatey.Explorer.Services;

namespace Chocolatey.Explorer.Test.Services
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public class PackagesSorterTests
    {
        [Test]
        public void ShouldOrderPackagesWithOnly1NumberCorrectly()
        {
            var packages = new List<String>() { "Choco.0.1.5", "Choco.0.1.4", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.1", packages[0]);
            Assert.AreEqual("Choco.0.1.4", packages[1]);
            Assert.AreEqual("Choco.0.1.5", packages[2]);
            Assert.AreEqual("Choco.0.1.6", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWithOnlyWhereTwoPackagesHaveTheSameNumber()
        {
            var packages = new List<String>() { "Choco.0.1.5", "Choco.0.1.5", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.1", packages[0]);
            Assert.AreEqual("Choco.0.1.5", packages[1]);
            Assert.AreEqual("Choco.0.1.5", packages[2]);
            Assert.AreEqual("Choco.0.1.6", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWithOnly1NumberCorrectlyWhereOneIsPre()
        {
            var packages = new List<String>() { "Choco.0.1.5", "Choco.0.1.4-pre", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.1", packages[0]);
            Assert.AreEqual("Choco.0.1.4-pre", packages[1]);
            Assert.AreEqual("Choco.0.1.5", packages[2]);
            Assert.AreEqual("Choco.0.1.6", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWhere1EndsInDoubleDigitsNumberCorrectly()
        {
            var packages = new List<String>() { "Choco.0.1.5", "Choco.0.1.14", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.1", packages[0]);
            Assert.AreEqual("Choco.0.1.5", packages[1]);
            Assert.AreEqual("Choco.0.1.6", packages[2]);
            Assert.AreEqual("Choco.0.1.14", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWhereAllEndInDoubleDigitsNumberCorrectly()
        {
            var packages = new List<String>() { "Choco.0.1.15", "Choco.0.1.14", "Choco.0.1.26", "Choco.0.1.31" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.14", packages[0]);
            Assert.AreEqual("Choco.0.1.15", packages[1]);
            Assert.AreEqual("Choco.0.1.26", packages[2]);
            Assert.AreEqual("Choco.0.1.31", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWhere1HasAMiddleNumberInDoubleDigitsNumberCorrectly()
        {
            var packages = new List<String>() { "Choco.0.1.5", "Choco.0.11.4", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.1", packages[0]);
            Assert.AreEqual("Choco.0.1.5", packages[1]);
            Assert.AreEqual("Choco.0.1.6", packages[2]);
            Assert.AreEqual("Choco.0.11.4", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWhere1HasAFirstNumberInDoubleDigitsNumberCorrectly()
        {
            var packages = new List<String>() { "Choco.0.1.5", "Choco.10.1.4", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.1", packages[0]);
            Assert.AreEqual("Choco.0.1.5", packages[1]);
            Assert.AreEqual("Choco.0.1.6", packages[2]);
            Assert.AreEqual("Choco.10.1.4", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWhere1HasAllNumbersInDoubleDigitsNumberCorrectly()
        {
            var packages = new List<String>() { "Choco.0.1.5", "Choco.10.11.14", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("Choco.0.1.1", packages[0]);
            Assert.AreEqual("Choco.0.1.5", packages[1]);
            Assert.AreEqual("Choco.0.1.6", packages[2]);
            Assert.AreEqual("Choco.10.11.14", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWhenOneIsEmpty()
        {
            var packages = new List<String>() { "Choco.0.1.5", "", "Choco.0.1.6", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("", packages[0]);
            Assert.AreEqual("Choco.0.1.1", packages[1]);
            Assert.AreEqual("Choco.0.1.5", packages[2]);
            Assert.AreEqual("Choco.0.1.6", packages[3]);
        }

        [Test]
        public void ShouldOrderPackagesWhenTwoAreEmpty()
        {
            var packages = new List<String>() { "Choco.0.1.5", "", "", "Choco.0.1.1" };
            packages.Sort(new PackagesSorter());
            Assert.AreEqual("", packages[0]);
            Assert.AreEqual("", packages[1]);
            Assert.AreEqual("Choco.0.1.1", packages[2]);
            Assert.AreEqual("Choco.0.1.5", packages[3]);
        }

    }
}