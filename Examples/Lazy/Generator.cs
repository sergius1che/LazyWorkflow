using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lazy
{
    public delegate Task AsyncEventHandler<T>(object? sender, T args);

    public class Generator
    {
        private readonly int _idStart;
        private readonly int _idEnd;
        private readonly int _msDelay;
        private Task _generatorTask;
        private CancellationTokenSource _cts;

        public Generator(int idStart, int idEnd, int msDelay)
        {
            _idStart = idStart;
            _idEnd = idEnd;
            _msDelay = msDelay;
        }

        public event AsyncEventHandler<Message> OnGenerated;

        public void Start(int cancelAfter)
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }

            _cts = new CancellationTokenSource();
            _cts.CancelAfter(cancelAfter);
            _generatorTask = StartGenerator(_cts.Token);
        }

        private async Task StartGenerator(CancellationToken token)
        {
            await Task.Yield();

            var index = 0;
            var rnd = new Random();

            while (!token.IsCancellationRequested)
            {
                var msg = new Message
                {
                    Id = rnd.Next(_idStart, _idEnd),
                    Index = index,
                    Data = index.ToString(),
                };

                index++;

                await OnGenerated?.Invoke(this, msg);

                await Task.Delay(_msDelay);
            }
        }
    }
}
