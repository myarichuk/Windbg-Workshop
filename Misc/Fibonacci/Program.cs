using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;

namespace Fibonacci
{

    [EventSource(Name = "MyMegaEventPipeProvider")]
    public sealed class MinimalEventCounterSource : EventSource
    {
        public static readonly MinimalEventCounterSource Log = new MinimalEventCounterSource();

        //note: at the end of each time interval, summary statistics for the set are computed such as the min, max, and mean.
        //dotnet-counters will always display the mean value
        private EventCounter _requestCounter;

        private MinimalEventCounterSource() =>
            _requestCounter = new EventCounter("fibonacci-step", this)
            {
                DisplayName = "Fibonacci Calculation Step"
            };

        public void OnFibonacciStep(double n)
        {
            WriteEvent(1, n);
            _requestCounter?.WriteMetric(n);
        }

        protected override void Dispose(bool disposing)
        {
            _requestCounter?.Dispose();
            _requestCounter = null;

            base.Dispose(disposing);
        }
    }

    //small app for simple WinDbg investigations
    //dotnet-counters monitor --process-id XYZ --refresh-interval 1 MyMegaEventPipeProvider
    class Program
    {
        public static double Fib(double n)
        {
            MinimalEventCounterSource.Log.OnFibonacciStep(n);
            return n <= 2 ? 1 : Fib(n - 1) + Fib(n - 2);
        }

        public static Dictionary<double, double> ResultCache = new Dictionary<double, double>();

        static async Task Main(string[] args)
        {
            var random = new Random();
            while(true)
            {
                double n = random.NextDouble() * random.Next(1, 35);

                //ensure some GC activity as well
                if (random.Next(0, 100) % 4 == 0)
                    ResultCache = new Dictionary<double, double>();

                if(!ResultCache.TryGetValue(n, out var result))
                {
                    result = await Task.Run(() => Fib(n));
                    ResultCache.Add(n, result);
                }

                Console.WriteLine(result);
                Thread.Sleep(50);
            }
        }
    }
}
