using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable S125
namespace Blackbox
{
    public class WorkerItemBase : IWorkerItem,
                                IEquatable<WorkerItemBase>,
                                IEqualityComparer<WorkerItemBase>
    {
        private static int _objCounter;

        public int ID { get; }

        public int LoopCount { get; set; } = 10;

        public TimeSpan MinDuration { get; set; } = TimeSpan.FromSeconds(3);

        public WorkerItemBase()
        {
            // atomic increment to ensure thread safety
            this.ID = Interlocked.Increment(ref _objCounter);

            Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" Created {this.GetType().Name} object with ID {ID}.");
        }

        public WorkerItemBase(int loopCount, TimeSpan minDuration)
            : this()
        {
            this.LoopCount = loopCount;
            this.MinDuration = minDuration;
        }

        [Benchmark]
        public virtual async Task DoWorkAsync(CancellationToken? cancelToken = null)
            => await Task.Delay(this.MinDuration, cancelToken ?? CancellationToken.None);

        public override string ToString()
            => $"{GetType().Name} (ID: {this.ID})";

        public virtual bool Equals(WorkerItemBase? other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return this.ID.Equals(other.ID);
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as WorkerItemBase);
        }

        public bool Equals(WorkerItemBase? wi1, WorkerItemBase? wi2)
        {
            // Schnell-Check: Sind beide Referenzen identisch?
            if (ReferenceEquals(wi1, wi2))
                return true;

            // Wenn einer der beiden null ist, kann auch nicht gleich sein.
            if (wi1 is null || wi2 is null)
                return false;

            // Vergleiche ausschließlich die Email-Property, case-insensitive.
            return wi1.ID.Equals(wi2.ID);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public int GetHashCode([DisallowNull] WorkerItemBase obj)
        {
            return obj.GetHashCode();
        }

        // Flag, um doppelte Dispose-Aufrufe zu verhindern
        private int disposed = 0;

        // Protected virtual Dispose Methode
        protected virtual void Dispose(bool disposing)
        {
            Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {nameof(WorkerItemBase)}: {nameof(Dispose)}({disposing}) called.");

            // Atomic check & assignment to disposed
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                if (disposing)
                {
                    // Freigabe verwalteter Ressourcen
                    //if (managedResource != null)
                    //{
                    //    managedResource.Dispose();
                    // Nicht vergessen: Referenz freigeben!
                    //    managedResource = null;  
                    //}

                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {nameof(WorkerItemBase)}: Disposed managed resources.");
                }

                // Freigabe nicht verwalteter Ressourcen unabhängig vom disposing-Parameter
                //if (unmanagedResource != IntPtr.Zero)
                //{
                //    Marshal.FreeHGlobal(unmanagedResource);
                //    unmanagedResource = IntPtr.Zero;
                //}

                Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {nameof(WorkerItemBase)}: De-allocated unmanaged resources.");
            }
            else
            {
                Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {nameof(WorkerItemBase)}.Dispose() already called ({nameof(disposed)}=={disposed}).", "WARN");
            }
        }

        // Öffentliche Dispose-Methode (IDisposable-Implementierung)
        public void Dispose()
        {
            Dispose(true);
            // Unterdrücke den Finalizer, da bereits aufgeräumt wurde
            GC.SuppressFinalize(this);
        }

        // Finalizer als Sicherheitsnetz, falls Dispose nicht aufgerufen wurde
        ~WorkerItemBase()
        {
            Dispose(false);
        }

    }
}

