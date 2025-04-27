using System;
using System.IO;
using System.Threading;
using test;
class Program
{
    private static int filesCopied = 0;
    private static test.Mutex mutex = new test.Mutex();
    static void Main() 
    {
        string sourceDirectory = "D:\\БГУИР\\Другое\\СС\\Посты";
        string destinationDirectory = "D:\\BSUIR_LABS\\Test";

        if (!Directory.Exists(sourceDirectory))
        {
            Console.WriteLine($"Исходный каталог '{sourceDirectory}' не существует.");
            return;
        }

        var taskQueue = new TaskQueue(4);

        CopyDirectory(sourceDirectory, destinationDirectory, taskQueue);

        taskQueue.Dispose();

        Console.WriteLine($"Скопировано файлов: {filesCopied}");
    }

    static void CopyDirectory(string sourceDir, string destinationDir, TaskQueue taskQueue)
    {
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destinationDir, fileName);

            taskQueue.EnqueueTask(() =>
            {
                try
                {
                    File.Copy(file, destFile, true);

                    mutex.Lock();
                    filesCopied ++;
                    mutex.Unlock();
                    
                    //Console.WriteLine($"Скопирован файл: {destFile}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при копировании файла '{fileName}': {ex.Message}");
                }
            });
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dir);
            string destDir = Path.Combine(destinationDir, dirName);
            CopyDirectory(dir, destDir, taskQueue);
        }
    }
}
