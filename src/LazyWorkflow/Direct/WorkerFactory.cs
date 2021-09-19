using System;
using System.Threading.Tasks;

namespace Workflow.Direct
{
    public class WorkerFactory<T> : IWorkerFactory<T>
        where T : class
    {
        private readonly Func<T, Task> _handleMessage;

        public WorkerFactory(Func<T, Task> handleMessage)
        {
            _handleMessage = handleMessage;
        }

        public IWorker<T> CreateWorker()
        {
            return new Worker<T>(_handleMessage);
        }
    }
}
