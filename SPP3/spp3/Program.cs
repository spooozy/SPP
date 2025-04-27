using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

class Program{
    static void Main(){
        Task1();
        Task2();
        Task3();
        Task4();
    }

    private static void Task1() {
        Console.WriteLine("\nTask 1. Enter path to .dll or .exe file");
        string? buildPath = Console.ReadLine();
        Console.WriteLine();
        //D:\BSUIR_LABS\MDP\SPP2\bin\Debug\net9.0\SPP2.dll
        ParseBuid parseBuid = new ParseBuid(buildPath);
        parseBuid.ParsePublic();
        parseBuid.ParseExportClass();
    }
    private static void Task2() {
        string input1, input2;

        Console.WriteLine("\nTask 2. Enter buffer limit");
        input1 = Console.ReadLine();

        Console.WriteLine("Task 2. Enter flush interval");
        input2 = Console.ReadLine();

        if(int.TryParse(input1, out int bufferLimit) && int.TryParse(input2, out int flushInterval)){
            LogBuffer logBuffer = new LogBuffer(bufferLimit, TimeSpan.FromSeconds(flushInterval));
            int i = 0;
            string? line;
            while (i<8){
                Console.WriteLine($"Enter №{i}");
                line = Console.ReadLine();
                logBuffer.Add($"Console Input: {line}");
                i++;
            }
            logBuffer.Dispose();
        } else {Console.WriteLine("Wrong input");}
    }

    private static void Task3(){

        Console.WriteLine("\nTask 3. Enter anything to start");
        Console.ReadLine();
        var tasks = new TaskQueue.TaskDelegate[]
        {
            () => { Thread.Sleep(100); Console.WriteLine("Task 1 done"); }, // заменить на 100
            () => { Thread.Sleep(1500); Console.WriteLine("Task 2 done"); },
            () => { Thread.Sleep(500);  Console.WriteLine("Task 3 done"); },
            () => { Thread.Sleep(2000); Console.WriteLine("Task 4 done"); }
        };

        Console.WriteLine("Starting tasks...");
        Parallel.WaitAll(tasks, maxThreads: 2);
        Console.WriteLine("All tasks completed!");
    }

    private static void Task4(){
        Console.WriteLine("\nTask 4. Enter anything to start");
        Console.ReadLine();

        DynamicList<string> dinosaurs = new DynamicList<string>();

        Console.WriteLine("\nCapacity: {0}", dinosaurs.Capacity);

        dinosaurs.Add("Tyrannosaurus");
        dinosaurs.Add("Amargasaurus");
        dinosaurs.Add("Mamenchisaurus");
        dinosaurs.Add("Deinonychus");
        dinosaurs.Add("Compsognathus");
        Console.WriteLine();
        foreach(string dinosaur in dinosaurs)
        {
            Console.WriteLine(dinosaur);
        }

        Console.WriteLine("\nCapacity: {0}", dinosaurs.Capacity);
        Console.WriteLine("Count: {0}", dinosaurs.Count);

        Console.WriteLine("\ndinosaurs[3]: {0}", dinosaurs[3]);

        Console.WriteLine("\nRemove(\"Compsognathus\")");
        dinosaurs.Remove("Compsognathus");

        Console.WriteLine("\nRemoveAt([1])");
        dinosaurs.RemoveAt(1);

        Console.WriteLine();
        foreach(string dinosaur in dinosaurs)
        {
            Console.WriteLine(dinosaur);
        }

        Console.WriteLine("Capacity: {0}", dinosaurs.Capacity);
        Console.WriteLine("Count: {0}", dinosaurs.Count);

        Console.WriteLine("\nClear()");
        dinosaurs.Clear();
        Console.WriteLine("Capacity: {0}", dinosaurs.Capacity);
        Console.WriteLine("Count: {0}", dinosaurs.Count);
    }
}