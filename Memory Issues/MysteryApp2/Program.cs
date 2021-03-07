using System;
using System.Threading;
using System.Threading.Tasks;

namespace MysteryApp2
{
    public class AMysteryClass
    {
        private readonly int _delay;

        public AMysteryClass(int delay) => _delay = delay;
        ~AMysteryClass() => Thread.Sleep(_delay);
    }
    class Program
    {

        static void Main(string[] args)
        {
            var mre = new ManualResetEventSlim();

            Console.WriteLine($"Process ID: {Environment.ProcessId}");
            Console.Write("Working, press any key to stop...");

            var task = Task.Run(() =>
            {
                int x = 0;
                var random = new Random();
                while (!mre.IsSet)
                {
                    Task.Run(() =>
                    {
                        var _ = new AMysteryClass(random.Next(100, 5000));
                    });
                }
            });

            //166x86
            Console.ReadKey();
            mre.Set();

            task.Wait();
            Console.WriteLine("OK, bye!");
        }
    }
}
