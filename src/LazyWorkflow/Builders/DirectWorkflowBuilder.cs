using System;
using System.Threading.Tasks;
using Workflow.Direct;

namespace Workflow.Builders
{
    public class DirectWorkflowBuilder<T>
        where T : class
    {
        protected readonly Func<T, int> _getMessageKey;
        protected readonly Func<T, Task> _handleMessage;

        protected TimeSpan? _workerLifetime;
        protected int? _workersCapacity;
        protected TimeSpan? _cleanupPeriod;

        public DirectWorkflowBuilder(
            Func<T, int> getMessageKey,
            Func<T, Task> handleMessage)
        {
            _getMessageKey = getMessageKey ?? throw new ArgumentNullException(nameof(getMessageKey));
            _handleMessage = handleMessage ?? throw new ArgumentNullException(nameof(handleMessage));
        }

        public virtual DirectWorkflowBuilder<T> SetWorkerLifeTime(TimeSpan lifetime)
        {
            if (lifetime <= TimeSpan.Zero)
            {
                throw new ArgumentException("Worker lifetime must be greater " + TimeSpan.Zero.ToString(), nameof(lifetime));
            }

            _workerLifetime = lifetime;
            return this;
        }

        public virtual DirectWorkflowBuilder<T> SetCleanupPeriod(TimeSpan period)
        {
            if (period <= TimeSpan.Zero)
            {
                throw new ArgumentException("Clean up period must be greater " + TimeSpan.Zero.ToString(), nameof(period));
            }

            _cleanupPeriod = period;
            return this;
        }

        public virtual DirectWorkflowBuilder<T> SetWorkersCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Initial capacity of workers must be greater 0", nameof(capacity));
            }

            _workersCapacity = capacity;
            return this;
        }

        public virtual IWorkflowForkManagment<T> Build()
        {
            var settings = new WorkflowSettings
            {
                CleanupPeriod = _cleanupPeriod ?? TimeSpan.FromSeconds(10),
                WorkerLifetime = _workerLifetime ?? TimeSpan.FromSeconds(60),
                WorkersCapacity = _workersCapacity ?? 1024,
            };

            var factory = new WorkerFactory<T>(_handleMessage);

            return new WorkflowForkManagment<T>(_getMessageKey, factory, settings);
        }
    }
}
