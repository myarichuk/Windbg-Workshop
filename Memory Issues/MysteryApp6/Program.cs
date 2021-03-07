using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MysteryApp5
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var mre = new ManualResetEventSlim();
            Console.WriteLine($"Process ID: {Environment.ProcessId}");
            Console.Write("Working, press any key to stop...");

            var task = Task.Run(() =>
            {
                while (!mre.IsSet)
                {
                    //note: PhysicalFileProvider under the hood uses unmanaged resources (see https://github.com/dotnet/runtime/blob/master/src/libraries/System.IO.FileSystem.Watcher/src/System/IO/FileSystemWatcher.Win32.cs#L32)
                    try
                    {
                        var fp = new PhysicalFileProvider(Path.GetTempPath());
                        fp.Watch("*.*");
                    }
                    catch (Exception) { }
                }
            });

            Console.ReadKey();
            mre.Set();

            task.Wait();
            Console.WriteLine("OK, bye!");
        }
    }
}