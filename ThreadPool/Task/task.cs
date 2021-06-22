using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadPool.Task
{
    abstract class Task<TResult> : IMyTask<TResult>, IDisposable
    {
        protected Func<TResult> task;

        public bool IsCompleted { get; protected set; }
        public TResult Result { get; protected set; }

        public abstract Func<TResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> next);

        public abstract void Dispose();
    }
}
