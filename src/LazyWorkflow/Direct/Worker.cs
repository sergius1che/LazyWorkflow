using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Workflow.Direct
{
    public class Worker<T> : IWorker<T>
    {
        private readonly ActionBlock<T> _pipe;
        private readonly Func<T, Task> _handleMessage;

        public Worker(Func<T, Task> handleMessage)
        {
            _pipe = new ActionBlock<T>(HandleMessage);
            _handleMessage = handleMessage;
        }

        public Task PostAsync(T message)
        {
            return _pipe.SendAsync(message);
        }

        private Task HandleMessage(T message)
        {
            return _handleMessage.Invoke(message);
        }
    }
}
