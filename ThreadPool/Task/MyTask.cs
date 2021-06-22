using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadPool.Task
{
    abstract class MyTask<TResult> : IMyTask<TResult>
    {
        protected readonly ManualResetEvent _ResultReady;
        public bool IsCompleted { get; protected set; }
        public TResult Result
        {
            get
            {
                if (!IsCompleted)
                {
                    ExecuteTask();
                }
                _ResultReady.WaitOne();
                return Result;
            }
            protected set
            {
            }
        }

        public MyTask()
        {
            IsCompleted = false;
            Result = default;
            _ResultReady = new ManualResetEvent(false);
        }

        protected abstract void ExecuteTask();
        
        //public abstract IMyTask ContinueWith<TNewResult>(Func<TResult, TNewResult> next);
    }

    class SimpleTask<TResult> : MyTask<TResult>
    {
        public Func<TResult> Task { get; private set; }

        public SimpleTask(Func<TResult> func)
            : base()
        {
            Task = func;
        }

        protected override void ExecuteTask()
        {
            try
            {
                Result = Task();
                IsCompleted = true;
                _ResultReady.Set();
            }
            catch (Exception exc)
            {
                throw new AggregateException(exc);
            }
        }

       /* public IMyTask ContinueWith<TNewResult>(Func<TResult, TNewResult> next)
        {
            
        }*/
    }

    class ChainedTask<TInput, TResult> : MyTask<TResult>
    {
        public Func<TInput, TResult> Task { get; private set; }

        private TInput _input;

        public ChainedTask(Func<TInput, TResult> func)
            : base()
        {
            Task = func;
        }

        protected override void ExecuteTask()
        {
            try
            {
                Result = Task(_input);
                IsCompleted = true;
                _ResultReady.Set();
            }
            catch (Exception exc)
            {
                throw new AggregateException(exc);
            }
        }

        /*public IMyTask ContinueWith<TNewResult>(Func<TResult, TNewResult> next)
        {

        }*/
    }
}
