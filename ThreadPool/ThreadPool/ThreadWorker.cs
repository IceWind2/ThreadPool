using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ThreadPool.Task;

namespace ThreadPool.ThreadPool
{
    class ThreadWorker
    {
        public bool IsWorking {get; set;}
        private CancellationToken _token;
        private Thread _thread;
        private readonly AutoResetEvent _NewTask;
        private IMyTask _task;

        public ThreadWorker (CancellationToken token)
        {
            IsWorking = false;
            _token = token;
            _NewTask = new AutoResetEvent(false);
            _thread = new Thread(new ThreadStart(Runtime));
            _task = default;
        }

        public void ExecuteTask(IMyTask task)
        {
            _task = task;
            _NewTask.Set();
        }

        public void Awake()
        {
            _NewTask.Set();
        }

        private void Runtime()
        {
            while (true)
            {
                _NewTask.WaitOne();

                if (_token.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    // cast??? Console.WriteLine(((IMyTask<>)_task).Result);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error occured: " + exc.Message);
                }
            }
        }
    }
}
