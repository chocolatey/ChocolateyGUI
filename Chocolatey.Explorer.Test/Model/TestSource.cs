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
    }
}