using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MysteryApp7
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var mre = new ManualResetEventSlim();
            Console.WriteLine($"Process ID: {Environment.ProcessId}");
            Console.Write("Working, press any key to stop...");

            var task = Task.Run(() =>
            {
                var heldArrays = new List<byte[]>();
                int i = 0;
                while (!mre.IsSet)
                {
                    // Allocate a large array (> 85,000 bytes goes to LOH)
                    var largeArray = new byte[90000];

                    // Fill with dummy data
                    for (int j = 0; j < largeArray.Length; j++)
                    {
                        largeArray[j] = 0xAA;
                    }

                    // Keep every 10th array to cause LOH fragmentation and steady growth
                    if (i % 10 == 0)
                    {
                        heldArrays.Add(largeArray);
                    }

                    i++;

                    // Slow down slightly to observe growth
                    Thread.Sleep(5);
                }
            });

            Console.ReadKey();
            mre.Set();

            task.Wait();
            Console.WriteLine("OK, bye!");
        }
    }
}
