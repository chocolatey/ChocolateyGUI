using System;
using System.Threading;
using Chocolatey.Explorer.Powershell;
using Chocolatey.Explorer.Test.Extensions;
using NUnit.Framework;

namespace Chocolatey.Explorer.Test.Powershell
{
    public class TestRunAsync
    {
        [Test]
        public void IfCanRunCommand()
        {
            var runAsync = new RunAsync();
            runAsync.Run("write test");
        }

        [Test]
        public void IfCanRunCommandAndReturnOutput()
        {
            var runAsync = new RunAsync();
            var result = "";
            var waitHandle = new AutoResetEvent(false);
            runAsync.OutputChanged += (x) =>
                {
                    result = x;
                    waitHandle.Set();
                };
            runAsync.Run("write test");

            waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(5));

            Assert.AreEqual("test\r\n", result);
        }

        [Test]
        public void IfCanRunCommandAndRaiseEventRunFinished()
        {
            var runSync = new RunAsync();
            var result = 0;
            var waitHandle = new AutoResetEvent(false);
            runSync.RunFinished += () => 
            { 
                result = 1;
                waitHandle.Set();
            };
            runSync.Run("write test");
            waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(5));
            Assert.AreEqual(1, result);
        }

        [Test]
        public void IfCanRunCommandWithNoOutput()
        {
            var runSync = new RunAsync();
            var result = 0;
            var waitHandle = new AutoResetEvent(false);
            runSync.RunFinished += () =>
                {
                    result = 1;
                    waitHandle.Set();
                };
            runSync.Run("write");
            waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(5));
            Assert.AreEqual(1, result);
        }

        [Test]
        public void IfCanRunWrongCommand()
        {
            var runSync = new RunAsync();
            var result = 0;
            var waitHandle = new AutoResetEvent(false);
            runSync.RunFinished += () =>
                {
                    result = 1;
                    waitHandle.Set();
                };
            runSync.Run("thingdingding");
            waitHandle.ThrowIfHandleTimesOut(TimeSpan.FromSeconds(5));
            Assert.AreEqual(1, result);
        }
    }
}