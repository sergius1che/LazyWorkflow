using System;
using System.Threading.Tasks;

namespace Workflow.Lazy
{
    public class LazyWorkerFactory<T> : IWorkerFactory<T>
        where T : class
    {
        private readonly Func<T, Task> _handleMessage;
        private readonly Func<T, T, T> _mergeMessages;
        private readonly ILazyWorkflowSettings _settings;

        public LazyWorkerFactory(
            Func<T, Task> handleMessage,
            Func<T, T, T> mergeMessages,
            ILazyWorkflowSettings settings)
        {
            _handleMessage = handleMessage;
            _mergeMessages = mergeMessages;
            _settings = settings;
        }

        public IWorker<T> CreateWorker()
        {
            return new LazyWorker<T>(_handleMessage, _mergeMessages, _settings.LazyPeriodMs);
        }
    }
}
