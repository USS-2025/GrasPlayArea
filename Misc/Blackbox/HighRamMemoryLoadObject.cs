using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace Blackbox
{
    public class HighRamMemoryLoadObject : WorkerItemBase
    {
        public const int ALLOC_SIZE = 1024 * 1024 * 1024; // 1 GB

        private MemoryStream _ms;

        public HighRamMemoryLoadObject()
            : base()
        {
            _ms = new MemoryStream();
        }

        [Benchmark]
        public override async Task DoWorkAsync(CancellationToken? cancelToken = null)
        {
            // Fix for CS4008: Change Task constructor to Task.Run to return a Task instead of void
            Task workerTask = Task.Run(() =>
                Parallel.For(0, this.LoopCount, i =>
                {
                    // Simulate memory work
                    byte[] bytes = new byte[ALLOC_SIZE]; // Allocate 1 MB

                    _ms = new MemoryStream(bytes);

                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                    $" Allocated {bytes.GetFormattedByteSize()}," +
                                    $" size of {nameof(MemoryStream)}: {_ms.GetFormattedByteSize()}.");
                })
            );

            await Task.WhenAll(workerTask, Task.Delay(this.MinDuration, cancelToken ?? CancellationToken.None));
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(MemoryStream)}:" +
                $" {nameof(_ms.Length)}=={_ms.Length}; {nameof(_ms.Capacity)}=={_ms.Capacity}";
        }
    }
}
