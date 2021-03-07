using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MysteryApp4
{
    public class CloseableThreadLocal
    {
        [ThreadStatic]
        private static Dictionary<object, object> slots;

        public static Dictionary<object, object> Slots =>
            slots ??= new Dictionary<object, object>();

        protected virtual object InitialValue() => null;

        public virtual object Get()
        {
            object val;

            if (Slots.TryGetValue(this, out val))
                return val;

            val = InitialValue();
            Set(val);

            return val;
        }

        public virtual void Set(object val) =>
            Slots[this] = val;

        public virtual void Close()
        {
            if (slots != null)// intentionally using the field here, to avoid creating the instance
                slots.Remove(this);
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            var mre = new ManualResetEventSlim();
            Console.WriteLine($"Process ID: {Environment.ProcessId}");
            Console.Write("Working, press any key to stop...");

            ThreadPool.SetMinThreads(1000, 100);

            var task = Task.Run(() =>
            {
                var tl = new CloseableThreadLocal();
                int x = 0;
                while (!mre.IsSet)
                {
                    Task.Run(() =>
                    {
                        tl.Set("hello!");
                        _ = tl.Get();
                        Thread.Sleep(10);
                    });

                    if(++x % 1000 == 0)
                    {
                        GC.Collect(2);
                        GC.WaitForPendingFinalizers();
                    }

                }
            });

            Console.ReadKey();
            mre.Set();

            task.Wait();
            Console.WriteLine("OK, bye!");
        }
    }
}
