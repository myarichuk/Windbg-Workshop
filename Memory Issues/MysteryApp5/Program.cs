using System;
using System.Threading;
using System.Threading.Tasks;

namespace MysteryApp5
{
    public class TimerEventDispatcher
    {
        private System.Timers.Timer _timer;

        public TimerEventDispatcher(System.Timers.Timer timer)
        {

            _timer = timer;
            _timer.Elapsed += (_, e) => OnAlarmNow(e);
        }

        public event EventHandler TimerElapsedNow;


        protected virtual void OnAlarmNow(EventArgs e) =>
            TimerElapsedNow?.Invoke(this, e);
    }

    public static class Program
    {

        static void Main(string[] args)
        {
            var mre = new ManualResetEventSlim();
            Console.WriteLine($"Process ID: {Environment.ProcessId}");
            Console.Write("Working, press any key to stop...");

            var task = Task.Run(() =>
            {
                var timer = new System.Timers.Timer(500);
                timer.Start();
                int x = 0;
                while(!mre.IsSet)
                {
                    var eventDispatcher = new TimerEventDispatcher(timer);
                    eventDispatcher.TimerElapsedNow += EventDispatcher_TimerElapsedNow;

                    if (++x % 2000 == 0)
                        Thread.Sleep(50);
                }
            });

            Console.ReadKey();
            mre.Set();

            task.Wait();
            Console.WriteLine("OK, bye!");
        }

        private static void EventDispatcher_TimerElapsedNow(object sender, EventArgs e)
        {
        }
    }
}
