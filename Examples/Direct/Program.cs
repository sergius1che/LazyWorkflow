using System;
using System.Threading.Tasks;
using Workflow.Builders;

namespace Direct
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new Generator(1, 5, 20);
            var fork = new DirectWorkflowBuilder<Message>(x => x.Id, Handle)
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

            Console.WriteLine($"Received message: {msg.Id} index: {msg.Index}");

            return Task.Delay(rnd.Next(900, 1100));
        }
    }
}
