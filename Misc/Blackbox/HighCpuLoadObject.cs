using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace Blackbox
{
    public class HighCpuLoadObject : WorkerItemBase
    {

        [Benchmark]
        public override async Task DoWorkAsync(CancellationToken? cancelToken = null)
        {
            // Fix for CS4008: Change Task constructor to Task.Run to return a Task instead of void
            Task workerTask = Task.Run(() =>
                Parallel.For(0, this.LoopCount, i =>
                {
                    // Simulate CPU work
                    double result = Math.Sqrt(i);

                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                    $" Finished square root extraction of {i}: {result:F2}");
                })
            );

            await Task.WhenAll(workerTask, Task.Delay(this.MinDuration, cancelToken ?? CancellationToken.None));
        }
    }
}
