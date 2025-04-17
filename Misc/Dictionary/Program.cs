// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using StackExchange.Profiling;

#pragma warning disable S125 

namespace DictionaryTests
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            //BenchmarkDotNet.Reports.Summary summary
            //    = BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchmarkTestClass>();

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                            $" {nameof(Program)}.{nameof(Main)}() started.");


            var mp = MiniProfiler.StartNew("Default Mini-Profiler");
            //MiniProfiler.Current.Name = "Default Mini-Profiler";

            using (mp.Step($"CALL: {nameof(BenchmarkTestClass)}.{nameof(BenchmarkTestClass.RunAllWorkerItemsAsync)}..."))
            {
                await new BenchmarkTestClass().RunAllWorkerItemsAsync();
            }

            await mp!.StopAsync();
            Trace.TraceInformation(mp.RenderPlainText());

            Trace.TraceInformation("Program finished. Press any key quit the app...");
            Console.Read();
        }
    }
}