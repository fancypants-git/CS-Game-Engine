using System.Reflection;
using Engine.Windowing;
using Engine.Attributes;
using Engine.Debugging;

namespace Engine.Internals;

public static class EngineBootstrap
{
    public static void Main()
    {
        // var entryType = AppDomain.CurrentDomain
        //     .GetAssemblies()
        //     .SelectMany(a => a.GetTypes())
        //     .FirstOrDefault(t => t.GetCustomAttribute<GameEntryAttribute>() != null);

        // if (entryType == null)
        // {
        //     Debug.Log("No class with [GameEntry] Attribute was found! Make sure to mark your Program Class as [GameEntry]");
        //     Debug.Log(LogType.Exit, "Exiting Bootstrap attempt early.");
        //     return;
        // }

        // var instance = (Game?)Activator.CreateInstance(entryType);
        // if (instance == null)
        // {
        //     Debug.LogFatal("Failed to create an instance of the GameEntry.");
        //     Debug.Log(LogType.Exit, "Exiting Bootstrap attempt early.");
        //     return;
        // }
        // instance.Run();
    }
}
