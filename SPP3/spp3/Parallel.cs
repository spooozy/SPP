using System;
using System.Threading;

public static class Parallel
{
    public static void WaitAll(TaskQueue.TaskDelegate[] tasks, int maxThreads = 4)
    {
        if (tasks == null) throw new ArgumentNullException(nameof(tasks));
        using (var taskQueue = new TaskQueue(maxThreads))
        {
            var countdown = new CountdownEvent(tasks.Length);
            foreach (var task in tasks)
            {
                TaskQueue.TaskDelegate wrappedTask = () =>
                {
                    try
                    {
                        task();
                    }
                    finally
                    {
                        countdown.Signal();
                    }
                };
                taskQueue.EnqueueTask(wrappedTask);
            }
            countdown.Wait();
        }
    }
}