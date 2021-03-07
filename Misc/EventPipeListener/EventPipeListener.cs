using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventPipeListener
{

    public class EventPipeListener : IDisposable
    {
        private readonly EventPipeSession _listeningSession;
        private readonly EventPipeEventSource _eventSource;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public event Action<TraceEvent> ClrEvents;
        public event Action<TraceEvent> KernelEvents;
        public event Action<TraceEvent> CustomEvents;

        public EventPipeListener(int targetProcessId, IEnumerable<EventPipeProvider> providers)
        {
            var client = new DiagnosticsClient(targetProcessId);
            _listeningSession = client.StartEventPipeSession(providers);

            _eventSource = new EventPipeEventSource(_listeningSession.EventStream);

            _eventSource.Kernel.All += (TraceEvent @event) => KernelEvents?.Invoke(@event);
            _eventSource.Clr.All += (TraceEvent @event) => ClrEvents?.Invoke(@event);
            _eventSource.Dynamic.All += (TraceEvent @event) => CustomEvents?.Invoke(@event);
            Task.Factory.StartNew(() => _eventSource.Process(), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            //with a code like this we can output a memory dump depending on some threshold of CPU usage
            /*
            _eventSource.Dynamic.All += (TraceEvent obj) =>
                    {
                        if (obj.EventName.Equals("EventCounters"))
                        {
                            IDictionary<string, object> payloadVal = (IDictionary<string, object>)(obj.PayloadValue(0));
                            IDictionary<string, object> payloadFields = (IDictionary<string, object>)(payloadVal["Payload"]);
                            if (payloadFields["Name"].ToString().Equals("cpu-usage"))
                            {
                                double cpuUsage = Double.Parse(payloadFields["Mean"]);
                                if (cpuUsage > (double)threshold)
                                {
                                    client.WriteDump(DumpType.Normal, "./minidump.dmp");
                                }
                            }
                        }
                    }
             */
        }

        public void Dispose()
        {
            _cts.Cancel();
            _eventSource?.Dispose();
            _listeningSession?.Dispose();
        }
    }
}
