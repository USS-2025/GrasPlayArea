using System.Collections.Concurrent;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Blackbox;
using StackExchange.Profiling;

#pragma warning disable S125 
namespace DictionaryTests
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

            object[] locks = new object[workerItems.Length];
            Array.Fill(locks, new object());

            // ConcurrentDictionary is thread-safe :-)
            ConcurrentDictionary<int, IWorkerItem> workerDictIntKey = new();
            ConcurrentDictionary<IWorkerItem, IWorkerItem> workerDictObjectKey = new();
            // HashTable is not thread-safe :-(
            HashSet<IWorkerItem> hashSet = new();

            using (MiniProfiler.Current.Step("Adding items to dictionary parallel"))
            {
                Parallel.ForEach(workerItems, wi =>
                {
                    workerDictIntKey.TryAdd(wi.ID, wi);
                    workerDictObjectKey.TryAdd(wi, wi);
                });

                // end index is exclusive ???
                Parallel.For(fromInclusive: 0, toExclusive: workerItems.Length, body: i =>
                {
                    // HashTable is not thread-safe :-(
                    lock (locks[i])
                    {
                        hashSet.Add(workerItems[i]);
                    }
                });

                Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Added {workerItems.Length} items.");
            }

            try
            {
                // Experiment: Adding already existing items
                // KeyAlreadyExistsException ???

                var existing = workerItems[0];

                workerDictIntKey.TryAdd(1, existing);
                workerDictObjectKey.TryAdd(existing, existing);

                IWorkerItem? found;

                if (workerDictIntKey.TryGetValue(1, out found))
                {
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" ID 1 found: {found}.");
                }
                else
                {
                    Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Worker item not found.");
                }

                if (workerDictIntKey.ContainsKey(1))
                {
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" ID 1 found: {found}.");
                }
                else
                {
                    Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Worker item not found.");
                }

                if (workerDictObjectKey.TryGetValue(existing, out found))
                {
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Existing item found: {found}.");
                }
                else
                {
                    Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Existing item not found.");
                }

                if (workerDictObjectKey.ContainsKey(existing))
                {
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Existing item found: {found}.");
                }
                else
                {
                    Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Existing item not found.");
                }

                if (hashSet.Contains(existing))
                {
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Existing item found: {found}.");
                }
                else
                {
                    Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Existing item not found.");
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

            using var cts = new CancellationTokenSource();

            var workersTask = new Task(() =>
            {
                using (MiniProfiler.Current.Step("START: Running all worker tasks parallel"))
                {
                    Parallel.ForEach(workerItems, async workerItem =>
                    {
                        Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                                $" Starting {workerItem.GetType().Name}...");

                        await workerItem.DoWorkAsync(cts.Token);

                        Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                                $" {workerItem.GetType().Name}.{nameof(IWorkerItem.DoWorkAsync)}() finished.");
                    });
                }

                Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                    $" All tasks started. Press 'c' to cancel...");

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

            Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                $" Free / clean up resources...");

            using (MiniProfiler.Current.Step("Cleaning up / disposing worker items parallel"))
            {
                Parallel.ForEach(workerDictIntKey, kvp =>
                {
                    IWorkerItem wi = kvp.Value;

                    try
                    {
                        wi.Dispose();

                        Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Disposed {nameof(IWorkerItem)} with Key/ID=={wi.ID}.");

                        // QUESTION: Removing already disposed object from Dictionary ???
                        // Or will ObjectDisposedException be thrown?
                        //if (workerDict.ContainsValue(wi))
                        if (workerDictIntKey.TryRemove(kvp.Key, out IWorkerItem? found))
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

                Parallel.ForEach(workerDictObjectKey, kvp =>
                {
                    IWorkerItem wi = kvp.Value;

                    try
                    {
                        wi.Dispose();

                        Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                        $" Disposed {nameof(IWorkerItem)} with Key/ID=={wi.ID}.");

                        // QUESTION: Removing already disposed object from Dictionary ???
                        // Or will ObjectDisposedException be thrown?
                        //if (workerDict.ContainsValue(wi))
                        if (workerDictObjectKey.TryRemove(kvp.Key, out IWorkerItem? found))
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
}
