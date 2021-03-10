using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deadlock2
{
    //credit for the repro code: https://devblogs.microsoft.com/premier-developer/the-danger-of-taskcompletionsourcet-class/
    public class DatabaseFacade : IDisposable
    {
        private readonly BlockingCollection<(string item, TaskCompletionSource<string> result)> _queue =
            new BlockingCollection<(string item, TaskCompletionSource<string> result)>();

        private readonly Task _processItemsTask;

        public DatabaseFacade() => _processItemsTask = Task.Run(ProcessItems);

        public void Dispose() => _queue.CompleteAdding();

        public Task SaveAsync(string command)
        {
            var tcs = new TaskCompletionSource<string>();
            _queue.Add((item: command, result: tcs));
            return tcs.Task;
        }

        private async Task ProcessItems()
        {
            foreach (var item in _queue.GetConsumingEnumerable())
            {
                Console.WriteLine($"DatabaseFacade: executing '{item.item}'...");

                // Waiting a bit to emulate some IO-bound operation
                await Task.Delay(100);
                item.result.SetResult("OK");
                Console.WriteLine("DatabaseFacade: done.");
            }
        }
    }

    public class Logger : IDisposable
    {
        private readonly DatabaseFacade _facade;
        private readonly BlockingCollection<string> _queue =
            new BlockingCollection<string>();

        private readonly Task _saveMessageTask;

        public Logger(DatabaseFacade facade) =>
            (_facade, _saveMessageTask) = (facade, Task.Run(SaveMessage));

        public void Dispose() => _queue.CompleteAdding();

        public void WriteLine(string message) => _queue.Add(message);

        private async Task SaveMessage()
        {
            foreach (var message in _queue.GetConsumingEnumerable())
            {
                // "Saving" message to the file
                Console.WriteLine($"Logger: {message}");

                // And to our database through the facade
                await _facade.SaveAsync(message);
            }
        }
    }

    class Program
    {

        static async Task Main(string[] args)
        {
            using (var facade = new DatabaseFacade())
            using (var logger = new Logger(facade))
            {
                logger.WriteLine("My message");
                await Task.Delay(100);

                await facade.SaveAsync("Another string");
                Console.WriteLine("Aha!");
            }
        }
    }

}
