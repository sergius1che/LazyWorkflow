using System;
using System.Threading;
using System.Threading.Tasks;

namespace Workflow.Lazy
{
    public class LazyWorker<T> : IWorker<T>, IDisposable
        where T : class
    {
        private readonly Func<T, Task> _handleMessage;
        private readonly Func<T, T, T> _merge;
        private readonly int _periodMs;

        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly CancellationTokenSource _cts;
        private readonly Task _handlingTask;

        private T _cache = null;
        private bool _disposed = false;

        public LazyWorker(
            Func<T, Task> handleMessage,
            Func<T, T, T> merge,
            int periodMs)
        {
            _handleMessage = handleMessage;
            _merge = merge;
            _periodMs = periodMs;

            _semaphoreSlim = new SemaphoreSlim(1);
            _cts = new CancellationTokenSource();
            _handlingTask = HandlingLoopAsync();
        }

        public async Task PostAsync(T message)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (_cache == null)
                {
                    _cache = message;
                }
                else
                {
                    _cache = _merge.Invoke(_cache, message);
                }
            }
            catch (Exception ex)
            {
                throw new WorkflowException("Error on merge messages", ex);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _cts.Cancel();
                _semaphoreSlim.Dispose();
            }

            _disposed = true;
        }

        private async Task HandlingLoopAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                var buffer = await TakeCache();

                if (buffer == null)
                {
                    await Task.Delay(_periodMs);
                    continue;
                }

                var dt = DateTime.UtcNow;

                await _handleMessage.Invoke(buffer);

                var delay = _periodMs - (int)(DateTime.UtcNow - dt).TotalMilliseconds;

                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
        }

        private async Task<T> TakeCache()
        {
            if (_cache == null)
            {
                return null;
            }

            await _semaphoreSlim.WaitAsync();

            T buffer = _cache;
            _cache = null;

            _semaphoreSlim.Release();

            return buffer;
        }
    }
}
