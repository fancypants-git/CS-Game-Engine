namespace Engine.Windowing;

public static class EngineApplication
{
    public static bool IsRunning { get; private set; }
    public static WindowManager WindowManager { get; private set; }
    public static Game Game { get; private set; }

    public static void RequestShutdown()
    {
        IsRunning = false;
    }
}
