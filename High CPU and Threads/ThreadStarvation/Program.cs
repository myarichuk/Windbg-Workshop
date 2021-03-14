using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HighCPU
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");

            for (int niceThreads = 1; niceThreads <= 5; niceThreads++)
                new Thread(() =>
                {
                    while (true)
                        Thread.Sleep(500);
                })
                { Name = "Nice Thread #" + niceThreads }.Start();

            new Thread(() =>
            {
                var iEnumerablesArray = GenerateArrayOfIEnumerables();
                List<string> result = null;
                int x = 0;
                while (true)
                {
                    if (x++ % 2 == 0)
                        Task.Run(() => result = iEnumerablesArray.Aggregate(Enumerable.Concat).ToList());
                    else
                        Thread.Sleep(500);
                }

                // just to be sure that Release mode would not omit some lines:
                Console.WriteLine(result);
            })
            { Name = "Evil Thread" }.Start();
        }

        private readonly static string MegaString = new string('a', 10000);
        static IEnumerable<string>[] GenerateArrayOfIEnumerables()
        {
            return Enumerable
                  .Range(0, 100000)
                  .Select(_ => Enumerable.Range(0, 1).Select(__ => MegaString))
                  .ToArray();
        }
    }

}
