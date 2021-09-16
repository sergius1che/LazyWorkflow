using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Workflow.Direct
{
    public class WorkflowForkManagment<T> : IWorkflowForkManagment<T>, IDisposable
    {
        private readonly Func<T, int> _getMessageKey;
        private readonly IWorkerFactory<T> _workerFactory;
        private readonly IWorkflowSettings _settings;
        private readonly ConcurrentDictionary<int, CacheValue<T>> _workers;

        private Timer _cleanupTimer;
        private bool _disposed;

        public WorkflowForkManagment(
            Func<T, int> getMessageKey,
            IWorkerFactory<T> workerFactory,
            IWorkflowSettings settings)
        {
            _getMessageKey = getMessageKey;
            _workerFactory = workerFactory;
            _settings = settings;
            _workers = new ConcurrentDictionary<int, CacheValue<T>>(2, settings.WorkersCapacity);
            _cleanupTimer = new Timer(CleanUp, null, settings.CleanupPeriod, settings.CleanupPeriod);
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
            return new CacheValue<T>(worker, _settings.WorkerLifetime);
        }

        private void CleanUp(object state)
        {
            foreach (var keyValue in _workers.Where(x => x.Value.ExpireDate < DateTime.UtcNow))
            {
                if (_workers.TryRemove(keyValue.Key, out var cache)
                    && cache.Value is IDisposable dispCache)
                {
                    dispCache.Dispose();
                }
            }
        }
    }
}
