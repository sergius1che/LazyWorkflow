using System;
using System.Threading.Tasks;
using Workflow.Builders;

namespace Lazy
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new Generator(1, 5, 20);
            var fork = new LazyWorkflowBuilder<Message>(x => x.Id, Handle, Merge)
                .SetPeriodMs(500)
                .SetWorkersCapacity(5)
                .Build();

            generator.OnGenerated += (s, m) =>
            {
                return fork.PostAsync(m);
            };

            generator.Start(5000);

            Console.ReadKey();
        }

        private static Task Handle(Message msg)
        {
            var rnd = new Random();

            Console.WriteLine($"Received message: {msg.Id} index: {msg.Index} data: {msg.Data}");

            return Task.Delay(rnd.Next(200, 600));
        }

        private static Message Merge(Message oldMessage, Message newMessage)
        {
            oldMessage.Data += " " + newMessage.Data;
            return oldMessage;
        }
    }
}
