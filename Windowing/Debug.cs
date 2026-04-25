namespace Windowing;

public static class Debug
{
    public static void Log(object message)
    {
        string timeString = DateTime.Now.ToString("HH:mm:ss");
        Console.Write($"[ LOG ]".PadRight(12) + $"[ {timeString} ]  >> ");
        Console.WriteLine(message);
    }

    public static void LogWarn(object message)
    {
        string timeString = DateTime.Now.ToString("HH:mm:ss");
        Console.Write($"[ WARN ]".PadRight(12) + $"[ {timeString} ]  >> ");
        Console.WriteLine(message);
    }

    public static void LogErr(object message)
    {
        string timeString = DateTime.Now.ToString("HH:mm:ss");
        Console.Write($"[ ERROR ]".PadRight(12) + $"[ {timeString} ]  >> ");
        Console.WriteLine(message);
    }
}