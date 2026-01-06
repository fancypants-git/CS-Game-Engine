using System.Diagnostics;
using System.Net;
using Engine.Windowing;
using JoltPhysicsSharp;

namespace Engine.Debugging;


public enum LogType : uint
{
    Info = 1 << 0,
    Warning = 1 << 1,
    Error = 1 << 2,
    Fatal = 1 << 3,
    Exit = 1 << 4,
    Launch = 1 << 5,
    Debug = 1 << 6,
    Stack = 1 << 7,
    Leak = 1 << 8,
}

public static partial class Debug
{
    // LOGGING //
    public static uint LogFilter = uint.MaxValue;
    
    private static readonly Dictionary<LogType, string> LogTypeLookup = new()
    {
        { LogType.Info, "INFO" },
        { LogType.Warning, "WARN" },
        { LogType.Error, "ERROR" },
        { LogType.Fatal, "FATAL" },
        { LogType.Exit, "EXIT" },
        { LogType.Launch, "START" },
        { LogType.Debug, "DEBUG" },
        { LogType.Stack, "STACK" },
    };
    
    [Conditional("DEBUG")]
    private static void LogInternal(LogType type, params string?[] messages)
    {
        if ((LogFilter & (uint)type) == 0)
            return;
        
        if (!LogTypeLookup.TryGetValue(type, out string typeString))
        {
            LogWarn("LogType type in LogInternal MUST be only one type.");
            return;
        }
            
        string timeString = DateTime.Now.ToString("HH:mm:ss");
        Console.Write($"[ {typeString} ]".PadRight(12) + $"[ {timeString} ]  >> ");
        foreach (var msg in messages) Console.Write(msg);
        Console.WriteLine();
    }
    
    [Conditional("DEBUG")]
    public static void Log(LogType type, params string?[] messages)
    {
        LogInternal(type, messages);
    }
    
    [Conditional("DEBUG")]
    public static void LogInfo(params string?[] messages)
    {
        
        LogInternal(LogType.Info, messages);
    }
    
    [Conditional("DEBUG")]
    public static void LogWarn(params string?[] messages)
    {
        LogInternal(LogType.Warning, messages);
    }
    
    [Conditional("DEBUG")]
    public static void LogErr(params string?[] messages)
    {
        LogInternal(LogType.Error, messages);
    }
    
    [Conditional("DEBUG")]
    public static void LogErr(Exception e)
    {
        LogInternal(LogType.Error, e.GetType().Name, " occured in ", e.TargetSite?.ToString(), " | ", e.Message);
        LogInternal(LogType.Stack, e.StackTrace?.TrimStart());
    }
    
    public static void LogFatal(params string?[] messages)
    {
    #if DEBUG
        LogInternal(LogType.Fatal, messages);
    #endif
        Application.RequestShutdown();
    }
    
    public static void LogFatal(Exception e)
    {
    #if DEBUG
        LogInternal(LogType.Fatal, "Fatal ", e.GetType().Name, " occured in ", e.TargetSite?.ToString(), " | ", e.Message);
        LogInternal(LogType.Stack, e.StackTrace?.TrimStart());
    #endif
        Application.RequestShutdown();
    }
    
    [Conditional("DEBUG")]
    public static void LogMemLeak(string name)
    {
        LogInternal(LogType.Leak, $"Memory leak detected in {name} instance! Did not call Dispose().");
    }
    
    [Conditional("DEBUG")]
    public static void Log(params string?[] messages)
    {
        LogInternal(LogType.Debug, messages);
    }
    
    [Conditional("DEBUG")]
    public static void Assert(bool condition, string? message = null)
    {
        if (!condition)
            LogFatal("Assertion failed: ", message);
    }
}
