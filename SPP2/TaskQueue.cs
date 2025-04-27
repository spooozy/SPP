using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

public delegate void TaskDelegate();

[ExportClass]
public class TaskQueue : IDisposable
{
    private readonly ConcurrentQueue<TaskDelegate> _taskQueue = new ConcurrentQueue<TaskDelegate>();
    private readonly List<Thread> _workers;
    private readonly ManualResetEvent _stopSignal = new ManualResetEvent(false);
    private readonly AutoResetEvent _newTaskSignal = new AutoResetEvent(false);
    private int _taskCount = 0;
    private readonly List<OSHandle> _threadHandles = new List<OSHandle>();

    public int TaskCount => _taskCount;

    public TaskQueue(int numberOfThreads)
    {
        _workers = new List<Thread>();
        for (int i = 0; i < numberOfThreads; i++)
        {
            Thread worker = new Thread(WorkerThread);
            worker.Start();
            _workers.Add(worker);
            
            IntPtr threadHandle = OpenThread(ThreadAccess.SYNCHRONIZE, false, worker.ManagedThreadId);
            if (threadHandle != IntPtr.Zero)
            {
                _threadHandles.Add(new OSHandle(threadHandle));
            }
        }
    }

    public void EnqueueTask(TaskDelegate task)
    {
        _taskQueue.Enqueue(task);
        Interlocked.Increment(ref _taskCount);
        _newTaskSignal.Set();
    }

    private void WorkerThread()
    {
        while (true)
        {
            int result = WaitHandle.WaitAny(new WaitHandle[] { _newTaskSignal, _stopSignal });
            if (result == 1)
                break;
            while (_taskQueue.TryDequeue(out TaskDelegate task))
            {
                task();
                if (Interlocked.Decrement(ref _taskCount) == 0)
                {
                    break;
                }
            }
        }
    }

    public void Stop()
    {
        _stopSignal.Set();
        foreach (var worker in _workers)
        {
            worker.Join();
        }
    }

    public void Dispose()
    {
        Stop();
        foreach (var handle in _threadHandles)
        {
            handle.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    ~TaskQueue()
    {
        foreach (var handle in _threadHandles)
        {
            handle.Dispose();
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, int dwThreadId);

    [Flags]
    private enum ThreadAccess : int
    {
        SYNCHRONIZE = 0x00100000
    }
}