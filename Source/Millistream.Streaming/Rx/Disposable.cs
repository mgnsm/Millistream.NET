using System;

namespace Millistream.Streaming.Rx
{
    /// <summary>
    /// Provides a set of static methods for creating <see cref="IDisposable"/> objects.
    /// </summary>
    internal static class Disposable
    {
        /// <summary>
        /// Represents a disposable that does nothing on disposal.
        /// </summary>
        private sealed class EmptyDisposable : IDisposable
        {
            private EmptyDisposable() { }

            /// <summary>
            /// Singleton default disposable.
            /// </summary>
            internal static EmptyDisposable Instance { get; } = new EmptyDisposable();

            /// <summary>
            /// Does nothing.
            /// </summary>
            public void Dispose() { }
        }

        /// <summary>
        /// Gets the disposable that does nothing when disposed.
        /// </summary>
        internal static IDisposable Empty => EmptyDisposable.Instance;
    }
}