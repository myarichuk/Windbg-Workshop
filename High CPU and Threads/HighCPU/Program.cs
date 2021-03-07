using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HighCPU
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Environment.ProcessId}");

            for (int niceThreads = 1; niceThreads <= 10; niceThreads++)
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

                while(true)
                    Task.Run(() => result = iEnumerablesArray.Aggregate(Enumerable.Concat).ToList());

                // just to be sure that Release mode would not omit some lines:
                Console.WriteLine(result);
            })
            { Name = "Evil Thread" }.Start();
        }

        private readonly static string MegaString = new string('a', 10000);
        static IEnumerable<string>[] GenerateArrayOfIEnumerables()
        {
            return Enumerable
                  .Range(0, 10000)
                  .Select(_ => Enumerable.Range(0, 1).Select(__ => MegaString))
                  .ToArray();
        }
    }

}
