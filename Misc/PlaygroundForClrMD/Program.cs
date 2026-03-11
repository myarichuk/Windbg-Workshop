using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.NETCore.Client;
using System;
using System.Linq;

namespace PlaygroundForClrMD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Playground for ClrMD and Diagnostics Client");

            // Example 1: Use Diagnostics Client to connect to a running process
            Console.WriteLine("Diagnostics Client: Searching for processes...");
            var publishedProcesses = DiagnosticsClient.GetPublishedProcesses();
            foreach (var pid in publishedProcesses)
            {
                try
                {
                    Console.WriteLine($"Found .NET process: {pid}");
                    // Here you could use client.DumpProcess(...) to get a dump
                    // var client = new DiagnosticsClient(pid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accessing process {pid}: {ex.Message}");
                }
            }

            Console.WriteLine("\nClrMD: Provide a path to a memory dump to analyze it.");
            // Example 2: Load a memory dump (uncomment and provide path)
            /*
            using var dataTarget = DataTarget.LoadDump("example.dmp");
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
            */
        }
    }
}
