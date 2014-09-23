// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AsyncSemaphore.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class AsyncSemaphore
    {
        private static readonly Task<bool> Completed = TaskEx.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException("initialCount");
            }

            this._currentCount = initialCount;
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (this._waiters)
            {
                if (this._waiters.Count > 0)
                {
                    toRelease = this._waiters.Dequeue();
                }
                else
                {
                    ++this._currentCount;
                }
            }

            if (toRelease != null)
            {
                toRelease.SetResult(true);
            }
        }

        public Task WaitAsync()
        {
            lock (this._waiters)
            {
                if (this._currentCount > 0)
                {
                    --this._currentCount;
                    return Completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    this._waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }
    }
}