using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Workflow.Direct
{
    public class WorkflowManager<T> : IWorkflowManager<T>, IDisposable
    {
        private readonly Func<T, int> _getMessageKey;
        private readonly IWorkerFactory<T> _workerFactory;
        private readonly ConcurrentDictionary<int, CacheValue<T>> _workers;

        private Timer _cleanupTimer;
        private bool _disposed;

        public WorkflowManager(
            Func<T, int> getMessageKey,
            IWorkerFactory<T> workerFactory)
        {
            _getMessageKey = getMessageKey;
            _workerFactory = workerFactory;
            _workers = new ConcurrentDictionary<int, CacheValue<T>>(2, 1024);
            _cleanupTimer = new Timer(CleanUp, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));
            _disposed = false;
        }

        public Task PostAsync(T message)
        {
            var key = _getMessageKey(message);
            var currentWorker = _workers.GetOrAdd(key, BuildWorker);

            currentWorker.Refresh();

            return currentWorker.Value.PostAsync(message);
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
                _cleanupTimer?.Dispose();
            }

            _cleanupTimer = null;

            _disposed = true;
        }

        private CacheValue<T> BuildWorker(int key)
        {
            var worker = _workerFactory.CreateWorker();
            return new CacheValue<T>(worker, TimeSpan.FromSeconds(120));
        }

        private void CleanUp(object state)
        {
            foreach (var keyValue in _workers.Where(x => x.Value.ExpireDate < DateTime.UtcNow))
            {
                if (_workers.TryRemove(keyValue.Key, out var cache) && cache is IDisposable dispCache)
                {
                    dispCache.Dispose();
                }
            }
        }
    }
}
