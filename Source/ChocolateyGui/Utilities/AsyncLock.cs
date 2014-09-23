// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="AsyncLock.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class AsyncLock
    {
        private readonly Task<Releaser> _releaser;
        private readonly AsyncSemaphore _semaphore;

        public AsyncLock()
        {
            this._semaphore = new AsyncSemaphore(1);
            this._releaser = TaskEx.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = this._semaphore.WaitAsync();
            return wait.IsCompleted ? this._releaser
                       : wait.ContinueWith(
                           (task) => new Releaser(this),
                           CancellationToken.None,
                           TaskContinuationOptions.ExecuteSynchronously,
                           TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _lock;

            public Releaser(AsyncLock @lock)
            {
                this._lock = @lock;
            }

            public void Dispose()
            {
                if (this._lock != null)
                {
                    this._lock._semaphore.Release();
                }
            }
        }
    }
}