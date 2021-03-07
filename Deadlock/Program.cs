using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Deadlock
{
    class Program
    {
        private static readonly CountdownEvent DeadlockSync = new CountdownEvent(2);

        public class SharedResource
        {
            public string Name { get; set; }
            public object Sync { get; } = new object();
            public void DoWork() => Thread.Sleep(1000);
        }

        public class ResourceConsumer
        {
            public string Name { get; set; }
            public SharedResource First { get; set; }
            public SharedResource Second { get; set; }

            public void DoWork()
            {
                lock (First.Sync)
                {
                    First.DoWork();

                    DeadlockSync.Signal();
                    DeadlockSync.Wait();

                    lock (Second.Sync)
                    {
                        Second.DoWork();
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            Console.Write("Working, press Ctrl+C to stop...");

            var resourceA = new SharedResource { Name = "ResourceA" };
            var resourceB = new SharedResource { Name = "ResourceB" };

            var consumerA = new ResourceConsumer
            {
                Name = "ConsumerA",
                First = resourceA,
                Second = resourceB
            };

            var consumerB = new ResourceConsumer
            {
                Name = "ConsumerB",
                First = resourceB,
                Second = resourceA
            };

            var t1 = Task.Run(() => consumerA.DoWork());
            var t2 = Task.Run(() => consumerB.DoWork());

            Task.WaitAll(t1, t2);
        }
    }
}