using System;
using Chocolatey.Explorer.Model;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Model
{
    [TestFixture]
    public class TestSource
    {
         
        [Test]
        public void IfSourcesAreEqualWhenNameIsEqual()
        {
            var source1 = new Source();
            var source2 = new Source();
            source1.Name = "test";
            source2.Name = "test";
            Assert.IsTrue(source1.Equals(source2));
        }

        [Test]
        public void IfSourcesAreNotEqualWhenNamesAreNotTheSame()
        {
            var source1 = new Source();
            var source2 = new Source();
            source1.Name = "test";
            source2.Name = "test2";
            Assert.IsFalse(source1.Equals(source2));
        }

        [Test]
        public void IfTwoClassesWithOneClassNotASourceAreNotEqual()
        {
            var source1 = new Source();
            var source2 = new Object();
            source1.Name = "test";
            Assert.IsFalse(source1.Equals(source2));
        }

        [Test]
        public void GivesHashCodeOfName()
        {
            var source1 = new Source();
            source1.Name = "test2";
            Assert.AreEqual("test2".GetHashCode(), source1.GetHashCode());
        }

        [Test]
        public void IfToStringGivesCorrectString()
        {
            var source1 = new Source();
            source1.Name = "testName";
            source1.Url = "testUrl";
            Assert.AreEqual("testName (testUrl)", source1.ToString());
        }
    }
}