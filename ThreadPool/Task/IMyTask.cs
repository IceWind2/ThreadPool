using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadPool.Task
{
    interface IMyTask
    {
        bool IsCompleted { get; }
    }

    interface IMyTask<TResult>
    {
        TResult Result { get; }

        //IMyTask<TResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> next);
    }
}
