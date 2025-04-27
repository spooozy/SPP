using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
public class ParseBuid{

    private string buildPath;
    public ParseBuid(string buildPath){
        this.buildPath = buildPath;
    }
    public void ParsePublic(){

        if(!CheckBuildPath()) return;

        try{
            Assembly assembly = Assembly.LoadFrom(buildPath);
                var types = assembly.GetExportedTypes()
                    .OrderBy(t => t.Namespace)
                    .ThenBy(t => t.Name);
            string? currentNamespace = null;
        foreach (var type in types)
                {
                    if (type.Namespace != currentNamespace)
                    {
                        currentNamespace = type.Namespace;
                        Console.WriteLine();
                        Console.WriteLine($"Namespace: {currentNamespace ?? "<без namespace>"}");                        
                    }
                    Console.WriteLine(type.FullName);
                }   
        }catch (Exception e){
            Console.WriteLine(e.Message);
        }
        
    }
    private bool CheckBuildPath(){
        if(!IsDllOrExeFile()) throw new BadImageFormatException("The file is not a valid .NET assembly");
        if (!File.Exists(buildPath)) throw new Exception("File doesn't exists");
        return true;
    }

    private bool IsDllOrExeFile(){
        string extension = Path.GetExtension(buildPath).ToLower();
        return extension == ".dll" || extension == ".exe";
    }

    public void ParseExportClass(){
        if(!CheckBuildPath()) return;
        try
        {
            Assembly assembly = Assembly.LoadFrom(buildPath);
            var exportedTypes = assembly.GetTypes()
                .Where(t => t.IsPublic);

            Console.WriteLine("\nPublic types with ExportClass attribute:");
            foreach (var type in exportedTypes)
            {
                var attributes = type.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if(attribute.GetType().Name == "ExportClassAttribute"){
                        Console.WriteLine(type.FullName);
                        break;
                    }
               
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при загрузке сборки: {ex.Message}");
        }
    }

}