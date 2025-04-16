// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Blackbox;
using Dictionary;
using StackExchange.Profiling;

#pragma warning disable S125 
namespace DictionaryTests
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                            $" {nameof(Program)}.{nameof(Main)}() started.");

            //BenchmarkDotNet.Reports.Summary summary = BenchmarkRunner.Run<BenchmarkTestClass>();

            var mp = MiniProfiler.StartNew("Default Mini-Profiler");
            //MiniProfiler.Current.Name = "Default Mini-Profiler";

            mp.Step($"CALL: {nameof(BenchmarkTestClass)}.{nameof(BenchmarkTestClass.RunAllWorkerItemsAsync)}...");
                await new BenchmarkTestClass().RunAllWorkerItemsAsync();

            await mp!.StopAsync();
            Console.WriteLine(mp.RenderPlainText());

            Console.WriteLine("Program finished. Press any key quit the app...");
            Console.Read();
        }
    }
}