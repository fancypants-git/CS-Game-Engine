namespace Engine.Windowing;

public static class EngineApplication
{
    public static bool IsRunning { get; private set; } = true;
    public static bool FatalError { get; private set; } = false;
    
    public static void RequestFatalShutdown()
    {
        IsRunning = false;
        FatalError = true;
    }
    
    public static void Exit()
    {
        IsRunning = false;
    }
}
