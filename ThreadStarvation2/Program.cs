using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ThreadStarvation2
{
    //credit: code adapted from https://stackoverflow.com/a/8451280/320103
    public class PriorityTest
    {
        private volatile bool _running;
        public PriorityTest() => _running = true;

        public bool IsRunning
        {
            set => _running = value;
        }

        [DllImport("kernel32")]
        static extern int GetCurrentThreadId();

        public void ThreadMethod()
        {
            long iterations = 0;

            foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
            {
                int utid = GetCurrentThreadId();
                if (utid == pt.Id)
                    pt.ProcessorAffinity = (IntPtr)1; //ensure all threads 'fight' for specific core
            }

            while (_running)
            {
                iterations++;
            }

            Console.WriteLine("{0} -> priority = {1,11} -> " +
                " iterations count = {2,13}", Thread.CurrentThread.Name,
                Thread.CurrentThread.Priority.ToString(),
                iterations.ToString("N0"));
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var priorityTest = new PriorityTest();
            var startDelegate = new ThreadStart(priorityTest.ThreadMethod);

            var lowPriorityThread = new Thread(startDelegate);
            lowPriorityThread.Name = "Low priority thread";
            lowPriorityThread.Priority = ThreadPriority.Lowest;

            for (int i = 1; i <= 2; i++)
            {
                var workerThread = new Thread(startDelegate);
                workerThread.Name = "High priority thread #" + i;
                workerThread.Priority = ThreadPriority.Highest;
                workerThread.Start();
            }

            lowPriorityThread.Start();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            priorityTest.IsRunning = false;

        }
    }
}
