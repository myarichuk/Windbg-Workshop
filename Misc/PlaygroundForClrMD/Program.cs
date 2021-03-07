using Microsoft.Diagnostics.Runtime;
using System;
using System.Linq;

namespace PlaygroundForClrMD
{
    class Program
    {
        static void Main(string[] args)
        {
            using var dataTarget = DataTarget.LoadDump("d:\\Fibonacci.dmp");
            using var clrRuntime = dataTarget.CreateRuntime();

            var strings = clrRuntime.Heap.EnumerateObjects()
                                    .Where(o => o.Type == clrRuntime.Heap.StringType);

            foreach (var stringObj in strings.Where(str => str.Size > 50))
            {
                Console.WriteLine(
                    $"{Environment.NewLine}>>>{Environment.NewLine}" +
                    $"{stringObj.AsString()}" +
                    $"{Environment.NewLine}<<<{Environment.NewLine}");
            }
        }
    }
}
