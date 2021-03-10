using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Crash2
{
    //Taken from repro of this issue: https://github.com/dotnet/runtime/issues/44071
    //This shows how a bug in the runtime can crash the
    //note: this issue is scheduled to be fixed only in .Net 6
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Process ID:" + Process.GetCurrentProcess().Id);
            Console.WriteLine("Press any key to start..");
            Console.ReadKey();

            var locks = new[] { new object(), new object() };

            for (int i = 0; i < locks.Length; i++)
            {
                var thread = new Thread(state =>
                {
                    Monitor.Enter(locks[(int)state]);
                });

                thread.Start(i);
                thread.Join();
            }

            // Force the thread to be collected, orphaning the lock
            // Note: the threads have no reference to GC root so they *should* be collected
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced, true, true);

            for (int i = 0; i < locks.Length; i++)
            {
                //promote thin-lock to syncblk
                _ = locks[i].GetHashCode();
            }

            // trigger the crash
            for (int i = 0; i < locks.Length; i++)
            {
                var thread = new Thread(state =>
                {
                    _ = Monitor.IsEntered(locks[(int)state]);
                });

                thread.Start(i);
                thread.Join();
            }
        }
    }
}
