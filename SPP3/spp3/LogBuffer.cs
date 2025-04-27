using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks.Dataflow;

public class LogBuffer : IDisposable{

    private string logFilePath = "log.txt";
    private int bufferLimit;
    private TimeSpan flushInterval;  //в миллисекундах
    private PeriodicTimer flushTimer;
    private bool disposed;
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    private SemaphoreSlim writeLock = new SemaphoreSlim(1, 1);
    private readonly CancellationTokenSource cts = new CancellationTokenSource();
    public LogBuffer(int bufferLimit, TimeSpan flushInterval){

        this.bufferLimit = bufferLimit > 4 ? bufferLimit : throw new ArgumentOutOfRangeException(nameof(bufferLimit));
        this.flushInterval = flushInterval.TotalSeconds >= 5 
                            ? flushInterval 
                            : throw new ArgumentOutOfRangeException(nameof(flushInterval), "Interval must be greater than 5 seconds.");
        
        if(!File.Exists(logFilePath)) File.Create(logFilePath).Close();

        flushTimer = new PeriodicTimer(flushInterval);
        _ = RunTimerAsync(cts.Token);
    }
    private async Task FlushMessagesAsync(){
        if(messageQueue.IsEmpty) return;
        await writeLock.WaitAsync();
        try{
            var messageQueueToWrite = new List<string>();
            while(messageQueue.TryDequeue(out var message)){
                messageQueueToWrite.Add(message);
            }
            if(messageQueueToWrite.Count() > 0){
                await File.AppendAllLinesAsync(logFilePath, messageQueueToWrite);
                Console.WriteLine($"Log file + {messageQueueToWrite.Count()} files");
            }
        } finally {writeLock.Release();}
    }

    public void Add(string item){
        if(disposed) throw new ObjectDisposedException(nameof(LogBuffer));
        if(item == null) throw new ArgumentNullException(nameof(item));

        messageQueue.Enqueue(item);

        if(messageQueue.Count() >= bufferLimit){
            _ = FlushMessagesAsync();
        }
    }

    private async Task RunTimerAsync(CancellationToken cancellationToken){
        try{
            while (await flushTimer.WaitForNextTickAsync(cancellationToken)){
                await FlushMessagesAsync();
            }
        } catch(OperationCanceledException ex){
            Console.WriteLine(ex.Message);
        }
    }
    public void Dispose(){
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            cts.Cancel();
            flushTimer?.Dispose();
            cts.Dispose();
            writeLock?.Dispose();
        }
        disposed = true;
    }

    ~LogBuffer()
    {
        Dispose(false);
    }

}