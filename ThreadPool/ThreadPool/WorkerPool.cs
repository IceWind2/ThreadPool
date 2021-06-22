using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using ThreadPool.Task;
using System.Linq;

namespace ThreadPool.ThreadPool
{
    class WorkerPool : IDisposable
    {
        private readonly CancellationTokenSource _CancelTokenSource;
        private readonly Thread _Scheduler;
        private readonly object _Lock;
        private readonly List<ThreadWorker> _Pool;
        private readonly Queue<IMyTask> _TaskQueue;

        private readonly AutoResetEvent _QueueNotEmpty;
        private readonly AutoResetEvent _FreeWorker;
        private long _NFreeWorkers;

        WorkerPool(int WorkersCount)
        {
            _NFreeWorkers = WorkersCount;
            _CancelTokenSource = new CancellationTokenSource();
            _Pool = new List<ThreadWorker>();
            _TaskQueue = new Queue<IMyTask>();

            for (int i = 0; i < WorkersCount; i++)
            {
                _Pool.Add(new ThreadWorker(_CancelTokenSource.Token));
            }
             
            _QueueNotEmpty = new AutoResetEvent(false);
            _FreeWorker = new AutoResetEvent(true);
            _Scheduler = new Thread(new ThreadStart(Schedule));
        }

        public void Enqueue(IMyTask task)
        {
            lock(_Lock)
            {
                _QueueNotEmpty.Set();
                _TaskQueue.Enqueue(task);
            }
        }

        private void Schedule()
        {
            int WorkerID = -1;
            IMyTask task;

            while (!_CancelTokenSource.IsCancellationRequested)
            {
                _QueueNotEmpty.WaitOne();
                _FreeWorker.WaitOne();

                WorkerID = _Pool.FindIndex(Worker => !Worker.IsWorking);
                lock (_Lock)
                {
                    task = _TaskQueue.Dequeue();
                }
                _Pool[WorkerID].ExecuteTask(task);
                Interlocked.Decrement(ref _NFreeWorkers);

                _SetEvents();
            }
        }

        private void _SetEvents()
        {
            lock (_Lock)
            {
                if (_TaskQueue.Count > 0)
                {
                    _QueueNotEmpty.Set();
                }

                if (Interlocked.Read(ref _NFreeWorkers) > 0)
                {
                    _FreeWorker.Set();
                }
            }
        }

        public void Dispose()
        {
            // ????
            _CancelTokenSource.Cancel();

            _Pool.ForEach(worker => { worker.Awake(); });

            while (_Pool.Any(Worker => Worker.IsWorking)) { }
        }
    }
}
