using Chocolatey.Explorer.Model;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Model
{   
    [TestFixture]
    public class TestPackage
    {
        [Test]
        public void IfToStringReturnsName()
        {
            var package = new Package {Name = "test"};
            Assert.AreEqual("test",package.ToString());
        }

        [Test]
        public void IfIsInstalledIsTrueWhenThereIsAVersion()
        {
            var package = new Package { InstalledVersion = "0.1.1" };
            Assert.IsTrue(package.IsInstalled);
        }

        [Test]
        public void IfIsInstalledIsFalseWhenVersionIsNotAvailable()
        {
            var package = new Package { InstalledVersion = strings.not_available };
            Assert.IsFalse(package.IsInstalled);
        }

        [Test]
        public void IfTwoClassesWithTheSameNameAreEqual()
        {
            var package1 = new Package();
            var package2 = new Package();
            package1.Name = "test";
            package2.Name = "test";
            Assert.IsTrue(package1.Equals(package2));
        }

        [Test]
        public void IfTwoClassesWithDifferentNamesAreNotEqual()
        {
            var package1 = new Package();
            var package2 = new Package();
            package1.Name = "test";
            package2.Name = "test2";
            Assert.IsFalse(package1.Equals(package2));
        }
    }
}
