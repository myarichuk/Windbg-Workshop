using System;
using System.Threading;

namespace Crash2
{
    //This shows how orphaned lock may crash the process
    //see for explanation why: https://github.com/dotnet/runtime/issues/44071
    //note: this issue is scheduled to be fixed only in .Net 6
    public class Program
    {
        static void Main(string[] args)
        {
            var locks = new object[4];

            for (int i = 0; i < locks.Length; i++)
                locks[i] = new object();

            for (int i = 0; i < 4; i++)
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

            // Promote thinlock to a syncblock
            for (int i = 0; i < locks.Length; i++)
            {
                _ = locks[i].GetHashCode();
            }

            // Trigger the crash
            for (int i = 0; i < 4; i++)
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
