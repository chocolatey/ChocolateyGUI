using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChocolateyGui.Utilities
{
    internal class AsyncLock
    {
        private readonly AsyncSemaphore _semaphore;
        private readonly Task<Releaser> _releaser; 
        public AsyncLock()
        {
            _semaphore = new AsyncSemaphore(1);
            _releaser = TaskEx.FromResult(new Releaser(this)); 
        }

        public Task<Releaser> LockAsync()
        {
            var wait = _semaphore.WaitAsync();
            return wait.IsCompleted ?
                _releaser :
                wait.ContinueWith((task) => new Releaser(this),
                    CancellationToken.None,TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default); 
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _lock;
            public Releaser(AsyncLock @lock)
            {
                _lock = @lock;
            }
            public void Dispose()
            {
                if (_lock != null)
                    _lock._semaphore.Release(); 
            }
        }
    }
}
