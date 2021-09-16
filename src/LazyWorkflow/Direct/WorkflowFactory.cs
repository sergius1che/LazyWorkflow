using System;
using System.Threading.Tasks;

namespace Workflow.Direct
{
    public class WorkflowFactory<T> : IWorkerFactory<T>
    {
        private readonly Func<T, Task> _handleMessage;

        public WorkflowFactory(Func<T, Task> handleMessage)
        {
            _handleMessage = handleMessage;
        }

        public IWorker<T> CreateWorker()
        {
            return new Worker<T>(_handleMessage);
        }
    }
}
