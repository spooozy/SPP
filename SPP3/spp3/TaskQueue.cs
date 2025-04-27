using System;
using System.Collections.Generic;
using System.Threading;

public class TaskQueue : IDisposable
{
    private readonly Queue<TaskDelegate> taskQueue = new Queue<TaskDelegate>();
    private readonly List<Thread> threads;
    private bool isStopped = false;
    public delegate void TaskDelegate();

    public TaskQueue(int threadCount)
    {
        threads = new List<Thread>(threadCount);
        for (int i = 0; i < threadCount; i++)
        {
            Thread thread = new Thread(Worker);
            thread.Start();
            threads.Add(thread);
        }
    }

    public void EnqueueTask(TaskDelegate task)
    {
        lock (taskQueue)
        {
            if (!isStopped)
            {
                taskQueue.Enqueue(task);
                Monitor.Pulse(taskQueue);
            }
        }
    }

    public void Stop()
    {
        lock (taskQueue)
        {
            isStopped = true;
            Monitor.PulseAll(taskQueue);
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    private void Worker()
    {
        while (true)
        {
            TaskDelegate task = null;
            lock (taskQueue)
            {
                while (taskQueue.Count == 0 && !isStopped)
                {
                    Monitor.Wait(taskQueue);
                }

                if (isStopped && taskQueue.Count == 0)
                {
                    return;
                }

                task = taskQueue.Dequeue();
            }

            task?.Invoke();
        }
    }
    public void Dispose()
    {
        Stop();
    }
}