using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hang
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Process.GetCurrentProcess().Id);
            var dict = new Dictionary<int, string>();
            var random = new Random();

            while(true)
            {
                Parallel.For(0, 1000, _ =>
                {
                    try
                    {
                        var num = random.Next();
                        if (!dict.TryGetValue(num, out var value))
                            dict.Add(num, num.ToString());
                    }
                    catch (ArgumentException e) when (e.Message == "An item with the same key has already been added.")
                    {
                        /*ignore 'key already exists' exceptions*/
                    }
                });

                Console.Write(".");
            }
        }

    }
}
