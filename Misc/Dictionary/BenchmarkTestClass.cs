using System.Collections.Concurrent;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Blackbox;
using StackExchange.Profiling;

#pragma warning disable S125 
namespace Dictionary
{
    public class BenchmarkTestClass
    {
        [Benchmark]
        public async Task RunAllWorkerItemsAsync()
        {
            IWorkerItem[] workerItems =
            [
                new WorkerItemBase(),
                new HighCpuLoadObject(),
                new HighRamMemoryLoadObject(),
                new HandlingUnmanagedResource(),
            ];

            ConcurrentDictionary<int, IWorkerItem> workerDict = new();

            MiniProfiler.Current.Step("Adding items to dictionary");

            Parallel.ForEach(workerItems, wi =>
            {
                workerDict.TryAdd(wi.ID, wi);
                Console.WriteLine($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                $" Added {wi.GetType().Name} object with ID {wi.ID}.");
            });

            try
            {
                // KeyAlreadyExistsException ???
                workerDict.TryAdd(1, new WorkerItemBase());

                if (workerDict.TryGetValue(1, out IWorkerItem? found))
                {
                    Console.WriteLine($"Found: {found}.");
                }
                else
                {
                    Console.WriteLine("Worker item not found.");
                }

                if (workerDict.ContainsKey(1))
                {
                    Console.WriteLine($"ID 1 found: {found}.");
                }
                else
                {
                    Console.WriteLine("Worker item not found.");
                }

                //var newItem = new WorkerItemBase();

                //if (workerDict.Contains(newItem))
                //{
                //    Console.WriteLine($"{nameof(newItem)} found: {newItem}.");
                //}
                //else
                //{
                //    Console.WriteLine($"{nameof(newItem)} not found.");
                //}
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" {ex}");
            }

            if (workerDict.TryGetValue(1, out var workerItem))
            {
                Console.WriteLine($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                $" Found {workerItem.GetType().Name} with ID {workerItem.ID}.");
            }
            else
            {
                Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                $" Worker item not found.");
            }

            using var cts = new CancellationTokenSource();

            var workersTask = new Task(() =>
            {
                MiniProfiler.Current.Step("START: Running all worker tasks");

                Parallel.ForEach(workerItems, async workerItem =>
                {
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" Starting {workerItem.GetType().Name}...");
                    
                    await workerItem.DoWorkAsync(cts.Token);
                    
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {workerItem.GetType().Name}.{nameof(IWorkerItem.DoWorkAsync)}() finished.");
                });
                
                Console.WriteLine($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                    $" All tasks started. Press 'c' to cancel...");

                MiniProfiler.Current.Step("END: Running all worker tasks");

            }, cts.Token);

            Task consoleReadTask = new(() =>
            {
                var key = Console.ReadKey();
                if (new[] { ConsoleKey.C }.Contains(key.Key))
                {
                    cts.Cancel();
                }
            }, cts.Token);


            //Task.WaitAny(workersTask, consoleReadTask);
            workersTask.Start();
            consoleReadTask.Start();
            await workersTask;

            Console.WriteLine($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                $" Free / clean up resources...");

            MiniProfiler.Current.Step("Cleaning up / disposing");

            Parallel.ForEach(workerDict, kvp =>
            {
                IWorkerItem wi = kvp.Value;

                try
                {
                    wi.Dispose();

                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                    $" Disposed {nameof(IWorkerItem)} with Key/ID++{kvp.Key}.");

                    // QUESTION: Removing already disposed object from Dictionary ???
                    // Or will ObjectDisposedException be thrown?
                    //if (workerDict.ContainsValue(wi))
                    if (workerDict.TryRemove(kvp.Key, out IWorkerItem? found))
                    {
                        Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" Found & Removed: {found}.");
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" {ex}");
                }
            });

        }

    }
}
