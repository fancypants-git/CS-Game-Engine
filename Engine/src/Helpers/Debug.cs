namespace Engine.Helpers;

public enum LogType
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Fatal = 3,
    Exit = 4,
    Launch = 5,
    Debug = 6,
}

public enum LogFilter
{
    Nothing = -1,
    Info = LogType.Info,
    Warning = LogType.Warning,
    Error = LogType.Error,
    Fatal = LogType.Fatal,
    Exit = LogType.Exit,
    Launch = LogType.Launch,
    Debug = LogType.Debug,
}

public static class Debug
{

    public static bool PrintLogWarnings = false;
    public static LogFilter[] LogFilter = [];

    public static void LogPrefixed(string type, params object?[] messages)
    {
        if (LogFilter.Contains(Helpers.LogFilter.Nothing))
            return;
        
        if (type.Length > 5 && PrintLogWarnings)
            LogPrefixed("DEV",
                "For readability, keep [type] of LogPrefixed(type, messages) shorter than 6 characters.");

        var timeString = DateTime.Now.ToString("HH:mm:ss");

        Console.Write($"[ {type} ]".PadRight(12) + $"[ {timeString} ]  >> ");

        foreach (var msg in messages) Console.Write(msg + " ");

        Console.WriteLine();
    }

    public static void LogPrefixed(LogType type, params object?[] messages)
    {
        // if a filter is set
        // and that filter is either LogFilter.Nothing or does not contain LogType type
        // do not log this message
        if (LogFilter.Length != 0 &&
            (LogFilter.Contains(Helpers.LogFilter.Nothing) ||
             !LogFilter.Contains((LogFilter)type)))
            return;
        
        var typeStr = type switch
        {
            LogType.Info => "INFO",
            LogType.Warning => "WARN",
            LogType.Error => "ERROR",
            LogType.Fatal => "FATAL",
            LogType.Launch => "START",
            LogType.Exit => "EXIT",
            LogType.Debug => "DEBUG",
            _ => "UNKWN"
        };

        LogPrefixed(typeStr, messages);
    }

    public static void LogInfo(params object?[] messages)
    {
        LogPrefixed(LogType.Info, messages);
    }

    public static void Log(params object?[] messages)
    {
        LogPrefixed(LogType.Debug, messages);
    }

    public static void LogWarn(params object?[] messages)
    {
        LogPrefixed(LogType.Warning, messages);
    }

    public static void LogError(params object?[] messages)
    {
        LogPrefixed(LogType.Error, messages);
    }
    
    public static void LogError(Exception exception)
    {
        LogPrefixed(LogType.Error, exception.GetType().Name, "occured in", exception.TargetSite, '|', exception.Message);
        LogPrefixed("STACK", exception.StackTrace?.TrimStart());
    }

    public static void LogFatal(params object?[] messages)
    {
        LogPrefixed(LogType.Fatal, messages);
    }
    
    public static void LogFatal(Exception exception)
    {
        LogPrefixed(LogType.Fatal, "Fatal", exception.GetType().Name, "occured in", exception.TargetSite, '|', exception.Message);
        LogPrefixed("STACK", exception.StackTrace);
    }

    public static void LogMemLeak(string name)
    {
        LogPrefixed("LEAK", $"Memory leak detected in {name} instance! Did not call Dispose().");
    }
}
