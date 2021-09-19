using System;
using System.Threading.Tasks;
using Workflow.Direct;
using Workflow.Lazy;

namespace Workflow.Builders
{
    public class LazyWorkflowBuilder<T> : DirectWorkflowBuilder<T>
        where T : class
    {
        private readonly Func<T, T, T> _mergeMessages;

        private int? _lazyPeriodMs;

        public LazyWorkflowBuilder(
            Func<T, int> getMessageKey,
            Func<T, Task> handleMessage,
            Func<T, T, T> mergeMessages)
            : base(getMessageKey, handleMessage)
        {
            _mergeMessages = mergeMessages;
        }

        public virtual LazyWorkflowBuilder<T> SetPeriodMs(int periodMs)
        {
            if (periodMs <= 0)
            {
                throw new ArgumentException("Handling period of workers must be greater 0", nameof(periodMs));
            }

            _lazyPeriodMs = periodMs;
            return this;
        }

        public override IWorkflowForkManagment<T> Build()
        {
            var settings = new LazyWorkflowSettings
            {
                CleanupPeriod = _cleanupPeriod ?? TimeSpan.FromSeconds(10),
                WorkerLifetime = _workerLifetime ?? TimeSpan.FromSeconds(60),
                WorkersCapacity = _workersCapacity ?? 1024,
                LazyPeriodMs = _lazyPeriodMs ?? 300,
            };

            var factory = new LazyWorkerFactory<T>(_handleMessage, _mergeMessages, settings);

            return new WorkflowForkManagment<T>(_getMessageKey, factory, settings);
        }
    }
}
