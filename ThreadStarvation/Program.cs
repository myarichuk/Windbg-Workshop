using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadStarvation
{
    class Program
    {
        static void Main(string[] args)
        {

            ThreadPool.SetMinThreads(8, 8);

            Task.Factory.StartNew(BackgroundWorker, TaskCreationOptions.None);
            Console.ReadKey();
        }

        static void BackgroundWorker()
        {
            while (true)
            {
                DoStuff();

                //notice -> we are adding BLOCKING worker threads faster than they can be processed
                Thread.Sleep(200);
            }
        }

        static async Task DoStuff()
        {
            await Task.Yield();

            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                tcs.SetResult(true);
            });

            //not doing await on purpose
            tcs.Task.Wait();
        }
    }
}