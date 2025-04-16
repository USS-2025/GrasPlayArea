using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Blackbox
{
    public class HandlingUnmanagedResource: WorkerItemBase
    {
        // Zusätzliche verwaltete Ressource in der abgeleiteten Klasse
        private Stream? _managedResourceSubclass;

        // Zusätzliche nicht verwaltete Ressource in der abgeleiteten Klasse
        private IntPtr _unManagedResourceSubclass = IntPtr.Zero;

        public HandlingUnmanagedResource()
        {
            // Beispielinitialisierung der zusätzlichen verwalteten Ressource
            _managedResourceSubclass = new MemoryStream();
            
            // Allocate unmanaged resource
            _unManagedResourceSubclass = Marshal.AllocHGlobal(1024); // Allocate 1 KB
        }

        public override async Task DoWorkAsync(CancellationToken? cancelToken = null)
        {
            // Simulate work with unmanaged resource
            await Task.Delay(this.MinDuration, cancelToken ?? CancellationToken.None);
        }

        public override string ToString()
        {
            return $"{base.ToString()}, Unmanaged Resource: {_unManagedResourceSubclass}";
        }

        // Flag für die Subklasse
        private int disposed = 0;

        [Benchmark]
        protected override void Dispose(bool disposing)
        {
            Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {nameof(HandlingUnmanagedResource)}: {nameof(Dispose)}({disposing}) called.");

            // Atomic check & assignment to disposed
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                if (disposing)
                {
                    // Freigabe der zusätzlichen verwalteten Ressource
                    if (_managedResourceSubclass != null)
                    {
                        _managedResourceSubclass.Dispose();
                        // Don't forget to free up reference (GC always checks references)!
                        _managedResourceSubclass = null;
                    }

                    // add more here if needed
                    Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {nameof(WorkerItemBase)}: Disposed managed resources.");
                }

                // Freigabe der zusätzlichen nicht verwalteten Ressource
                if (_unManagedResourceSubclass != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_unManagedResourceSubclass);
                    _unManagedResourceSubclass = IntPtr.Zero;
                }

                Trace.TraceInformation($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                           $" {nameof(WorkerItemBase)}: De-allocated unmanaged resources.");
            }
            else
            {
                Trace.TraceWarning($"{DateTime.Now:T} [{Environment.CurrentManagedThreadId:000}]:" +
                                $" {nameof(HandlingUnmanagedResource)}.Dispose() already called ({nameof(disposed)}=={disposed}).", "WARN");
            }

            // !!! NOTE: Very important: Don't forget to call the base class Dispose method !!!
            base.Dispose(disposing);
        }

        // Finalizer der Subklasse (optional, falls nur als Fallback gedacht)
        ~HandlingUnmanagedResource()
        {
            Dispose(false);
        }
    }

}
