using System;
using System.Threading.Tasks;
using Workflow.Direct;

namespace Workflow.Builders
{
    public class DirectWorkflowBuilder<T>
    {
        private readonly Func<T, int> _getMessageKey;
        private readonly Func<T, Task> _handleMessage;

        private TimeSpan? _workerLifetime;
        private int? _workersCapacity;
        private TimeSpan? _cleanupPeriod;

        public DirectWorkflowBuilder(
            Func<T, int> getMessageKey,
            Func<T, Task> handleMessage)
        {
            _getMessageKey = getMessageKey ?? throw new ArgumentNullException(nameof(getMessageKey));
            _handleMessage = handleMessage ?? throw new ArgumentNullException(nameof(handleMessage));
        }

        public DirectWorkflowBuilder<T> SetWorkerLifeTime(TimeSpan lifetime)
        {
            if (lifetime <= TimeSpan.Zero)
            {
                throw new ArgumentException("Worker lifetime must be greater " + TimeSpan.Zero.ToString(), nameof(lifetime));
            }

            _workerLifetime = lifetime;
            return this;
        }

        public DirectWorkflowBuilder<T> SetCleanupPeriod(TimeSpan period)
        {
            if (period <= TimeSpan.Zero)
            {
                throw new ArgumentException("Clean up period must be greater " + TimeSpan.Zero.ToString(), nameof(period));
            }

            _cleanupPeriod = period;
            return this;
        }

        public DirectWorkflowBuilder<T> SetWorkersCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Initial capacity of workers must be greater 0", nameof(capacity));
            }

            _workersCapacity = capacity;
            return this;
        }

        public IWorkflowForkManagment<T> Build()
        {
            var settings = new WorkflowSettings
            {
                CleanupPeriod = _cleanupPeriod ?? TimeSpan.FromSeconds(10),
                WorkerLifetime = _workerLifetime ?? TimeSpan.FromSeconds(60),
                WorkersCapacity = _workersCapacity ?? 1024,
            };

            var factory = new WorkflowFactory<T>(_handleMessage);

            return new WorkflowForkManagment<T>(_getMessageKey, factory, settings);
        }
    }
}
