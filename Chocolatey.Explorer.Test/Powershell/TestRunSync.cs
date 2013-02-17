using Chocolatey.Explorer.Powershell;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Powershell
{
    public class TestRunSync
    {
        [Test]
        public void IfCanRunCommand()
        {
            var runSync = new RunSync();
            runSync.Run("write test");
        }

        [Test]
        public void IfCanRunCommandAndReturnOutput()
        {
            var runSync = new RunSync();
            var result = "";
            runSync.OutputChanged += (x) => result = x;
            runSync.Run("write test");
            Assert.AreEqual("test\r\n\r\n", result);
        }

        [Test]
        public void IfCanRunCommandAndRaiseEventRunFinished()
        {
            var runSync = new RunSync();
            var result = 0;
            runSync.RunFinished += () => result = 1;
            runSync.Run("write test");
            Assert.AreEqual(1, result);
        }

        [Test]
        public void IfCanRunCommandWithNoOutput()
        {
            var runSync = new RunSync();
            var result = "";
            runSync.OutputChanged += (x) => result = x;
            runSync.Run("write");
            Assert.AreEqual("No output", result);
        }

        [Test]
        public void IfCanRunWrongCommand()
        {
            var runSync = new RunSync();
            var result = "";
            runSync.OutputChanged += (x) => result = x;
            runSync.Run("thingdingding");
            Assert.AreEqual("No output", result);
        }
    }
}