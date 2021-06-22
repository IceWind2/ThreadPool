using System;
using System.Collections.Generic;
using System.Text;
using ThreadPool.ThreadPool;

namespace ThreadPool.Task
{
    static class TaskFactory
    {
        static public IMyTask CreateSimpleTask<TResult> (WorkerPool workerpool, Func<TResult> func)
        {
            var task = new SimpleTask<TResult>(func);
            workerpool.Enqueue((IMyTask) task);

            return (IMyTask) task;
        }

        static public IMyTask CreateChainedTask<TInput, TNewResult> (WorkerPool workerpool, Func<TInput, TNewResult> func)
        {
            var task = new ChainedTask<TInput, TNewResult>(func);
            workerpool.Enqueue((IMyTask)task);

            return (IMyTask)task;
        }
    }
}
