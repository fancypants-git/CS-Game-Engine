using System.Reflection;
using Engine.Windowing;
using Engine.Attributes;
using Engine.Debugging;

namespace Engine.Internals;

public static class EngineBootstrap
{
    public static void Main()
    {
        var entryType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.GetCustomAttribute<GameEntryAttribute>() != null);

        if (entryType == null)
            throw new Exception("No class with [GameEntry] was found.");

        var instance = (Game)Activator.CreateInstance(entryType);
        if (instance == null)
        {
            Debug.LogFatal("No GameEntry Attribute instance was found. Make sure to add this to your main class!");
            Debug.LogPrefixed(LogType.Exit, "Exiting Bootstrap attempt early.");
            return;
        }
        instance.Run();
    }
}
