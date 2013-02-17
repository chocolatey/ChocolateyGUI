using System;
using System.Threading;

namespace Chocolatey.Explorer.Test.Extensions
{
    public static class AsyncTesting
    {
        public static void ThrowIfHandleTimesOut(this WaitHandle handle, TimeSpan maxWaitTime)
        {
            var signalReceived = handle.WaitOne(maxWaitTime);
            if (!signalReceived)
            {
                throw new Exception("Timeout while waiting on handle");
            }
        } 
    }
}