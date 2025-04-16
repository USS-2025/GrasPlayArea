namespace Blackbox
{
    public interface IWorkerItem : IDisposable
    {
        public int ID { get; }

        public int LoopCount { get; set; }

        public TimeSpan MinDuration { get; set; }

        public Task DoWorkAsync(CancellationToken? cancelToken = null);
    }
}
