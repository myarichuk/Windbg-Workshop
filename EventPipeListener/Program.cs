using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace EventPipeListener
{

    class Program
    {


        static void Main(string[] args)
        {
            Console.Write("Enter process Id to monitor:");
            if (!Int32.TryParse(Console.ReadLine(), out var processId))
                throw new ArgumentException("Invalid input, must be a number");

            using var listener = new EventPipeListener(processId, new[] {
                new EventPipeProvider("Microsoft-Windows-DotNETRuntime",
                            EventLevel.Informational, (long)ClrTraceEventParser.Keywords.GC),
                new EventPipeProvider("System.Runtime", EventLevel.Informational),
                new EventPipeProvider("MyMegaEventPipeProvider", EventLevel.Informational)
            });

            listener.ClrEvents += Listener_OnEvent;
            listener.CustomEvents += Listener_OnEvent;

            Console.ReadKey();
        }

        private static void Listener_OnEvent(TraceEvent @event)
        {
            Console.WriteLine(@event.EventName + ", " + @event.ProviderName);
        }
    }
}
