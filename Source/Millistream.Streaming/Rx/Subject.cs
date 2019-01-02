using System;
using System.Threading;

namespace Millistream.Streaming.Rx
{
    /// <summary>
    /// Class from System.Reactive (Rx.NET) that represents an object that is both an observable sequence as well as an observer.
    /// Each notification is broadcasted to all subscribed observers.
    /// </summary>
    /// <typeparam name="T">The type of the elements processed by the subject.</typeparam>
    internal sealed class Subject<T> : IObservable<T>, IDisposable
    {
        #region Fields
        private static readonly SubjectDisposable[] s_empty = new SubjectDisposable[0];
        private static readonly SubjectDisposable[] s_terminated = new SubjectDisposable[0];
        private static readonly SubjectDisposable[] s_disposed = new SubjectDisposable[0];
        private SubjectDisposable[] _observers;
        private Exception _exception;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a subject.
        /// </summary>
        internal Subject() => Volatile.Write(ref _observers, s_empty);
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether the subject has observers subscribed to it.
        /// </summary>
        internal bool HasObservers => Volatile.Read(ref _observers).Length != 0;

        /// <summary>
        /// Indicates whether the subject has been disposed.
        /// </summary>
        internal bool IsDisposed => Volatile.Read(ref _observers) == s_disposed;
        #endregion

        #region Methods
        /// <summary>
        /// Notifies all subscribed observers about the end of the sequence.
        /// </summary>
        internal void OnCompleted()
        {
            for (; ; )
            {
                SubjectDisposable[] observers = Volatile.Read(ref _observers);
                if (observers == s_disposed)
                {
                    _exception = null;
                    break;
                }
                if (observers == s_terminated)
                    break;
                if (Interlocked.CompareExchange(ref _observers, s_terminated, observers) == observers)
                {
                    foreach (SubjectDisposable observer in observers)
                        observer.Observer?.OnCompleted();
                    break;
                }
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the specified exception.
        /// </summary>
        /// <param name="error">The exception to send to all currently subscribed observers.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
        internal void OnError(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            for (; ; )
            {
                SubjectDisposable[] observers = Volatile.Read(ref _observers);
                if (observers == s_disposed)
                {
                    _exception = null;
                    break;
                }
                if (observers == s_terminated)
                    break;
                _exception = error;
                if (Interlocked.CompareExchange(ref _observers, s_terminated, observers) == observers)
                {
                    foreach (SubjectDisposable observer in observers)
                        observer.Observer?.OnError(error);
                    break;
                }
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the arrival of the specified element in the sequence.
        /// </summary>
        /// <param name="value">The value to send to all currently subscribed observers.</param>
        internal void OnNext(T value)
        {
            SubjectDisposable[] observers = Volatile.Read(ref _observers);
            if (observers == s_disposed)
            {
                _exception = null;
                return;
            }
            foreach (var observer in observers)
                observer.Observer?.OnNext(value);
        }

        /// <summary>
        /// Subscribes an observer to the subject.
        /// </summary>
        /// <param name="observer">Observer to subscribe to the subject.</param>
        /// <returns>Disposable object that can be used to unsubscribe the observer from the subject.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observer"/> is null.</exception>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            SubjectDisposable disposable = default(SubjectDisposable);
            for (; ; )
            {
                SubjectDisposable[] observers = Volatile.Read(ref _observers);
                if (observers == s_disposed)
                {
                    _exception = null;
                    throw new ObjectDisposedException(string.Empty);
                }
                if (observers == s_terminated)
                {
                    Exception ex = _exception;
                    if (ex != null)
                        observer.OnError(ex);
                    else
                        observer.OnCompleted();
                    break;
                }

                if (disposable == null)
                    disposable = new SubjectDisposable(this, observer);

                int n = observers.Length;
                SubjectDisposable[] b = new SubjectDisposable[n + 1];
                Array.Copy(observers, 0, b, 0, n);
                b[n] = disposable;
                if (Interlocked.CompareExchange(ref _observers, b, observers) == observers)
                    return disposable;
            }
            return Disposable.Empty;
        }

        private void Unsubscribe(SubjectDisposable observer)
        {
            for (; ; )
            {
                SubjectDisposable[] a = Volatile.Read(ref _observers);
                int n = a.Length;
                if (n == 0)
                    break;

                int j = Array.IndexOf(a, observer);
                if (j < 0)
                    break;

                SubjectDisposable[] b = default(SubjectDisposable[]);
                if (n == 1)
                {
                    b = s_empty;
                }
                else
                {
                    b = new SubjectDisposable[n - 1];
                    Array.Copy(a, 0, b, 0, j);
                    Array.Copy(a, j + 1, b, j, n - j - 1);
                }
                if (Interlocked.CompareExchange(ref _observers, b, a) == a)
                    break;
            }
        }

        private sealed class SubjectDisposable : IDisposable
        {
            private Subject<T> _subject;
            private IObserver<T> _observer;

            internal SubjectDisposable(Subject<T> subject, IObserver<T> observer)
            {
                _subject = subject;
                Volatile.Write(ref _observer, observer);
            }

            public void Dispose()
            {
                IObserver<T> observer = Interlocked.Exchange(ref _observer, null);
                if (observer == null)
                    return;

                _subject.Unsubscribe(this);
                _subject = null;
            }

            internal IObserver<T> Observer => Volatile.Read(ref _observer);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Subject{T}"/> class and unsubscribes all observers.
        /// </summary>
        public void Dispose()
        {
            Interlocked.Exchange(ref _observers, s_disposed);
            _exception = null;
        }
        #endregion
    }
}