using Chocolatey.Explorer.Model;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Model
{   
    [TestFixture]
    public class TestPackage
    {
        [Test]
        public void IfToStringReturnName()
        {
            var package = new Package();
            package.Name = "test";
            Assert.AreEqual("test",package.ToString());
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
