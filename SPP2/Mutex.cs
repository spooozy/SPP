using System;
using System.IO;
using System.Threading;

namespace test
{
    [ExportClass]
    public class Mutex
{
    private int _lock = 0;

    public void Lock()
    {
        while (Interlocked.CompareExchange(ref _lock, 1, 0) != 0)
        {
            Thread.Yield();
        }
    }
    public void Unlock()
    {
        Interlocked.Exchange(ref _lock, 0);
    }
}}