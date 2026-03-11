using System;
using System.Diagnostics;
using System.Threading;

namespace ModernDiagnostics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Modern Diagnostics Demo");
            Console.WriteLine($"Process ID: {Environment.ProcessId}");
            Console.WriteLine("Run: dotnet-counters monitor -p " + Environment.ProcessId);
            Console.WriteLine("Run: dotnet-dump collect -p " + Environment.ProcessId);
            Console.WriteLine("Run: dotnet-gcdump collect -p " + Environment.ProcessId);
            Console.WriteLine("Run: dotnet-trace collect -p " + Environment.ProcessId);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
