using Chocolatey.Explorer.Services;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Services
{
    public class TestDependenciesFormatter
    {
        [Test]
        public void IfSplitsOnPipe()
        {
            var formatter = new DependenciesFormatter();
            var result = formatter.Execute("test1|test2");
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void IfRemovesColonsFromText()
        {
            var formatter = new DependenciesFormatter();
            var result = formatter.Execute("Git.Install::");
            Assert.AreEqual("Git.Install", result[0]);
        }

        [Test]
        public void IfVersionNumberWithoutEndIsReadCorrectlyAndBeginningSquareBracket()
        {
            var formatter = new DependenciesFormatter();
            var result = formatter.Execute("SABnzbd:[0.7.5, ]:");
            Assert.AreEqual("SABnzbd (≥ 0.7.5)", result[0]);
        }

        [Test]
        public void IfVersionNumberWithoutEndIsReadCorrectlyAndBeginningRoundBracket()
        {
            var formatter = new DependenciesFormatter();
            var result = formatter.Execute("SABnzbd:(0.7.5, ]:");
            Assert.AreEqual("SABnzbd (> 0.7.5)", result[0]);
        }

        [Test]
        public void IfVersionNumberReplacesCommaWiiDoubleAmpersand()
        {
            var formatter = new DependenciesFormatter();
            var result = formatter.Execute("python:[2.7, 3.0):");
            Assert.AreEqual("python (≥ 2.7 && < 3.0)", result[0]);
        }

        [Test]
        public void IfSplitDependeciesForCouchPotatoCorrectly()
        {
            var formatter = new DependenciesFormatter();
            var result = formatter.Execute("python:[2.7, 3.0):|Git.Install::|rktools.2003::|PyWin32::|SABnzbd:[0.7.5, ]:");
            Assert.AreEqual(5, result.Length);
        }
        
    }
}