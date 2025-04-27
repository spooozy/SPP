using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

public class OSHandle : IDisposable
{
    public IntPtr Handle { get; private set; }

    public OSHandle(IntPtr handle)
    {
        Handle = handle;
    }

    private void ReleaseHandle()
    {
        if (Handle != IntPtr.Zero)
        {
            CloseHandle(Handle);
            Handle = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        ReleaseHandle();
        GC.SuppressFinalize(this);
    }

    ~OSHandle()
    {
        ReleaseHandle();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);
}