using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Workflow.Direct
{
    public class Worker<T> : IWorker<T>
        where T : class
    {
        private readonly ActionBlock<T> _pipe;
        private readonly Func<T, Task> _handleMessage;

        public Worker(Func<T, Task> handleMessage)
        {
            _pipe = new ActionBlock<T>(HandleMessageAsync);
            _handleMessage = handleMessage;
        }

        public Task PostAsync(T message)
        {
            return _pipe.SendAsync(message);
        }

        private Task HandleMessageAsync(T message)
        {
            return _handleMessage.Invoke(message);
        }
    }
}
