namespace UDO.LOB.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class UdoSafe<T> : IDisposable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _disposed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool _disposeValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T _value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ReaderWriterLockSlim _valueLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// Initializes a new instance of the <see cref="UdoSafe{T}" /> class.
        /// </summary>
        /// <param name="disposeValue">
        /// If <c>true</c>, automatically dispose the value if it's disposable.
        /// Default is <c>true</c>.
        /// </param>
        public UdoSafe(bool disposeValue = true)
        {
            _disposeValue = disposeValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdoSafe{T}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="disposeValue">
        /// If <c>true</c>, automatically dispose the value if it's disposable.
        /// Default is <c>true</c>.
        /// </param>
        public UdoSafe(T value, bool disposeValue = true)
        {
            _disposeValue = disposeValue;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the value in a thread-safe way.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value
        {
            get
            {
                try
                {
                    _valueLock.EnterReadLock();
                    return _value;
                }
                finally
                {
                    _valueLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _valueLock.EnterWriteLock();
                    _value = value;
                }
                finally
                {
                    _valueLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _valueLock != null)
                {
                    // dispose the _value if it is disposable and not null
                    if (Value is IDisposable disposableValue && _disposeValue)
                    {
                        disposableValue?.Dispose();
                    }

                    // set the _value to null if it can be set to null
                    if (default(T) == null)
                    {
                        Value = default;
                    }

                    // dispose the lock
                    _valueLock.Dispose();

                    // set the lock to null
                    _valueLock = null;
                }

                _disposed = true;
            }
        }
    }
}
